using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �������������ť����ӿ���
/// </summary>
public class AddDeckButton : MonoBehaviour
{
    public void OnClick()
    {
        Dictionary<string, string> newDeck = new Dictionary<string, string>();
        newDeck.Add("DeckName", "�¿���");
        Database.cardMonster.Insert("PlayerDeck", newDeck);

        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        deckInCollection.deckId = Database.cardMonster.Query("PlayerDeck", "order by DeckID desc")[0]["DeckID"];
        GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>().SwitchDeck();
    }
}
