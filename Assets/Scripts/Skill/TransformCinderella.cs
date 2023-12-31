using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// �ҷ�ս���׶ο�ʼʱ������˹���װ���˴���<����>Ч����װ��������{��ṫ�� ��������}��
/// </summary>
public class TransformCinderella : SkillInBattle
{
    [TriggerEffect("^TransfromOpportunity$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, string> cardData = new();
        cardData.Add("CardID", "19020");
        cardData.Add("CardName", "��ṫ�� ��������");
        cardData.Add("CardType", "monster");
        cardData.Add("CardKind", "{\"leftKind\":\"balance\"}");
        cardData.Add("CardRace", "[\"woman\"]");
        cardData.Add("CardHP", "10");
        cardData.Add("CardFlags", "[\"1\",\"3\"]");
        cardData.Add("CardSkinID", "190201");
        cardData.Add("CardCost", "4");
        cardData.Add("CardSkill", "{\"magic\":6,\"beauty_suit\":2,\"evacuate\":1,\"pageant\":1}");
        cardData.Add("CardEliteSkill", null);

        Dictionary<string, object> parameter = new();
        parameter.Add("EffectTarget", gameObject);
        parameter.Add("CardData", cardData);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.TransformMonster, parameterNode1));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غϣ�������
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    Debug.Log(systemPlayerData.monsterGameObjectArray[j] == gameObject);
                    Debug.Log(gameObject.TryGetComponent(out BeautySuit _));
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject && gameObject.TryGetComponent(out BeautySuit _))
                    {
                        Debug.Log("�ж��Ƿ��Ǽ����غϣ�������");
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
