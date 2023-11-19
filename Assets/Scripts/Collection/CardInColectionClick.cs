using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 收藏里的卡牌的点击事件
/// </summary>
public class CardInColectionClick : MonoBehaviour
{
    public string cardId;
    //public bool monsterOrItemInDeck;//怪兽还是道具，true怪兽，false道具

    /// <summary>
    /// 上阵卡牌
    /// </summary>
    public void OnClick()
    {
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        bool monsterOrItemInDeck = gameObject.GetComponent<CardForShow>().type.Equals("monster") ? true : false;
        //是怪兽卡
        if (monsterOrItemInDeck)
        {
            string theCardColor = Database.cardMonster.Query("AllCardConfig", "and CardId='" + cardId + "'")[0]["CardKind"];
            string[] theCardColorArray = theCardColor.Substring(1, theCardColor.Length - 2).Split('|');
            //插入的位置
            int insertIndex = -1;
            //点击的卡是双色
            if (theCardColorArray.Length == 2 && !theCardColorArray[0].Equals(theCardColorArray[1]))
            {
                for (int i = 0; i < 8; i++)
                {
                    //已在卡组则结束
                    if (cardId.Equals(deckInCollection.monsterCardInDeck[i])) {
                        goto end;
                    }
                    //这个位置没卡
                    if (deckInCollection.monsterCardInDeck[i].Equals(""))
                    {
                        if (insertIndex == -1) insertIndex = i;
                        continue;
                    }
                    string cardInDeckColor = Database.cardMonster.Query("AllCardConfig", "and CardId='" + deckInCollection.monsterCardInDeck[i] + "'")[0]["CardKind"];
                    string[] cardInDeckColorArray = cardInDeckColor.Substring(1, cardInDeckColor.Length - 2).Split('|');
                    //如果这个位置的卡的其中一个颜色不在点击的卡的颜色里面就结束
                    foreach (string color in cardInDeckColorArray)
                    {
                        if (!color.Equals(theCardColorArray[0]) && !color.Equals(theCardColorArray[1])) goto end;
                    }
                }
            }
            //点击的卡不是双色卡
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    //Debug.Log("CardInColectionClick:闪电");

                    //已在卡组则结束
                    if (cardId.Equals(deckInCollection.monsterCardInDeck[i]))
                    {
                        goto end;
                    }

                    //这个位置没卡
                    if (deckInCollection.monsterCardInDeck[i].Equals(""))
                    {
                        if (insertIndex == -1) insertIndex = i;
                        continue;
                    }
                    string cardInDeckColor = Database.cardMonster.Query("AllCardConfig", "and CardID='" + deckInCollection.monsterCardInDeck[i] + "'")[0]["CardKind"];

                    string[] cardInDeckColorArray = cardInDeckColor.Substring(1, cardInDeckColor.Length - 2).Split('|');
                    //如果这个位置的卡是双色卡，点击的卡的颜色不在里面，就结束
                    if (cardInDeckColorArray.Length == 2 && !cardInDeckColorArray[0].Equals(cardInDeckColorArray[1]))
                    {
                        if (!theCardColorArray[0].Equals(cardInDeckColorArray[0]) && !theCardColorArray[0].Equals(cardInDeckColorArray[1])) goto end;
                    }

                }
            }

            if (insertIndex != -1)
            {
                deckInCollection.monsterCardInDeck[insertIndex] = cardId;
                GameObject.Find("DeckToMonsterButtonImage").GetComponent<Button>().onClick.Invoke();
            }
        }
        //不是怪兽卡
        else
        {
            int insertIndex = -1;
            for (int i = 0; i < 8; i++)
            {
                if (deckInCollection.itemCardInDeck[i].Equals(cardId))
                {
                    goto end;
                }
                if (deckInCollection.itemCardInDeck[i].Equals("") && insertIndex == -1)
                {
                    insertIndex = i;
                    continue;
                }
            }
            if (insertIndex != -1)
            {
                deckInCollection.itemCardInDeck[insertIndex] = cardId;
                GameObject.Find("DeckToItemButtonImage").GetComponent<Button>().onClick.Invoke();
            }
        }
    end:;
    }
}
