using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// µã»÷±£´æ¿¨×é
/// </summary>
public class SaveDeck : MonoBehaviour
{
    public void OnClick()
    {
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        Dictionary<string, string> deck = new Dictionary<string, string>();
        deck.Add("DeckName", deckInCollection.deckName);
        deck.Add("HeroSkillId", deckInCollection.heroSkillId);
        deck.Add("CardID1", deckInCollection.monsterCardInDeck[0]);
        deck.Add("CardID2", deckInCollection.monsterCardInDeck[1]);
        deck.Add("CardID3", deckInCollection.monsterCardInDeck[2]);
        deck.Add("CardID4", deckInCollection.monsterCardInDeck[3]);
        deck.Add("CardID5", deckInCollection.monsterCardInDeck[4]);
        deck.Add("CardID6", deckInCollection.monsterCardInDeck[5]);
        deck.Add("CardID7", deckInCollection.monsterCardInDeck[6]);
        deck.Add("CardID8", deckInCollection.monsterCardInDeck[7]);
        deck.Add("CardID9", deckInCollection.itemCardInDeck[0]);
        deck.Add("CardID10", deckInCollection.itemCardInDeck[1]);
        deck.Add("CardID11", deckInCollection.itemCardInDeck[2]);
        deck.Add("CardID12", deckInCollection.itemCardInDeck[3]);
        deck.Add("CardID13", deckInCollection.itemCardInDeck[4]);
        deck.Add("CardID14", deckInCollection.itemCardInDeck[5]);
        deck.Add("CardID15", deckInCollection.itemCardInDeck[6]);
        deck.Add("CardID16", deckInCollection.itemCardInDeck[7]);
        Database.cardMonster.Update("PlayerDeck", deck, "and DeckId=" + deckInCollection.deckId);
    }
}
