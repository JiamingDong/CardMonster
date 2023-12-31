using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyPortChange : MonoBehaviour
{
    public InputField inputField;

    void LockInput(InputField input)
    {
        Dictionary<string, string> dict = new();
        dict.Add("itemvalue", input.text);
        Database.cardMonster.Update("ConfigParameter", dict, " and itemname='defalutAllyPort'");
    }

    public void Start()
    {
        inputField.onEndEdit.AddListener(delegate { LockInput(inputField); });
    }
}
