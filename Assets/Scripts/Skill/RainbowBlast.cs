using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 彩虹冲击
/// 使用时：对所有敌人造成X点真实伤害，X为你构筑套牌时，怪兽牌库中包含的阵营数量（不含中立）
/// </summary>
public class RainbowBlast : SkillInBattle
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
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        HashSet<string> set = new();
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 2; j >= 0; j--)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, List<string>> initialDeck = systemPlayerData.initialDeck;
                    List<string> monsterDeck = initialDeck["Monster"];

                    for (int k = 0; k < monsterDeck.Count; k++)
                    {
                        Dictionary<string, string> cardConfig = Database.cardMonster.Query("AllCardConfig", "and CardID='" + monsterDeck[k] + "'")[0];
                        string kind = cardConfig["CardKind"];
                        Debug.Log(kind);
                        Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(kind);
                        foreach (var item in keyValuePairs)
                        {
                            if (item.Value != "all")
                            {
                                set.Add(item.Value);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("彩虹冲击");
        foreach (var item in set)
        {
            Debug.Log(item);
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player)
            {
                for (int k = 0; k < set.Count; k++)
                {
                    for (int j = 2; j >= 0; j--)
                    {
                        if (systemPlayerData.monsterGameObjectArray[j] != null)
                        {
                            int r = RandomUtils.GetRandomNumber(0, j);

                            //伤害参数
                            Dictionary<string, object> damageParameter = new();
                            //当前技能
                            damageParameter.Add("LaunchedSkill", this);
                            //效果名称
                            damageParameter.Add("EffectName", "Effect1");
                            //受到伤害的怪兽
                            damageParameter.Add("EffectTarget", systemPlayerData.monsterGameObjectArray[r]);
                            //伤害数值
                            damageParameter.Add("DamageValue", launchMark);
                            //伤害类型
                            damageParameter.Add("DamageType", DamageType.Real);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = damageParameter;

                            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));

                            break;
                        }
                    }
                }

                goto a;
            }
        }

    a:;

        launchMark = 0;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、对方有没有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //使用手牌的玩家
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

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

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                return false;
            }
        }

        return true;
    }
}