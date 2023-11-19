using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private string deckId;
    private string deckName;
    private string heroSkillId;
    private string[] monsterDeck;
    private string[] itemDeck;

    public Deck(string deckId, string deckName, string heroSkillId, string[] monsterDeck, string[] itemDeck)
    {
        this.deckId = deckId;
        this.deckName = deckName;
        this.heroSkillId = heroSkillId;
        this.monsterDeck = monsterDeck;
        this.itemDeck = itemDeck;
    }

    public string DeckId { get => deckId; set => deckId = value; }
    public string DeckName { get => deckName; set => deckName = value; }
    public string HeroSkillId { get => heroSkillId; set => heroSkillId = value; }
    public string[] MonsterDeck { get => monsterDeck; set => monsterDeck = value; }
    public string[] ItemDeck { get => itemDeck; set => itemDeck = value; }
}
