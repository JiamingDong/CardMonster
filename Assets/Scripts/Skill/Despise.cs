using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despise : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //����з����ޣ�������з�װ��
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

        //�����ҷ����ޣ��������ҷ�װ��
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer == player)
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
                            foreach (var item in equipment)
                            {
                                Debug.Log(item.Key + "----" + item.Value);
                            }

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

        MonsterInBattle monsterInBattle1 = gameObject.GetComponent<MonsterInBattle>();

        List<string> needRemoveSource = new();
        foreach (KeyValuePair<string, int> keyValuePair in sourceAndValue)
        {
            needRemoveSource.Add(keyValuePair.Key);
        }

        foreach (var item in needRemoveSource)
        {
            Dictionary<string, object> parameter4 = new();
            parameter4.Add("LaunchedSkill", this);
            parameter4.Add("EffectName", "Effect1");
            parameter4.Add("SkillName", "despise");
            parameter4.Add("Source", item);

            ParameterNode parameterNode4 = new();
            parameterNode4.parameter = parameter4;

            yield return battleProcess.StartCoroutine(monsterInBattle1.DoAction(monsterInBattle1.DeleteSkillSource, parameterNode4));
        }
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ�ʹ�õ��Ǵ˿����Է���û�й���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;

        //����Ʒ����
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                //Debug.Log("Ⱥ����Ϯ1");
                return false;
            }
        }
        //����
        else if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                //Debug.Log("Ⱥ����Ⱦ�ж�2");
                return false;
            }
        }
        //װ��
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                //Debug.Log("Ⱥ����Ⱦ�ж�3");
                return false;
            }
        }
        else
        {
            //Debug.Log("Ⱥ����Ϯ2");
            return false;
        }

        return true;
    }
}