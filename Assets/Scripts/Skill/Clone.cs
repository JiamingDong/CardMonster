using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������
/// �ҷ�ս���غϿ�ʼʱ�����ҷ���λ�ٻ�һֻ������ǰ����������ͬ��Ѫ��Ϊ1�Ĺ��ޣ������޷����׼�����һ���ҷ��غϿ�ʼʱ�ƻ���
/// </summary>
public class Clone : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.AddComponent<MonsterInBattle>();

        int vacancy = 2;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, int> p = new();
                    for (int k = 0; k < monsterInBattle.skillList.Count; k++)
                    {
                        SkillInBattle skillInBattle = monsterInBattle.skillList[k];

                        if (skillInBattle.IsBasicAttackEffect())
                        {
                            p.Add(skillInBattle.GetType().Name.ToLower(), skillInBattle.GetSKillValue());
                        }
                    }

                    Dictionary<string, object> card = new();
                    card.Add("CardID", "");
                    card.Add("CardName", "����");
                    card.Add("CardType", monsterInBattle.type);
                    card.Add("CardKind", monsterInBattle.kind);
                    card.Add("CardRace", "");
                    card.Add("CardHP", 1);
                    card.Add("CardSkinID", monsterInBattle.skinID);
                    card.Add("CardCost", 0);
                    card.Add("CardP", p);

                    Dictionary<string, object> parameter = new();
                    parameter.Add("Player", systemPlayerData.perspectivePlayer);
                    parameter.Add("BattlePanelNumber", vacancy);
                    parameter.Add("CardData", card);

                    yield return StartCoroutine(gameAction.DoAction(gameAction.MonsterEnterBattle, parameter));

                    goto a;
                }

                if (systemPlayerData.monsterGameObjectArray[j] == null)
                {
                    vacancy = j;
                }
            }
        }

    a:;

        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غϣ��ҳ����п�λ
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally && systemPlayerData.monsterGameObjectArray[2] == null)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
