using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回到过去
/// 被破坏前，将“哭泣天使·原型”洗入牌库
/// </summary>
public class LastWords2 : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.DestroyMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> parameter = new();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, string> cardData = new();
        cardData.Add("CardID", "");
        cardData.Add("CardName", "哭泣天使·原型");
        cardData.Add("CardType", "monster");
        cardData.Add("CardRace", "[\"woman\"]");
        cardData.Add("CardHP", "8");
        cardData.Add("CardFlags", null);
        cardData.Add("CardSkinID", "190532");
        cardData.Add("CardCost", "3");
        cardData.Add("CardEliteSkill", null);

        if (monsterInBattle.kind == "balance")
        {
            cardData.Add("CardKind", "{\"leftKind\":\"balance\"}");
            cardData.Add("CardSkill", "{\"coverage_attack\":0,\"drain_crystal\":1,\"magic\":3}");
        }
        else if (monsterInBattle.kind == "fortune")
        {
            cardData.Add("CardKind", "{\"leftKind\":\"fortune\"}");
            cardData.Add("CardSkill", "{\"coverage_attack\":0,\"drain_crystal\":1,\"chance\":5}");
        }

        parameter.Add("CardData", cardData);

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    parameter.Add("Player", battleProcess.systemPlayerData[i].perspectivePlayer);
                    break;
                }
            }
        }

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeDestroy = (GameObject)parameter["EffectTarget"];

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        return monsterBeDestroy == gameObject && (monsterInBattle.kind == "balance" || monsterInBattle.kind == "fortune");
    }
}
