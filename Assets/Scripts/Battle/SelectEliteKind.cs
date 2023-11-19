using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用精英怪兽时，弹出的选择阵营的界面的两张卡上挂的脚本，点击后会选择这个阵营
/// </summary>
public class SelectEliteKind : MonoBehaviour
{
    //DragHandCard传过来的卡牌目标位置等信息
    public ParameterNode parameterNode;

    public void OnClick()
    {
        PlayerAction playerAction = PlayerAction.GetInstance();

        CardForShow cardForShow = gameObject.GetComponentInParent<CardForShow>();

        Dictionary<string, string> cardData = cardForShow.GetCardData();

        string cardP = cardData["CardP"];
        string cardEP = cardData["CardEP"];

        JObject cardPD = JsonConvert.DeserializeObject<JObject>(cardP);
        JObject cardEPD = JsonConvert.DeserializeObject<JObject>(cardEP);

        Dictionary<string, int> cardPDictionary = new();
        foreach (var item in cardPD)
        {
            cardPDictionary.Add(item.Key, Convert.ToInt32(item.Value));
        }

        JToken eliteSkillL = cardEPD["leftSkill"];

        string nL = (string)eliteSkillL["name"];
        long vL = (long)eliteSkillL["value"];

        cardPDictionary.Add(nL, Convert.ToInt32(vL));

        string CardKind = cardData["CardKind"];
        JObject CardKindD = JsonConvert.DeserializeObject<JObject>(CardKind);

        string kind = CardKindD["leftKind"].ToString();

        Dictionary<string, object> cardDataInBattle = new()
        {
            { "CardID", cardData["CardID"] },
            { "CardName", cardData["CardName"] },
            { "CardType", cardData["CardType"] },
            { "CardKind", kind },
            { "CardRace", cardData["CardRace"] },
            { "CardHP", Convert.ToInt32(cardData["CardHP"]) },
            { "CardSkinID", cardData["CardSkinID"] },
            { "CardCost", Convert.ToInt32(cardData["CardCost"]) },
            { "CardP", cardPDictionary }
        };

        parameterNode.parameter.Add("CardDataInBattle", cardDataInBattle);

        SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.DragHandCard, parameterNode.parameter));

        StartCoroutine(playerAction.DoAction(playerAction.UseACard, parameterNode.parameter));

        Destroy(GameObject.Find("SelectEliteKindPrefabInstantiation"));
    }
}
