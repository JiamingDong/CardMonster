//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// �����ƶ�
///// </summary>
//public class MonsterInBattleMove : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("MonsterInBattleMove.ExecuteAction:�����ƶ�ǰ");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeMonsterInBattleMove", parameter));

//        //�ƶ��Ĺ���
//        GameObject monsterMove = (GameObject)parameter["monsterMove"];
//        //�ƶ����λ��
//        int targetPosition = (int)parameter["targetPosition"];
//        //�ĸ�����ƶ�
//        PlayerData player = (PlayerData)parameter["player"];

//        if (player.monsterInBattleArray[targetPosition] == null)
//        {
//            //���������ƶ�
//            for(int i = 0; i < 3; i++)
//            {
//                if (player.monsterInBattleArray[i] == monsterMove)
//                {
//                    player.monsterInBattleArray[i] = null;
//                    break;
//                }
//            }
//            player.monsterInBattleArray[targetPosition] = monsterMove;
//            //�����ϵ��ƶ�
//            monsterMove.transform.parent = player.monsterInBattlePanel[targetPosition].transform;
//        }

//        yield return null;
//        Debug.Log("MonsterInBattleMove.ExecuteAction:�����ƶ���");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterMonsterInBattleMove", parameter));
//    }
//}
