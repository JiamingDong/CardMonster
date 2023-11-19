using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 覆盖攻击
/// 对每种基础技能各限一次。发动随机伤害/魔法时，改为对所有敌方怪兽各发动一次随机伤害/魔法的效果。
/// 只是执行效果，所以不会触发连咒、散射、弹幕、法反等效果。
/// </summary>
public class CoverageAttack : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
        effectList.Add(Effect2);
    }

    [TriggerEffectCondition("Replace.Magic.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //所在怪兽的位置
        int position = -1;
        //对方玩家
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    position = j;
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        //选取技能目标
        for (int i = 0; i < 3; i++)
        {
            GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[i];

            if (effectTarget == null)
            {
                continue;
            }

            int skillValue = gameObject.GetComponent<Magic>().GetSKillValue();

            //伤害参数
            Dictionary<string, object> damageParameter = new();
            //当前技能
            damageParameter.Add("LaunchedSkill", this);
            //效果名称
            damageParameter.Add("EffectName", "Effect1");
            //受到伤害的怪兽
            damageParameter.Add("EffectTarget", effectTarget);
            //伤害数值
            damageParameter.Add("DamageValue", skillValue);
            //伤害类型
            damageParameter.Add("DamageType", DamageType.Magic);

            yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

            yield return null;
        }
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        if (parameter.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        GameObject go = skillInBattle.gameObject;

        if (go != gameObject)
        {
            return false;
        }

        return true;
    }

    [TriggerEffectCondition("Replace.Chance.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        System.Random random = new();

        //对方玩家
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        //对面1号位没怪兽就返回
        if (oppositePlayerMessage.monsterGameObjectArray[0] == null)
        {
            yield break;
        }

        //选取技能目标
        for (int i = 0; i < 3; i++)
        {
            GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[i];

            if (effectTarget == null)
            {
                continue;
            }

            int skillValue = gameObject.GetComponent<Chance>().GetSKillValue();

            if (skillValue > 0)
            {
                Dictionary<string, object> keyValuePairs = new();
                yield return StartCoroutine(NetworkMessageUtils.GetRandomResult(0, skillValue, keyValuePairs));
                skillValue = (int)keyValuePairs["RandomResult"];
            }

            //伤害参数
            Dictionary<string, object> damageParameter = new();
            //当前技能
            damageParameter.Add("LaunchedSkill", this);
            //效果名称
            damageParameter.Add("EffectName", "Effect2");
            //受到伤害的怪兽
            damageParameter.Add("EffectTarget", effectTarget);
            //伤害数值
            damageParameter.Add("DamageValue", skillValue);
            //伤害类型
            damageParameter.Add("DamageType", DamageType.Real);

            yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

            yield return null;
        }

    }
}
