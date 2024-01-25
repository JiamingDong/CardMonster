using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitSacrificeInterface : MonoBehaviour
{
    public Canvas[] mdCanvas;
    public Canvas[] idCanvas;
    public Canvas[] mbCanvas;

    void Start()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        PlayerData playerData = battleProcess.allyPlayerData;

        Dictionary<string, string>[] handMonster = playerData.handMonster;
        for (int i = 0; i < handMonster.Length; i++)
        {
            if (handMonster[i] == null)
            {
                mdCanvas[i].transform.Find("ButtonBackgroundImage").GetComponent<Image>().color = Color.grey;
            }
            else
            {
                SacrificeButton sacrificeButton = mdCanvas[i].gameObject.AddComponent<SacrificeButton>();
                sacrificeButton.cardIndex = i;

                mdCanvas[i].GetComponent<Button>().onClick.AddListener(sacrificeButton.OnClick);
            }
        }

        Dictionary<string, string>[] handItem = playerData.handItem;
        for (int i = 0; i < handItem.Length; i++)
        {
            if (handItem[i] == null)
            {
                idCanvas[i].transform.Find("ButtonBackgroundImage").GetComponent<Image>().color = Color.grey;
            }
            else
            {
                SacrificeButton sacrificeButton = idCanvas[i].gameObject.AddComponent<SacrificeButton>();
                sacrificeButton.cardIndex = i + 2;

                idCanvas[i].GetComponent<Button>().onClick.AddListener(sacrificeButton.OnClick);
            }
        }

        GameObject[] monsterGameObjectArray = playerData.monsterGameObjectArray;
        for (int i = 0; i < monsterGameObjectArray.Length; i++)
        {
            if (monsterGameObjectArray[i] == null)
            {
                mbCanvas[i].transform.Find("ButtonBackgroundImage").GetComponent<Image>().color = Color.grey;
            }
            else
            {
                SacrificeButton sacrificeButton = mbCanvas[i].gameObject.AddComponent<SacrificeButton>();
                sacrificeButton.cardIndex = i + 4;

                mbCanvas[i].GetComponent<Button>().onClick.AddListener(sacrificeButton.OnClick);
            }
        }
    }

}
