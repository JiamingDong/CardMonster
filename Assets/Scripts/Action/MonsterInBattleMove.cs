//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 怪兽移动
///// </summary>
//public class MonsterInBattleMove : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("MonsterInBattleMove.ExecuteAction:怪兽移动前");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeMonsterInBattleMove", parameter));

//        //移动的怪兽
//        GameObject monsterMove = (GameObject)parameter["monsterMove"];
//        //移动后的位置
//        int targetPosition = (int)parameter["targetPosition"];
//        //哪个玩家移动
//        PlayerData player = (PlayerData)parameter["player"];

//        if (player.monsterInBattleArray[targetPosition] == null)
//        {
//            //先数据上移动
//            for(int i = 0; i < 3; i++)
//            {
//                if (player.monsterInBattleArray[i] == monsterMove)
//                {
//                    player.monsterInBattleArray[i] = null;
//                    break;
//                }
//            }
//            player.monsterInBattleArray[targetPosition] = monsterMove;
//            //画面上的移动
//            monsterMove.transform.parent = player.monsterInBattlePanel[targetPosition].transform;
//        }

//        yield return null;
//        Debug.Log("MonsterInBattleMove.ExecuteAction:怪兽移动后");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterMonsterInBattleMove", parameter));
//    }
//}
