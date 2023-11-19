using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ղ���Ŀ��Ƶĵ���¼�
/// </summary>
public class CardInColectionClick : MonoBehaviour
{
    public string cardId;
    //public bool monsterOrItemInDeck;//���޻��ǵ��ߣ�true���ޣ�false����

    /// <summary>
    /// ������
    /// </summary>
    public void OnClick()
    {
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        bool monsterOrItemInDeck = gameObject.GetComponent<CardForShow>().type.Equals("monster") ? true : false;
        //�ǹ��޿�
        if (monsterOrItemInDeck)
        {
            string theCardColor = Database.cardMonster.Query("AllCardConfig", "and CardId='" + cardId + "'")[0]["CardKind"];
            string[] theCardColorArray = theCardColor.Substring(1, theCardColor.Length - 2).Split('|');
            //�����λ��
            int insertIndex = -1;
            //����Ŀ���˫ɫ
            if (theCardColorArray.Length == 2 && !theCardColorArray[0].Equals(theCardColorArray[1]))
            {
                for (int i = 0; i < 8; i++)
                {
                    //���ڿ��������
                    if (cardId.Equals(deckInCollection.monsterCardInDeck[i])) {
                        goto end;
                    }
                    //���λ��û��
                    if (deckInCollection.monsterCardInDeck[i].Equals(""))
                    {
                        if (insertIndex == -1) insertIndex = i;
                        continue;
                    }
                    string cardInDeckColor = Database.cardMonster.Query("AllCardConfig", "and CardId='" + deckInCollection.monsterCardInDeck[i] + "'")[0]["CardKind"];
                    string[] cardInDeckColorArray = cardInDeckColor.Substring(1, cardInDeckColor.Length - 2).Split('|');
                    //������λ�õĿ�������һ����ɫ���ڵ���Ŀ�����ɫ����ͽ���
                    foreach (string color in cardInDeckColorArray)
                    {
                        if (!color.Equals(theCardColorArray[0]) && !color.Equals(theCardColorArray[1])) goto end;
                    }
                }
            }
            //����Ŀ�����˫ɫ��
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    //Debug.Log("CardInColectionClick:����");

                    //���ڿ��������
                    if (cardId.Equals(deckInCollection.monsterCardInDeck[i]))
                    {
                        goto end;
                    }

                    //���λ��û��
                    if (deckInCollection.monsterCardInDeck[i].Equals(""))
                    {
                        if (insertIndex == -1) insertIndex = i;
                        continue;
                    }
                    string cardInDeckColor = Database.cardMonster.Query("AllCardConfig", "and CardID='" + deckInCollection.monsterCardInDeck[i] + "'")[0]["CardKind"];

                    string[] cardInDeckColorArray = cardInDeckColor.Substring(1, cardInDeckColor.Length - 2).Split('|');
                    //������λ�õĿ���˫ɫ��������Ŀ�����ɫ�������棬�ͽ���
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
        //���ǹ��޿�
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
