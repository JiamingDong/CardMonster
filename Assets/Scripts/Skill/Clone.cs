using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������
/// �ҷ�ս���غϿ�ʼʱ�����ҷ���λ�ٻ�һֻ������ǰ����������ͬ��Ѫ��Ϊ1�Ĺ��ޣ������޷����׼�����һ���ҷ��غϿ�ʼʱ�ƻ���
/// </summary>
public class Clone : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

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

                if (isAlly && systemPlayerData.monsterGameObjectArray[j] == null)
                {
                    Dictionary<string, int> skill = new();
                    skill.Add("self_explosive", 0);
                    for (int k = 0; k < monsterInBattle.skillList.Count; k++)
                    {
                        SkillInBattle skillInBattle = monsterInBattle.skillList[k];

                        if (SkillUtils.IsBasicAttackEffect(skillInBattle))
                        {
                            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillClassName='" + skillInBattle.GetType().Name + "'")[0];
                            skill.Add(skillConfig["SkillEnglishName"], skillInBattle.GetSkillValue());
                        }
                    }

                    Dictionary<string, string> cardData = new();
                    cardData.Add("CardID", "");
                    cardData.Add("CardName", "����");
                    cardData.Add("CardType", monsterInBattle.type);
                    cardData.Add("CardKind", "{\"leftKind\":\"" + monsterInBattle.kind + "\"}");
                    cardData.Add("CardRace", null);
                    cardData.Add("CardHP", "1");
                    cardData.Add("CardFlags", null);
                    cardData.Add("CardSkinID", monsterInBattle.skinId);
                    cardData.Add("CardCost", "0");
                    cardData.Add("CardSkill", JsonConvert.SerializeObject(skill));

                    Dictionary<string, object> parameter = new();
                    parameter.Add("LaunchedSkill", this);
                    parameter.Add("EffectName", "Effect1");
                    parameter.Add("Player", systemPlayerData.perspectivePlayer);
                    parameter.Add("BattlePanelNumber", j);
                    parameter.Add("CardData", cardData);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.MonsterEnterBattle, parameterNode1));
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غϣ��ҳ����п�λ
    /// </summary>
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
