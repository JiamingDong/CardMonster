using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 征募
/// 此卡使用后，获得%d点水晶。
/// </summary>
public class Draft : SkillInBattle
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

            bool isAlly = false;
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    isAlly = true;
                }
            }

            if (systemPlayerData.consumeGameObject == gameObject)
            {
                isAlly = true;
            }

            if (isAlly)
            {
                Dictionary<string, object> parameter = new();
                parameter.Add("CrystalAmount", launchMark);
                parameter.Add("Player", systemPlayerData.perspectivePlayer);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));

                launchMark = 0;

                yield break;
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
