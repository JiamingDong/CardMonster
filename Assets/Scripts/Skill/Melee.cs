using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 近战
/// 效果1：在回合战斗中，对对方1号位怪兽造成等于技能数值的物理伤害
/// 效果2：代替近战的效果1，代替效果为无效果
/// </summary>
public class Melee : SkillInBattle
{
    public Melee(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
        effectList.Add(Effect2);
    }

    [TriggerEffectCondition("InRoundBattle")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //发动技能的怪兽所属玩家的对方玩家
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

        GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[0];

        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "Melee.Effect1.ChoiceTarget";

        //Dictionary<string, object> targetCondition = new();
        //targetCondition.Add("LaunchedSkill", this);

        Dictionary<string, object> targetParameter = new();
        targetParameter.Add("LaunchedSkill", this);
        targetParameter.Add("SkillTarget", effectTarget);

        parameterNode1.parameter = targetParameter;
        //parameterNode1.condition = targetCondition;

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        //伤害参数
        Dictionary<string, object> damageParameter = new();
        //当前技能
        damageParameter.Add("LaunchedSkill", this);
        //效果名称
        damageParameter.Add("EffectName", "Effect1");
        //受到伤害的怪兽
        damageParameter.Add("EffectTarget", effectTarget);
        //伤害数值
        damageParameter.Add("DamageValue", GetSKillValue());
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Physics);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

        yield return null;
    }

    [TriggerEffectCondition("Replace.Melee.Effect1")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();

        List<string> replaceReason = (List<string>)parameter["ReplaceReason"];
        List<string> antiReplaceReason = (List<string>)parameter["AntiReplaceReason"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string replaceEffectName = (string)parameter["ReplaceEffectName"];

        if (skillInBattle != this)
        {
            yield break;
        }

        if (!replaceEffectName.Equals("Effect1"))
        {
            yield break;
        }

        GameObject monsterOfLaunchingSkill = skillInBattle.gameObject;

        int position = -1;//发动技能的怪兽位置
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == monsterOfLaunchingSkill)
                {
                    position = j;
                    goto end;
                }
            }
        }
    end:;

        if (position != 0 && !antiReplaceReason.Contains("Melee.Effect2") && replaceReason.Count == 0)
        {
            replaceReason.Add("Melee.Effect2");
        }

        yield return null;
    }
}