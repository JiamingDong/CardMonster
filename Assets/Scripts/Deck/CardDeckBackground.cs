using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ŀ��Ʊ�������¼�
/// </summary>
public class CardDeckBackground : MonoBehaviour
{
    public int CardDeckBackgroundIndex;

    /// <summary>
    /// ������
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
