//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// ��ˮ��
///// crystalAmount ����
///// player ��ˮ�������
///// </summary>
//public class AddCrystal : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("AddCrystal.ExecuteAction:��ˮ��ǰ");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeAddCrystal", new Dictionary<string, object>()));

//        int crystalAmount = (int)parameter["crystalAmount"];
//        Player player=(Player)parameter["player"];

//        foreach(PlayerData playerMessage in battleProcess.systemPlayerData)
//        {
//            if (playerMessage.player == player)
//            {
//                playerMessage.surplusCrystal = (playerMessage.surplusCrystal + crystalAmount) > 0 ? playerMessage.surplusCrystal + crystalAmount : 0;
                
//                if(playerMessage.surplusCrystalText!=null) playerMessage.surplusCrystalText.text = playerMessage.surplusCrystal.ToString();

//                break;
//            }
//        }

//        Debug.Log("AddCrystal��"+ player + "��" + crystalAmount + "ˮ��");

//        yield return null;
//        Debug.Log("AddCrystal.ExecuteAction:��ˮ����");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterAddCrystal", new Dictionary<string, object>()));
//    }
//}
