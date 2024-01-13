using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 防空
/// <近战><远程>攻击具有<飞翔>的敌人时，此伤害+%d且无法被<飞翔>回避
/// </summary>
public class Antiair : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        parameter["DamageValue"] = damageValue + GetSkillValue();

        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断近战、远程，具有飞翔
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];
        return (launchedSkill is Melee || launchedSkill is Ranged) && launchedSkill.gameObject == gameObject && monsterBeHurt.TryGetComponent<Flying>(out _);
    }

    [TriggerEffect(@"^Replace\.Flying\.Effect1$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断触发对方飞翔的伤害的技能的发动者是否是本怪兽
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        //造成触发飞翔的伤害的技能
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if ((skillInBattle is Melee || skillInBattle is Ranged) && skillInBattle.gameObject == gameObject)
        {
            return true;
        }

        return false;
    }
}