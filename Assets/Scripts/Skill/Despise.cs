using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despise : SkillInBattle
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

        //消灭敌方怪兽，再消灭敌方装备
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player)
            {
                for (int j = 2; j >= 0; j--)
                {
                    GameObject go = systemPlayerData.monsterGameObjectArray[j];
                    if (go != null && go.GetComponent<MonsterInBattle>().GetCost() <= GetSkillValue())
                    {
                        Dictionary<string, object> destroyParameter = new();
                        destroyParameter.Add("LaunchedSkill", this);
                        destroyParameter.Add("EffectName", "Effect1");
                        destroyParameter.Add("EffectTarget", go);
                        destroyParameter.Add("Destroyer", gameObject);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = destroyParameter;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyMonster, parameterNode1));
                    }
                }


                for (int j = 2; j >= 0; j--)
                {
                    GameObject go = systemPlayerData.monsterGameObjectArray[j];
                    if (go != null)
                    {
                        MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();

                        Dictionary<string, string> equipment = monsterInBattle.equipment;

                        if (equipment != null)
                        {
                            string cardCost = equipment["CardCost"];

                            if (Convert.ToInt32(cardCost) <= GetSkillValue())
                            {
                                Dictionary<string, object> parameter1 = new();
                                parameter1.Add("LaunchedSkill", this);
                                parameter1.Add("EffectName", "Effect1");
                                parameter1.Add("EffectTarget", go);

                                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                                parameterNode1.parameter = parameter1;

                                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyEquipment, parameterNode1));
                            }
                        }
                    }
                }
            }
        }

        //消灭我方怪兽，再消灭我方装备
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 2; j >= 0; j--)
                {
                    GameObject go = systemPlayerData.monsterGameObjectArray[j];
                    if (go != null && go.GetComponent<MonsterInBattle>().GetCost() <= launchMark)
                    {
                        Dictionary<string, object> destroyParameter = new();
                        destroyParameter.Add("LaunchedSkill", this);
                        destroyParameter.Add("EffectName", "Effect1");
                        destroyParameter.Add("EffectTarget", go);
                        destroyParameter.Add("Destroyer", gameObject);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = destroyParameter;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyMonster, parameterNode1));
                    }
                }


                for (int j = 2; j >= 0; j--)
                {
                    GameObject go = systemPlayerData.monsterGameObjectArray[j];
                    if (go != null)
                    {
                        MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();

                        Dictionary<string, string> equipment = monsterInBattle.equipment;
                        if (equipment != null)
                        {
                            string cardCost = equipment["CardCost"];

                            if (Convert.ToInt32(cardCost) <= launchMark)
                            {
                                Dictionary<string, object> parameter1 = new();
                                parameter1.Add("LaunchedSkill", this);
                                parameter1.Add("EffectName", "Effect1");
                                parameter1.Add("EffectTarget", go);

                                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                                parameterNode1.parameter = parameter1;

                                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyEquipment, parameterNode1));
                            }
                        }
                    }
                }
            }
        }

        launchMark = 0;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、对方有没有怪兽
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