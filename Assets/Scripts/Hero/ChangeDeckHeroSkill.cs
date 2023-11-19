using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDeckHeroSkill : MonoBehaviour
{
    public string heroSkillId;

    public void OnClick()
    {
        GameObject cardDeckWindowPanel = GameObject.Find("CardDeckWindowPanel");
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        deckInCollection.heroSkillId = heroSkillId;
        cardDeckWindowPanel.GetComponent<DeckInCollection>().ChangeHeroSkillInDeck(heroSkillId);
        GameObject selectHeroSkillPrefabInstantiation = GameObject.Find("SelectHeroSkillPrefabInstantiation");
        Destroy(selectHeroSkillPrefabInstantiation);
    }
}
