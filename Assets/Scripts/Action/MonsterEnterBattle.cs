//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// ���޽���ս��
///// </summary>
//public class MonsterEnterBattle : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("MonsterEnterBattle.ExecuteAction:���޽���ս��ǰ");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeMonsterEnterBattle", new Dictionary<string, object>()));

//        Player player = (Player)parameter["player"];
//        int battlePanelNumber = (int)parameter["battlePanelNumber"];
//        Dictionary<string,string> card = (Dictionary<string, string>)parameter["cardAttribute"];

//        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
//        {
//            PlayerData playerMessage = battleProcess.systemPlayerData[i];
//            if (playerMessage.player == player)
//            {
//                GameObject cardInBattlePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardInBattlePrefab");
//                GameObject battlePanel = playerMessage.monsterInBattlePanel[battlePanelNumber];
//                GameObject cardInBattle = Instantiate(cardInBattlePrefab, battlePanel.transform);
//                cardInBattle.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
//                cardInBattle.GetComponent<MonsterInBattle>().GenerateMonster(card);

//                GameObject cardCanvas = cardInBattle.transform.GetChild(0).gameObject;
//                cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 1);

//                playerMessage.monsterInBattleArray[battlePanelNumber] = cardInBattle;

//                break;
//            }
//        }

//        Debug.Log("MonsterEnterBattle��" + player + "�Ĺ��ޡ�" + card["CardName"] + "������" + battlePanelNumber + "��λ");

//        yield return null;
//        Debug.Log("MonsterEnterBattle.ExecuteAction:���޽���ս����");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterMonsterEnterBattle", new Dictionary<string, object>()));
//    }
//}
