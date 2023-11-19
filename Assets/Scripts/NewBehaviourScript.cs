using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public void OnClick()
    {
        List<Dictionary<string, string>> keyValuePairs = Database.cardMonster.Query("AllCardConfig", "");
        foreach (Dictionary<string, string> kv in keyValuePairs)
        {
            //string cardEP = kv["CardEP"];

            //if (cardEP == null || cardEP.Length == 0)
            //{
            //    continue;
            //}

            //cardEP = cardEP.Replace("\"L\"", "\"leftSkill\"").Replace("\"R\"", "\"rightSkill\"").Replace("\"N\"", "\"name\"").Replace("\"V\"", "\"value\"");
            //Debug.Log(cardEP);

            string cardFlags = kv["CardKind"];
            if (cardFlags == null || cardFlags.Length == 0)
            {
                continue;
            }
            string[] a= cardFlags.Substring(1, cardFlags.Length - 2).Split("|");
            string b = "{\"leftKind\":\""+a[0]+"\"";
            if(a.Length > 1)
            {
                b+=",\"rightKind\":\""+a[1]+"\"";
            }
            b += "}";

            Debug.Log(b);

            Dictionary<string, string> keyValuePairs2 = new Dictionary<string, string>();

            keyValuePairs2.Add("CardKind", b);

            Database.cardMonster.Update("AllCardConfig", keyValuePairs2, "and CardID='" + kv["CardID"] + "'");
        }
    }


}