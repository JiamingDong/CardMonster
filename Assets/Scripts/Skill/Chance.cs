using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机伤害
/// 随机选择对方一个怪兽作为目标，若技能数值为0，对目标造成0点伤害，否则，随机造成1到技能数值的伤害
/// </summary>
public class Chance : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
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

        //选取技能目标
        GameObject effectTarget;
        for (int i = 0; i < 3; i++)
        {
            if (oppositePlayerMessage.monsterGameObjectArray[i] != null)
            {
                effectTarget = oppositePlayerMessage.monsterGameObjectArray[random.Next(0, i + 1)];
                goto endOfTarget;
            }
        }
        yield break;
    endOfTarget:;

        int skillValue = GetSKillValue();

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
        damageParameter.Add("EffectName", "Effect1");
        //受到伤害的怪兽
        damageParameter.Add("EffectTarget", effectTarget);
        //伤害数值
        damageParameter.Add("DamageValue", skillValue);
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Real);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

        yield return null;
    }

    /// <summary>
    /// 判断是否是己方回合，对方场上有怪兽
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        bool isAlly = false;
        bool enemyHasMonster = false;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        isAlly = true;
                    }
                }
            }

            if (systemPlayerData.perspectivePlayer == Player.Enemy && systemPlayerData.monsterGameObjectArray[0] != null)
            {
                enemyHasMonster = true;
            }
        }

        return isAlly && enemyHasMonster;
    }
}