//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 消灭怪兽
///// </summary>
//public class DestroyMonster : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("DestroyMonster.ExecuteAction:消灭怪兽前");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeDestroyMonster", parameter));

//        GameObject monsterBeDestroy = (GameObject)parameter["monsterBeDestroy"];
//        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
//        {
//            for (int j = 2; j > -1; j++)
//            {
//                if (battleProcess.systemPlayerData[i].monsterInBattleArray[j] == monsterBeDestroy)
//                {
//                    battleProcess.systemPlayerData[i].monsterInBattleArray[j] = null;
//                    Destroy(monsterBeDestroy);
//                    goto end;
//                }
//            }
//        }
//    end:;

//        yield return null;
//        Debug.Log("DestroyMonster.ExecuteAction:消灭怪兽后");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterDestroyMonster", parameter));
//    }
//}
