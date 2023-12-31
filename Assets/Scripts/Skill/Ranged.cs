using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 远程
/// 在回合战斗中，若所在怪兽位置为1号位，对对方1个怪兽造成等于技能数值的物理伤害，优先级为1、2、3，
/// 若为2号位，优先级为3、2、1，若为3号位，优先级为2、3、1。
/// </summary>
public class Ranged : SkillInBattle
{
    [TriggerEffect("^InRoundBattle$", "Compare1")]
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
        List<GameObject> nontargetList = new();
        List<GameObject> priorTargetList = new();
        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("NontargetList", nontargetList);
        parameter1.Add("PriorTargetList", priorTargetList);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.SelectEffectTarget, parameterNode1));
        yield return null;

        GameObject effectTarget = null;
        if (priorTargetList.Count > 0)
        {
            effectTarget = priorTargetList[0];
        }
        else
        {
            int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 2, 1, 0 }, new int[] { 1, 2, 0 } };

            for (int i = 0; i < 3; i++)
            {
                effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];
                if (effectTarget == null)
                {
                    continue;
                }

                bool isNontarget = false;
                foreach (GameObject go in nontargetList)
                {
                    if (go == effectTarget)
                    {
                        isNontarget = true;
                        break;
                    }
                }

                if (isNontarget && i != 2)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
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
        damageParameter.Add("DamageValue", GetSkillValue());
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Physics);

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode2));
        yield return null;
    }

    /// <summary>
    /// 判断是否是己方回合，在2、3号位，对方场上有怪兽
    /// </summary>
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
                for (int j = 1; j < systemPlayerData.monsterGameObjectArray.Length; j++)
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