using Newtonsoft.Json;
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
        bool monsterOrItemInDeck = gameObject.GetComponent<CardForShow>().type.Equals("monster");
        //�ǹ��޿�
        if (monsterOrItemInDeck)
        {
            string theCardColor = Database.cardMonster.Query("AllCardConfig", "and CardId='" + cardId + "'")[0]["CardKind"];
            Dictionary<string, string> cardKind = JsonConvert.DeserializeObject<Dictionary<string, string>>(theCardColor);
            //�����λ��
            int insertIndex = -1;
            //����Ŀ���˫ɫ
            if (cardKind.ContainsKey("rightKind") && !cardKind["leftKind"].Equals(cardKind["rightKind"]))
            {
                Debug.Log(1);
                string leftKind = cardKind["leftKind"];
                string rightKind = cardKind["rightKind"];

                for (int i = 0; i < deckInCollection.monsterCardInDeck.Length; i++)
                {
                    //���ڿ��������
                    if (cardId.Equals(deckInCollection.monsterCardInDeck[i]))
                    {
                        Debug.Log(2);
                        goto end;
                    }

                    //���λ��û��
                    if (deckInCollection.monsterCardInDeck[i].Equals(""))
                    {
                        Debug.Log(3);
                        if (insertIndex == -1)
                        {
                            insertIndex = i;
                        }

                        continue;
                    }

                    string cardInDeckColor = Database.cardMonster.Query("AllCardConfig", "and CardId='" + deckInCollection.monsterCardInDeck[i] + "'")[0]["CardKind"];

                    Dictionary<string, string> cardKind2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardInDeckColor);

                    //������λ�õĿ�������һ����ɫ���ڵ���Ŀ�����ɫ����ͽ���
                    foreach (KeyValuePair<string, string> keyValuePair in cardKind2)
                    {
                        Debug.Log(keyValuePair + "----" + leftKind + "----" + rightKind);
                        if (!keyValuePair.Value.Equals(leftKind) && !keyValuePair.Value.Equals(rightKind))
                        {
                            goto end;
                        }
                    }
                }
            }
            //����Ŀ�����˫ɫ��
            else
            {
                string leftKind = cardKind["leftKind"];

                for (int i = 0; i < 8; i++)
                {
                    //���ڿ��������
                    if (cardId.Equals(deckInCollection.monsterCardInDeck[i]))
                    {
                        goto end;
                    }

                    //���λ��û��
                    if (deckInCollection.monsterCardInDeck[i].Equals(""))
                    {
                        if (insertIndex == -1)
                        {
                            insertIndex = i;
                        }
                        continue;
                    }
                    string cardInDeckColor = Database.cardMonster.Query("AllCardConfig", "and CardID='" + deckInCollection.monsterCardInDeck[i] + "'")[0]["CardKind"];

                    Dictionary<string, string> cardKind2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardInDeckColor);

                    //������λ�õĿ���˫ɫ��������Ŀ�����ɫ�������棬�ͽ���
                    if (cardKind2.ContainsKey("rightKind") && !cardKind2["leftKind"].Equals(cardKind2["rightKind"]))
                    {
                        if (!leftKind.Equals(cardKind2["leftKind"]) && !leftKind.Equals(cardKind2["rightKind"]))
                        {
                            goto end;
                        }
                    }

                }
            }

            Debug.Log(insertIndex);
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
