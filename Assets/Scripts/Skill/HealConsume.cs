using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 治疗（消耗品）
/// </summary>
public class HealConsume : SkillInBattle
{
    public HealConsume(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("AfterACardBeUsed")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();


        //发动技能的怪兽
        GameObject monsterOfLaunchingSkill = (GameObject)parameter["monsterOfLaunchingSkill"];
        //目标场上位置的怪兽
        GameObject monsterBeTreat = (GameObject)parameter["targetMonster"];
        //确定发动技能的怪兽位置，确定发动技能的怪兽所属玩家的对方玩家
        int skillValue = (int)parameter["skillValue"];

        //目标怪兽结算前死亡就返回
        if (monsterBeTreat == null) yield break;

        //治疗
        Dictionary<string, object> treatParameter = new Dictionary<string, object>();
        //造成治疗的怪兽
        treatParameter.Add("monsterOfGenerateTreatment", monsterOfLaunchingSkill);
        //受到治疗的怪兽
        treatParameter.Add("monsterBeTreat", monsterBeTreat);
        //造成治疗的技能的名字
        treatParameter.Add("skillName", "ZhiLiaoXiaoHaoPin");
        //治疗数值
        treatParameter.Add("treatValue", skillValue);

        yield return StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, treatParameter));
        yield return null;

        Debug.Log("ZhiLiaoXiaoHaoPin.Action:治疗后");
    }

}