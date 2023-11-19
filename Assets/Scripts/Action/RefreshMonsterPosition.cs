//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 刷新场上怪兽的位置
///// </summary>
//public class RefreshMonsterPosition : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        GameAction gameAction = GameObject.Find("BattleProcessSystem").GetComponent<GameAction>();

//        //从1号位开始，有空位则后面补上
//        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
//        {
//            for (int j = 0; j < battleProcess.systemPlayerData[i].monsterInBattleArray.Length - 1; j++)
//            {
//                if (battleProcess.systemPlayerData[i].monsterInBattleArray[j] == null)
//                {
//                    for (int x = j + 1; j < battleProcess.systemPlayerData[i].monsterInBattleArray.Length; x++)
//                    {
//                        if (battleProcess.systemPlayerData[i].monsterInBattleArray[x] != null)
//                        {
//                            int y = x;
//                            while (y > j)
//                            {
//                                Dictionary<string, object> moveParameter = new Dictionary<string, object>();
//                                moveParameter.Add("monsterMove", battleProcess.systemPlayerData[i].monsterInBattleArray[y]);
//                                moveParameter.Add("targetPosition", y - 1);
//                                moveParameter.Add("player", battleProcess.systemPlayerData[i]);
//                                yield return StartCoroutine(gameAction.MonsterInBattleMove(moveParameter));

//                                j--;
//                            }
//                            break;
//                        }
//                    }
//                }
//            }
//        }

//        yield return null;
//    }
//}
