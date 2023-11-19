using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点击“使用”按钮，切换卡组
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
