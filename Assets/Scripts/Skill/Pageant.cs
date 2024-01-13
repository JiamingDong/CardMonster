using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 华服
/// 我方回合结束时，将一张华丽的道具卡加入我方牌组。
/// </summary>
public class Pageant : SkillInBattle
{
    [TriggerEffect("^AfterRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> parameter = new();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        int r = RandomUtils.GetRandomNumber(1, 3);

        Dictionary<string, string> cardData = new();
        switch (r)
        {
            case 1:
                cardData.Add("CardID", "");
                cardData.Add("CardName", "华丽的南瓜车");
                cardData.Add("CardType", "equip");
                cardData.Add("CardKind", "{\"leftKind\":\"all\"}");
                cardData.Add("CardRace", null);
                cardData.Add("CardHP", "5");
                cardData.Add("CardFlags", null);
                cardData.Add("CardSkinID", "800041");
                cardData.Add("CardCost", "3");
                cardData.Add("CardSkill", "{\"magic\":3,\"beauty_suit\":1,\"magic_outburst\":2}");
                cardData.Add("CardEliteSkill", null);
                break;

            case 2:
                cardData.Add("CardID", "");
                cardData.Add("CardName", "华丽的礼服");
                cardData.Add("CardType", "equip");
                cardData.Add("CardKind", "{\"leftKind\":\"all\"}");
                cardData.Add("CardRace", null);
                cardData.Add("CardHP", "4");
                cardData.Add("CardFlags", null);
                cardData.Add("CardSkinID", "800051");
                cardData.Add("CardCost", "1");
                cardData.Add("CardSkill", "{\"beauty_suit\":1,\"immunity\":0}");
                cardData.Add("CardEliteSkill", null);
                break;

            case 3:
                cardData.Add("CardID", "");
                cardData.Add("CardName", "一只水晶鞋");
                cardData.Add("CardType", "equip");
                cardData.Add("CardKind", "{\"leftKind\":\"all\"}");
                cardData.Add("CardRace", null);
                cardData.Add("CardHP", "0");
                cardData.Add("CardFlags", null);
                cardData.Add("CardSkinID", "800061");
                cardData.Add("CardCost", "0");
                cardData.Add("CardSkill", "{\"evacuate\":1,\"recycle_crystal\":1,\"crystal\":1}");
                cardData.Add("CardEliteSkill", null);
                break;
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
    /// 判断是否是己方回合
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
