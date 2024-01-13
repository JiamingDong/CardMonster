using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加载切换卡组界面所有的卡组
/// </summary>
public class AllDeckInSwitchPage : MonoBehaviour
{
    void Start()
    {
        LoadAllDeck();
    }

    public void LoadAllDeck()
    {
        //Debug.Log("AllDeckInSwitchPage.LoadAllDeck:进入");
        GameObject deckItemBackgroundPanel = GameObject.Find("DeckItemBackgroundPanel");
        for (int i = 0; i < deckItemBackgroundPanel.transform.childCount; i++)
        {
            Destroy(deckItemBackgroundPanel.transform.GetChild(i).gameObject);
        }

        List<Dictionary<string, string>> allDeck = Database.cardMonster.Query("PlayerDeck", "order by DeckID asc");
        deckItemBackgroundPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(650, 110 * allDeck.Count);

        int index = 0;
        for (int i = 0; i < allDeck.Count; i++)
        {
            GameObject deckNameSelectPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("DeckNameSelectPrefab");
            GameObject aDeckNameSelectPrefab = Instantiate(deckNameSelectPrefab, deckItemBackgroundPanel.transform);
            aDeckNameSelectPrefab.name = "DeckNameSelectPrefab" + (index + 1).ToString();
            aDeckNameSelectPrefab.GetComponent<Transform>().localPosition = new Vector3(0, -5 - 110 * index);
            aDeckNameSelectPrefab.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);

            GameObject aDeckNameSelectCanvas = aDeckNameSelectPrefab.transform.GetChild(0).gameObject;
            aDeckNameSelectCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(650, 100);
            aDeckNameSelectCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            aDeckNameSelectCanvas.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

            GameObject deckNameText = aDeckNameSelectCanvas.transform.Find("DeckNameText").gameObject;
            deckNameText.GetComponent<Text>().text = allDeck[i]["DeckName"];

            GameObject deckNameButtonPrefab = aDeckNameSelectCanvas.transform.Find("DeckNameButtonPrefab").gameObject;
            deckNameButtonPrefab.transform.localPosition = new Vector3(575, -50, 0);
            GameObject deckNameButtonCanvas = deckNameButtonPrefab.transform.Find("DeckNameButtonCanvas").gameObject;
            deckNameButtonCanvas.GetComponent<UseDeckButton>().deckId = allDeck[i]["DeckID"];

            GameObject deleteDeckButtonPrefab = aDeckNameSelectCanvas.transform.Find("DeleteDeckButtonPrefab").gameObject;
            deleteDeckButtonPrefab.transform.localPosition = new Vector3(450, -50, 0);
            GameObject deleteDeckButtonCanvas = deleteDeckButtonPrefab.transform.Find("DeleteDeckButtonCanvas").gameObject;
            deleteDeckButtonCanvas.GetComponent<DeleteDeckButton>().deckId = allDeck[i]["DeckID"];

            index++;
        }
    }
}
