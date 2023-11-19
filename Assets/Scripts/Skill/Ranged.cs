using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 远程
/// 效果1：在回合战斗中，若所在怪兽位置为1号位，对对方1个怪兽造成等于技能数值的物理伤害，优先级为1、2、3，
/// 若为2号位，优先级为3、2、1，若为3号位，优先级为2、3、1。
/// 效果2：代替远程的效果1，代替效果为无效果
/// </summary>
public class Ranged : SkillInBattle
{
    public Ranged(GameObjectInBattle gameObjectInBattle) : base()
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

        //对面1号位没怪兽就返回
        if (oppositePlayerMessage.monsterGameObjectArray[0] == null)
        {
            yield break;
        }

        //选取技能目标
        int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 2, 1, 0 }, new int[] { 1, 2, 0 } };
        GameObject effectTarget = null;
        for (int i = 0; i < 3; i++)
        {
            effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];
            if (effectTarget != null)
            {
                ParameterNode parameterNode1 = new();
                parameterNode1.opportunity = "Ranged.Effect1.ChoiceTarget";

                //parameterNode.condition.Add("LaunchedSkill", this);

                parameterNode1.parameter.Add("LaunchedSkill", this);
                parameterNode1.parameter.Add("SkillTarget", effectTarget);

                yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
            }
        }

        yield return null;

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

    [TriggerEffectCondition("Replace.Ranged.Effect1")]
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

        if (position == 0 && !antiReplaceReason.Contains("Ranged.Effect2") && replaceReason.Count == 0)
        {
            replaceReason.Add("Melee.Effect2");
        }

        yield return null;
    }
}