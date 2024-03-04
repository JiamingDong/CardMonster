using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 万物通
/// 使用时：本回合中，我方使用的下一张消耗品获得<征募%s>，并且会在相同的位置额外生效1次。
/// </summary>
public class KnowEverything : SkillInBattle
{
    int launchMark = 0;

    public override int AddValue(string source, int value)
    {
        launchMark = value;

        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] += value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, object> parameter2 = new();
                    parameter2.Add("LaunchedSkill", this);
                    parameter2.Add("EffectName", "Effect1");
                    parameter2.Add("Player", systemPlayerData.perspectivePlayer);
                    parameter2.Add("SkillName", "know_everything_derive");
                    parameter2.Add("SkillValue", launchMark);
                    parameter2.Add("Source", "Skill.KnowEverything.Effect1");

                    ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    parameterNode2.parameter = parameter2;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddPlayerSkill, parameterNode2));

                    launchMark = 0;

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 判断是否被使用的是此卡
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

        if (launchMark < 1)
        {
            return false;
        }

        //消耗品物体
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        //怪兽
        else if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        //装备
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}