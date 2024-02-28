using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 破甲
/// 以“近战”、“远程”、“侵袭”、“群体侵袭”对有护甲的敌人造成伤害时，此伤害+%d。<远程>攻击不会被护甲回避
/// </summary>
public class Breaker : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        parameter["DamageValue"] = damageValue + GetSkillValue();

        yield break;
    }

    /// <summary>
    /// 此卡以“近战”、“远程”、“侵袭”、“群体侵袭”对有护甲的敌人造成伤害
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        return skillInBattle.gameObject == gameObject && monsterBeHurt.TryGetComponent(out Armor _) && (skillInBattle is Melee || skillInBattle is Ranged || skillInBattle is Damage || skillInBattle is DamageAll);
    }

    [TriggerEffect(@"^Replace\.Armor\.Effect1$", "Compare1")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
    }

    /// <summary>
    /// 判断是否是本怪兽，造成伤害的技能是远程
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        if (skillInBattle.gameObject == gameObject && skillInBattle is Ranged)
        {
            return true;
        }
        return false;
    }
}
