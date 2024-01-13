using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ϲ��ը��
/// ʹ�ú󣬵з����еĹ���װ������Ʒ��ը����
/// </summary>
public class SurpriseGift : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player)
            {
                for (int j = 2; j >= 0; j--)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] != null)
                    {
                        Dictionary<string, string> cardData = new();
                        cardData.Add("CardID", "");
                        cardData.Add("CardName", "��Ʒ��ը��");
                        cardData.Add("CardType", "equip");
                        cardData.Add("CardKind", "{\"leftKind\":\"all\"}");
                        cardData.Add("CardRace", null);
                        cardData.Add("CardHP", "0");
                        cardData.Add("CardFlags", null);
                        cardData.Add("CardSkinID", "499081");
                        cardData.Add("CardCost", "4");
                        cardData.Add("CardSkill", "{\"small_burst\":3}");
                        cardData.Add("CardEliteSkill", null);

                        Dictionary<string, object> parameter1 = new();
                        parameter1.Add("CardData", cardData);
                        parameter1.Add("EquipmentTarget", systemPlayerData.monsterGameObjectArray[j]);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = parameter1;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.EquipmentEnterBattle, parameterNode1));
                        //yield return null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ�ʹ�õ��Ǵ˿����Է���û�й���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

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
                Debug.Log("Ⱥ����Ⱦ�ж�2");
                return false;
            }
        }
        //װ��
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                Debug.Log("Ⱥ����Ⱦ�ж�3");
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