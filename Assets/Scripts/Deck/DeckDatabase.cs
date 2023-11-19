//using Mono.Data.Sqlite;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

///// < summary >
///// 存储所有卡组数据
///// </ summary >
//public class DeckDatabase : MonoBehaviour
//{
//    public static Dictionary<string, Deck> allDeck;
//    public List<Dictionary<string, string>> queryResult;

//    void Awake()
//    {
//        LoadAllDeck();
//    }

//    public void LoadAllDeck()
//    {
//        queryResult = Database.db.Query("PlayerDeck", "order by DeckId asc");
//        allDeck = new Dictionary<string, Deck>();
//        foreach (Dictionary<string, string> keyValuePairs in queryResult)
//        {
//            string[] monsterDeck = new string[] { keyValuePairs["CardId1"].ToString(), keyValuePairs["CardId2"].ToString(), keyValuePairs["CardId3"].ToString(), keyValuePairs["CardId4"].ToString(), keyValuePairs["CardId5"].ToString(), keyValuePairs["CardId6"].ToString(), keyValuePairs["CardId7"].ToString(), keyValuePairs["CardId8"].ToString() };
//            string[] itemDeck = new string[] { keyValuePairs["CardId9"].ToString(), keyValuePairs["CardId10"].ToString(), keyValuePairs["CardId11"].ToString(), keyValuePairs["CardId12"].ToString(), keyValuePairs["CardId13"].ToString(), keyValuePairs["CardId14"].ToString(), keyValuePairs["CardId15"].ToString(), keyValuePairs["CardId16"].ToString() };
//            Deck aDeck = new Deck(keyValuePairs["DeckId"].ToString(), keyValuePairs["DeckName"].ToString(), keyValuePairs["HeroSkillId"].ToString(), monsterDeck, itemDeck);
//            allDeck.Add(keyValuePairs["DeckId"].ToString().ToString(), aDeck);
//        }
//    }
//    public static void WritePlayerDeck()
//    {
//        string content = "DeckId,DeckName,HeroSkillId,CardId1,CardId2,CardId3,CardId4,CardId5,CardId6,CardId7,CardId8,CardId9,CardId10,CardId11,CardId12,CardId13,CardId14,CardId15,CardId16\n";
//        int index = 1;
//        foreach (Deck deckConfig in configDictionary.Values)
//        {
//            string[] deckColor = null;
//            for (int i = 0; i < 8; i++)
//            {
//                if (deckConfig.MonsterDeck[i].Equals("")) continue;
//                string[] colorArray = CardDatabase.cardConfigDictionary[deckConfig.MonsterDeck[i]].Color.Split('|');
//                if (colorArray.Length == 2)
//                {
//                    if (!colorArray[0].Equals(colorArray[1]))
//                    {
//                        deckColor = colorArray;
//                        break;
//                    }
//                }
//            }
//            if (deckColor != null)
//            {
//                for (int i = 0; i < 8; i++)
//                {
//                    string[] cardColorArray = CardDatabase.cardConfigDictionary[deckConfig.MonsterDeck[i]].Color.Split('|');
//                    foreach (string cardColor in cardColorArray)
//                    {
//                        if (!cardColor.Equals(deckColor[0]) && !cardColor.Equals(deckColor[1])) goto end;
//                    }

//                }
//            }
//            content += index.ToString() + "," + deckConfig.DeckName.Replace(",", " ") + "," + deckConfig.HeroSkillId + "," + deckConfig.MonsterDeck[0] + "," + deckConfig.MonsterDeck[1] + "," +
//                deckConfig.MonsterDeck[2] + "," + deckConfig.MonsterDeck[3] + "," + deckConfig.MonsterDeck[4] + "," + deckConfig.MonsterDeck[5] + "," + deckConfig.MonsterDeck[6] + "," +
//                deckConfig.MonsterDeck[7] + "," + deckConfig.ItemDeck[0] + "," + deckConfig.ItemDeck[1] + "," + deckConfig.ItemDeck[2] + "," + deckConfig.ItemDeck[3] + "," +
//                deckConfig.ItemDeck[4] + "," + deckConfig.ItemDeck[5] + "," + deckConfig.ItemDeck[6] + "," + deckConfig.ItemDeck[7] + "\n";
//            index++;
//        end:;
//        }
//        File.WriteAllText(Path.Combine(Application.persistentDataPath, "PlayerDeck.csv"), content);
//    }
//}
