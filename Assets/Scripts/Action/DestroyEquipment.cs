//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 消灭怪兽上的装备
///// </summary>
//public class DestroyEquipment : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("DestroyEquipment.ExecuteAction:消灭怪兽上的装备前");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeDestroyEquipment", parameter));

//        GameObject monsterBeDestroyEquipment = (GameObject)parameter["monsterBeDestroyEquipment"];

//        MonsterInBattle monsterInBattle = monsterBeDestroyEquipment.GetComponent<MonsterInBattle>();
//        monsterInBattle.equipment = null;
//        monsterInBattle.EquipmentBackgroundLImage.enabled = false;
//        monsterInBattle.EquipmentBackgroundLImage.texture = null;
//        monsterInBattle.EquipmentBackgroundRImage.enabled = false;
//        monsterInBattle.EquipmentBackgroundRImage.texture = null;
//        monsterInBattle.EquipmentImage.enabled = false;
//        monsterInBattle.EquipmentImage.texture = null;
//        monsterInBattle.ArmorImage.enabled = false;
//        monsterInBattle.ArmorText.enabled = false;
//        monsterInBattle.ArmorText.text = null;

//        yield return null;
//        Debug.Log("DestroyEquipment.ExecuteAction:消灭怪兽上的装备后");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterDestroyEquipment", parameter));
//    }
//}
