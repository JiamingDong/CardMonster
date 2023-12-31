using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// �ҷ�ս���׶ο�ʼʱ���ҷ�����������������ͬ��Ӫ���޿��ķ��ñ�Ϊ%d
/// </summary>
public class Equality : SkillInBattle
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
                    MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
                    string kind = monsterInBattle.kind;

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
    /// �ж��Ƿ��ǵ�ǰ�غϽ�ɫ
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