using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 收藏界面卡组和英雄技能
/// </summary>
[Serializable]
public class DeckInCollection : MonoBehaviour
{
    public GameObject[] CardDeckBackgroundPanel = new GameObject[8];
    public Text DeckNameInputFieldText;

    //卡组
    public string deckId;
    public string deckName;
    public string heroSkillId;
    public string deckCard;

    public List<string> monsterCardInDeck;
    public List<string> itemCardInDeck;

    /// <summary>
    /// 怪兽还是道具，true怪兽，false道具
    /// </summary>
    public bool monsterOrItemInDeck = true;
    private int deckCardNumber = 8;
    private float cardInDeckX = 170;
    private float cardInDeckY = 233.5f;
    private float localScaleCoefficientInDeck = 1;//卡牌缩放系数
    private float cardClearanceDistanceInDeck = 10;//卡牌间距
    private float cardAddClearanceDistanceInDeckX;
    private float cardAddClearanceDistanceInDeckY;
    private float cardCoordinatesOriginInDeckX;
    private float cardCoordinatesOriginInDeckY;

    // Start is called before the first frame update
    void Start()
    {
        cardAddClearanceDistanceInDeckX = cardClearanceDistanceInDeck + cardInDeckX * localScaleCoefficientInDeck;//
        cardAddClearanceDistanceInDeckY = cardClearanceDistanceInDeck + cardInDeckY * localScaleCoefficientInDeck;
        cardCoordinatesOriginInDeckX = cardAddClearanceDistanceInDeckX / 2;
        cardCoordinatesOriginInDeckY = cardAddClearanceDistanceInDeckY / 2;

        //初始化卡组
        deckId = Database.cardMonster.Query("PlayerData", "and PlayerID='1'")[0]["DefaultDeckID"];

        SwitchDeck();
    }

    public void SwitchDeck()
    {
        List<Dictionary<string, string>> deckList = Database.cardMonster.Query("PlayerDeck", "and DeckID='" + deckId + "'");
        if (deckList.Count < 1)
            return;

        Dictionary<string, string> deck = deckList[0];
        heroSkillId = deck["HeroSkillID"];
        deckName = deck["DeckName"];
        deckCard = deck["DeckCard"];

        //8个卡组背景，并添加点击事件脚本
        for (int i = 0; i < 8; i++)
        {
            CardDeckBackgroundPanel[i] = GameObject.Find("CardDeckBackgroundPanel" + (i + 1).ToString());
            CardDeckBackgroundPanel[i].GetComponent<CardDeckBackground>().CardDeckBackgroundIndex = i;
        }

        //卡组名
        ChangeDeckName();

        //卡组图片
        monsterOrItemInDeck = true;
        ChangeDeckCardShow();

        //英雄技能
        ChangeHeroSkillInDeck(heroSkillId);

    }

    public void ChangeDeckName()
    {
        GameObject deckNameInputField = GameObject.Find("DeckNameInputField");
        deckNameInputField.GetComponent<InputField>().text = deckName;
    }

    public void ChangeDeckCardShow()
    {
        Dictionary<string, object> deckCardD = JsonConvert.DeserializeObject<Dictionary<string, object>>(deckCard);
        JArray monster = (JArray)deckCardD["monster"];
        JArray item= (JArray)deckCardD["item"];

        foreach(var m in monster)
        {
            monsterCardInDeck.Clear();
            monsterCardInDeck.Add((string)m);
        }
        foreach(var m in item)
        {
            itemCardInDeck.Clear();
            itemCardInDeck.Add((string)m);
        }

        //卡组图像
        JArray cardInDeck = monsterOrItemInDeck ? monster : item;
        for (int i = 0; i < deckCardNumber; i++)
        {
            string cardId = (string)cardInDeck[i];
            ChangeACardInDeck(i, cardId);
        }
    }

    public void ChangeACardInDeck(int i, string cardId)
    {
        //清除原卡组
        for (int j = 0; j < CardDeckBackgroundPanel[i].transform.childCount; j++)
        {
            Destroy(CardDeckBackgroundPanel[i].transform.GetChild(j).gameObject);
        }

        if (cardId == null) return;
        if (cardId.Equals("")) return;

        Dictionary<string, string> aCardData = Database.cardMonster.Query("AllCardConfig", "and CardID='" + cardId + "'")[0];

        GameObject CardInDeckPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardInDeckPrefab");

        GameObject aCard = Instantiate(CardInDeckPrefab, CardDeckBackgroundPanel[i].transform);
        aCard.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        aCard.GetComponent<CardInDeck>().changeAllAttribute(aCardData);

        GameObject cardCanvas = aCard.transform.GetChild(0).gameObject;
        cardCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(cardInDeckX, cardInDeckY);
        cardCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(localScaleCoefficientInDeck, localScaleCoefficientInDeck, 1);
    }

    public void ChangeHeroSkillInDeck(string heroSkillId)
    {
        HeroSkill heroSkillInDeck = GameObject.Find("HeroSkillPrefab").GetComponent<HeroSkill>();
        heroSkillInDeck.ChangeHeroSkill(heroSkillId);
    }
}
