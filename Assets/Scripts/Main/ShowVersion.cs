using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowVersion : MonoBehaviour
{
    public Text text;

    void Start()
    {
        List<Dictionary<string, string>> result = Database.cardMonster.Query("ConfigParameter", " and itemname='databaseVersion'");
        string databaseVersion = result[0]["itemvalue"];
        text.text = databaseVersion;
    }
}
