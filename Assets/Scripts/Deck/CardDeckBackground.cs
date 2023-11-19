using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡组里的卡牌背景点击事件
/// </summary>
public class CardDeckBackground : MonoBehaviour
{
    public int CardDeckBackgroundIndex;

    /// <summary>
    /// 下阵卡牌
    /// </summary>
    public void OnClick()
    {
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        if (deckInCollection.monsterOrItemInDeck)
        {
            deckInCollection.monsterCardInDeck[CardDeckBackgroundIndex] = "";
        }
        else
        {
            deckInCollection.itemCardInDeck[CardDeckBackgroundIndex] = "";
        }
        for (int i = 0; i < deckInCollection.CardDeckBackgroundPanel[CardDeckBackgroundIndex].transform.childCount; i++)
        {
            Destroy(deckInCollection.CardDeckBackgroundPanel[CardDeckBackgroundIndex].transform.GetChild(i).gameObject);
        }
    }
}
