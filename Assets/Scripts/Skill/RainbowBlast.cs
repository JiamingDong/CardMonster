using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʺ���
/// ʹ��ʱ�������е������X����ʵ�˺���XΪ�㹹������ʱ�������ƿ��а�������Ӫ����������������
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

        Debug.Log("�ʺ���");
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

                            //�˺�����
                            Dictionary<string, object> damageParameter = new();
                            //��ǰ����
                            damageParameter.Add("LaunchedSkill", this);
                            //Ч������
                            damageParameter.Add("EffectName", "Effect1");
                            //�ܵ��˺��Ĺ���
                            damageParameter.Add("EffectTarget", systemPlayerData.monsterGameObjectArray[r]);
                            //�˺���ֵ
                            damageParameter.Add("DamageValue", launchMark);
                            //�˺�����
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
    /// �ж��Ƿ�ʹ�õ��Ǵ˿����Է���û�й���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //ʹ�����Ƶ����
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchMark < 1)
        {
            return false;
        }

        //����Ʒ����
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        //����
        else if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        //װ��
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