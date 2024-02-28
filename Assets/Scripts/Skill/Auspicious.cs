using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// �ҷ�ʹ������Ʒʱ����25%%����ʹ������Ʒϴ���ҷ����ƿ⣬��ԭʼ����-%s��
/// </summary>
public class Auspicious : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

        GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
        ConsumeInBattle consumeInBattle = consumeBeGenerated.GetComponent<ConsumeInBattle>();
        Dictionary<string, string> cardData = consumeInBattle.cardData;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        cardData["CardCost"] = (Convert.ToInt32(cardData["CardCost"]) - GetSkillValue()).ToString();

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect1");
        parameter2.Add("CardData", cardData);
        parameter2.Add("Player", Player.Ally);

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode2));
    }

    /// <summary>
    /// ����ʹ������Ʒ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        int r = RandomUtils.GetRandomNumber(1, 4);

        if (r > 1)
        {
            return false;
        }

        //����Ʒ����
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];

            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject && systemPlayerData.consumeGameObject == consumeBeGenerated)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
