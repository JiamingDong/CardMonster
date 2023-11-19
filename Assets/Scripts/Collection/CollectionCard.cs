using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;
using System.IO;
using Mono.Data.Sqlite;

/// <summary>
/// 加载收藏界面所有的卡
/// </summary>
public class CollectionCard : MonoBehaviour
{
    private static int collectionCardInRow = 6;
    private static readonly float cardX = 728;
    private static float cardY = 1016;
    private static float localScaleCoefficient = 0.3f;//卡牌缩放系数
    private static float cardClearanceDistance = 50;//卡牌间距
    private static float cardAddClearanceDistanceX = cardClearanceDistance + cardX * localScaleCoefficient;
    private static float cardAddClearanceDistanceY = cardClearanceDistance + cardY * localScaleCoefficient;
    private static float cardCoordinatesOriginX = cardAddClearanceDistanceX / 2;
    private static float cardCoordinatesOriginY = cardAddClearanceDistanceY / 2;

    /// <summary>
    /// 展示在收藏中的卡牌数据
    /// </summary>
    public List<Dictionary<string, string>> cardData;
    /// <summary>
    /// 当前所在页数
    /// </summary>
    public int pageNumber = 0;

    void Start()
    {
        cardData = Database.cardMonster.Query("AllCardConfig", "and CardType='monster' and CardFlags not like '%\"1\"%' order by CardKind asc,CardCost asc");

        ChangeCollectionCardShow();
    }

    public void ChangeCollectionCardShow()
    {
        GameObject cardCollectionBackgroundPanel = GameObject.Find("CardCollectionBackgroundPanel");
        //清除原来的卡牌图像
        for (int i = 0; i < cardCollectionBackgroundPanel.transform.childCount; i++)
        {
            Destroy(cardCollectionBackgroundPanel.transform.GetChild(i).gameObject);
        }

        for (int j = 0; j < 12; j++)
        {
            int index = 12 * pageNumber + j;
            if (cardData.Count <= index)
            {
                break;
            }

            Dictionary<string, string> aCardDate = cardData[index];

            string cardId = aCardDate["CardID"];

            int indexX = j % collectionCardInRow;
            int indexY = j / collectionCardInRow;
            
            GameObject cardForShowPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardForShowPrefab");
            GameObject aCard = Instantiate(cardForShowPrefab, cardCollectionBackgroundPanel.transform);
            aCard.GetComponent<Transform>().localPosition = new Vector3((float)(cardCoordinatesOriginX + cardAddClearanceDistanceX * indexX), (float)(-cardCoordinatesOriginY - cardAddClearanceDistanceY * indexY), 0);
            aCard.GetComponent<CardForShow>().SetAllAttribute(aCardDate);
            aCard.AddComponent<CardInColectionClick>().cardId = cardId;
            aCard.AddComponent<Button>().onClick.AddListener(aCard.GetComponent<CardInColectionClick>().OnClick);

            GameObject cardCanvas = aCard.transform.GetChild(0).gameObject;
            RectTransform rectTransform = cardCanvas.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(cardX, cardY);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = new Vector3(localScaleCoefficient, localScaleCoefficient, 1);
        }
    }

    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="n"></param>
    public void ChangePageNumber(int n)
    {
        int maxNum = (cardData.Count - 1) / 12;
        
        if(n==0 || n == maxNum)
        {
            return;
        }

        if (n + pageNumber < 0)
            pageNumber = 0;
        else if (n + pageNumber > maxNum)
            pageNumber = maxNum;
        else
            pageNumber += n;
        ChangeCollectionCardShow();
    }

    public void ChangeCardData(List<Dictionary<string,string>> cardList)
    {
        cardData = cardList;
        pageNumber = 0;
        ChangeCollectionCardShow();
    }
}