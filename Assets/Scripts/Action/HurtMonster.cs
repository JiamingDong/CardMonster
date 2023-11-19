//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 对怪兽造成伤害
///// </summary>
//public class HurtMonster : MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        GameAction gameAction = GameObject.Find("BattleProcessSystem").GetComponent<GameAction>();
//        Debug.Log("HurtMonster.ExecuteAction:对怪兽造成伤害前");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeHurtMonster", parameter));

//        GameObject monsterOfGenerateDamage = (GameObject)parameter["monsterOfGenerateDamage"];
//        GameObject monsterBeHurt = (GameObject)parameter["monsterBeHurt"];
//        string skillName = (string)parameter["skillName"];
//        int damageValue = (int)parameter["damageValue"];
//        DamageType damageType = (DamageType)parameter["damageType"];

//        damageValue = damageValue > 0 ? damageValue : 0;
//        MonsterInBattle monsterInBattle = monsterOfGenerateDamage.GetComponent<MonsterInBattle>();
//        if (monsterInBattle.currentArmorNumber > 0)
//        {
//            monsterInBattle.currentArmorNumber = monsterInBattle.currentArmorNumber - damageValue;
//            monsterInBattle.ArmorText.text = monsterInBattle.currentArmorNumber.ToString();
//            //护甲被打破
//            if (monsterInBattle.currentArmorNumber < 1)
//            {
//                Dictionary<string, object> destroyEquipmentParameter = new Dictionary<string, object>();
//                destroyEquipmentParameter.Add("monsterBeDestroy", monsterBeHurt);
//                yield return StartCoroutine(gameAction.DestroyEquipment(destroyEquipmentParameter));
//            }

//            goto end;
//        }

//        monsterInBattle.currentHp = monsterInBattle.currentHp - damageValue;
//        monsterInBattle.LifeText.text = monsterInBattle.currentHp.ToString();
//        if (monsterInBattle.currentHp < 1)
//        {
//            Dictionary<string, object> destroyMonsterParameter = new Dictionary<string, object>();
//            destroyMonsterParameter.Add("monsterBeDestroy", monsterBeHurt);
//            yield return StartCoroutine(gameAction.DestroyMonster(destroyMonsterParameter));
//        }

//        end:;

//        yield return null;
//        Debug.Log("HurtMonster.ExecuteAction:对怪兽造成伤害后");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterHurtMonster", parameter));
//    }
//}