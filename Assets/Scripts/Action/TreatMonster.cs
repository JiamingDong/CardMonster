//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////÷Œ¡∆π÷ ﬁ
//public class TreatMonster : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("TreatMonster.ExecuteAction:÷Œ¡∆π÷ ﬁ«∞");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeTreatMonster", parameter));

//        GameObject monsterOfGenerateTreatment = (GameObject)parameter["monsterOfGenerateTreatment"];
//        GameObject monsterBeTreat = (GameObject)parameter["monsterBeTreat"];
//        string skillName = (string)parameter["skillName"];
//        int treatValue = (int)parameter["treatValue"];

//        treatValue = treatValue > 0 ? treatValue : 0;

//        MonsterInBattle monsterInBattle = monsterBeTreat.GetComponent<MonsterInBattle>();
//        monsterInBattle.currentHp = monsterInBattle.currentHp + treatValue > monsterInBattle.maximumHp ? monsterInBattle.maximumHp : monsterInBattle.currentHp + treatValue;

//        yield return null;
//        Debug.Log("TreatMonster.ExecuteAction:÷Œ¡∆π÷ ﬁ∫Û");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterTreatMonster", parameter));
//    }
//}
