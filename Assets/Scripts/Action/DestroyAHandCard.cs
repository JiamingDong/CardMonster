//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// ����һ������
///// handPanelNumber ����λ�õ���ţ�12���ޣ�34����
///// </summary>
//public class DestroyAHandCard : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("DestroyAHandCard.ExecuteAction:����һ������ǰ");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeDestroyAHandCard", new Dictionary<string, object>()));

//        int handPanelNumber = (int)parameter["handPanelNumber"];

//        Player player = (Player)parameter["player"];

//        foreach (PlayerData playerMessage in battleProcess.systemPlayerData)
//        {
//            if (playerMessage.player == player)
//            {
//                switch (handPanelNumber)
//                {
//                    case 0:
//                        playerMessage.handMonster[0] = null;
//                        if (playerMessage.handMonsterPanel != null) Destroy(playerMessage.handMonsterPanel[0].GetComponent<Transform>().Find("CardForShow").gameObject);
//                        break;
//                    case 1:
//                        playerMessage.handMonster[1] = null;
//                        if (playerMessage.handMonsterPanel != null) Destroy(playerMessage.handMonsterPanel[1].GetComponent<Transform>().Find("CardForShow").gameObject);
//                        break;
//                    case 2:
//                        playerMessage.handItem[0] = null;
//                        if (playerMessage.handItemPanel != null) Destroy(playerMessage.handItemPanel[0].GetComponent<Transform>().Find("CardForShow").gameObject);
//                        break;
//                    case 3:
//                        playerMessage.handItem[1] = null;
//                        if (playerMessage.handItemPanel != null) Destroy(playerMessage.handItemPanel[1].GetComponent<Transform>().Find("CardForShow").gameObject);
//                        break;
//                }
//                break;
//            }
//        }

//        yield return null;
//        Debug.Log("DestroyAHandCard.ExecuteAction:����һ�����ƺ�");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterDestroyAHandCard", new Dictionary<string, object>()));
//    }
//}
