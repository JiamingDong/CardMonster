using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 群体治疗
/// 我方战斗阶段，我方所有怪兽回复%d点生命值
/// </summary>
public class HealAll : SkillInBattle
{
    [TriggerEffect("^InRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        PlayerData playerMessage = null;//发动技能的怪兽所属玩家
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    playerMessage = battleProcess.systemPlayerData[i];
                    goto end;
                }
            }
        }
    end:;

        for (int i = 2; i > -1; i--)
        {
            GameObject go = playerMessage.monsterGameObjectArray[i];
            if (go != null)
            {
                //治疗
                Dictionary<string, object> treatParameter = new();
                //当前技能
                treatParameter.Add("LaunchedSkill", this);
                //效果名称
                treatParameter.Add("EffectName", "Effect1");
                //受到治疗的怪兽
                treatParameter.Add("EffectTarget", go);
                //治疗数值
                treatParameter.Add("TreatValue", GetSkillValue());

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = treatParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, parameterNode1));
                yield return null;
            }
        }
    }

    /// <summary>
    /// 判断是否是当前回合角色、我方有没有受伤的怪兽
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Ally)
            {
                GameObject[] gameObjects = battleProcess.systemPlayerData[i].monsterGameObjectArray;
                for (int j = 0; j < gameObjects.Length; j++)
                {
                    if (gameObjects[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
