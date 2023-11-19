using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ʹ�á���ť���л�����
/// </summary>
public class UseDeckButton : MonoBehaviour
{
    public string deckId;

    public void OnClick()
    {
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();

        deckInCollection.deckId = deckId;
        GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>().SwitchDeck();

        Destroy(GameObject.Find("SwitchDeckPrefabInstantiation"));
        
    }
}
