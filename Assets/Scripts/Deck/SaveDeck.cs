using Newtonsoft.Json;
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

        Dictionary<string, string[]> deckCard = new();
        deckCard.Add("monster", deckInCollection.monsterCardInDeck);
        deckCard.Add("item", deckInCollection.itemCardInDeck);

        Dictionary<string, string> deck = new();
        deck.Add("DeckName", deckInCollection.deckName);
        deck.Add("HeroSkillId", deckInCollection.heroSkillId);
        deck.Add("DeckCard", JsonConvert.SerializeObject(deckCard));

        Database.cardMonster.Update("PlayerDeck", deck, "and DeckId=" + deckInCollection.deckId);
    }
}
