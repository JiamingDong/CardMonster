using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// µã»÷¡°É¾³ý¡±°´Å¥£¬É¾³ý¿¨×é
/// </summary>
public class DeleteDeckButton : MonoBehaviour
{
    public string deckId;

    public void OnClick()
    {
        Database.cardMonster.Delete("PlayerDeck", "and DeckID='" + deckId + "'");
        GameObject.Find("SwitchDeckCanvas").GetComponent<AllDeckInSwitchPage>().LoadAllDeck();

        //DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();

        //deckInCollection.deckId = deckId;
        //GameObject.Find("CardDeckCardDeckWindowPanel").GetComponent<DeckInCollection>().SwitchDeck();

        //Destroy(GameObject.Find("SwitchDeckPrefabInstantiation"));

    }
}
