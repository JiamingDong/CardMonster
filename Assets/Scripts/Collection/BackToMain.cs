using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMain : MonoBehaviour
{
    public void OnClick()
    {
        //Debug.Log("BackToMain");
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        Dictionary<string, string> record = new Dictionary<string, string>();
        record.Add("DefaultDeckId", deckInCollection.deckId);
        Database.cardMonster.Update("PlayerData", record, "and PlayerID='1'");
        SceneManager.LoadScene("MainScene");//ÇÐ»»³¡¾°µ½CardDeckScene
    }
}
