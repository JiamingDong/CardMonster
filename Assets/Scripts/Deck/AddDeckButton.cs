using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 点击“新增”按钮，添加卡组
/// </summary>
public class AddDeckButton : MonoBehaviour
{
    public void OnClick()
    {
        Dictionary<string, string[]> deckCardD = new();
        deckCardD.Add("monster", new string[] { "", "", "", "", "", "", "", "" });
        deckCardD.Add("item", new string[] { "", "", "", "", "", "", "", "" });

        Dictionary<string, string> newDeck = new();
        newDeck.Add("DeckName", "新卡组");
        newDeck.Add("HeroSkillId", "");
        newDeck.Add("DeckCard", JsonConvert.SerializeObject(deckCardD));
        Database.cardMonster.Insert("PlayerDeck", newDeck);

        string deckId = Database.cardMonster.Query("PlayerDeck", "order by DeckID desc")[0]["DeckID"];
        GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>().SwitchDeck(deckId);
    }
}
