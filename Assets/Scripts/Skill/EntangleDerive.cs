using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 被束缚
/// 跳过战斗阶段且<飞翔>无效，持续到敌方回合结束
/// 跳过
/// </summary>
public class EntangleDerive : SkillInBattle
{
    [TriggerEffect(@"^Replace\..*", "Compare1")]
    [TriggerEffect(@"^Replace.Flying\..*", "Compare2")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽，效果的方法上的时机是不是InRoundBattle
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.result;

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (parameterNode.creator is SkillInBattle skillInBattle)
        {
            GameObject go = skillInBattle.gameObject;
            if (gameObject != go)
            {
                Debug.Log("gameObject != go");
                return false;
            }

            ParameterNode parameterNode1 = parameterNode.Parent;
            if (parameterNode1.opportunity == "InRoundBattle")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;

        GameObject go = skillInBattle.gameObject;

        return gameObject == go;
    }

    [TriggerEffect("^AfterRoundBattle$", "Compare3")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillName", "entangle_derive");
        parameter1.Add("Source", "Skill.Entangle.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断是否是我方回合
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
