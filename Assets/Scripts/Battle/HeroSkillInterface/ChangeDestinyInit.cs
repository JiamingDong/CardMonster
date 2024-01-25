using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 逆天改命，初始化界面
/// </summary>
public class ChangeDestinyInit : MonoBehaviour
{
    public Canvas[] mbCanvas;

    void Start()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        PlayerData playerData = battleProcess.allyPlayerData;

        GameObject[] monsterGameObjectArray = playerData.monsterGameObjectArray;
        for (int i = 0; i < monsterGameObjectArray.Length; i++)
        {
            if (monsterGameObjectArray[i] == null)
            {
                mbCanvas[i].transform.Find("ButtonBackgroundImage").GetComponent<Image>().color = Color.grey;
            }
            else
            {
                ChangeDestinyLaunch changeDestinyLaunch = mbCanvas[i].gameObject.AddComponent<ChangeDestinyLaunch>();
                changeDestinyLaunch.cardIndex = i;

                mbCanvas[i].GetComponent<Button>().onClick.AddListener(changeDestinyLaunch.OnClick);
            }
        }
    }
}
