using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自然均衡
/// 我方战斗阶段开始时，我方手牌中所有自然阵营的怪兽水晶消耗变为%d。
/// </summary>
public class EqualityNature : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                GameObject monsterGameObject = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                if (monsterGameObject == gameObject)
                {
                    string kind = "nature";

                    for (int k = 0; k < 2; k++)
                    {
                        Dictionary<string, string> keyValuePairs = battleProcess.systemPlayerData[i].handMonster[k];

                        Dictionary<string, string> cardKind = JsonConvert.DeserializeObject<Dictionary<string, string>>(keyValuePairs["CardKind"]);

                        if (cardKind["leftKind"] == kind || (cardKind.ContainsKey("rightKind") && cardKind["rightKind"] == kind))
                        {
                            keyValuePairs["CardCost"] = GetSkillValue().ToString();

                            CardForShow cardForShow = battleProcess.systemPlayerData[i].handMonsterPanel[k].GetComponent<Transform>().Find("CardForShow").gameObject.GetComponent<CardForShow>();
                            cardForShow.cost = GetSkillValue();
                            cardForShow.costText.text = GetSkillValue().ToString();
                        }
                    }

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是当前回合角色
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}