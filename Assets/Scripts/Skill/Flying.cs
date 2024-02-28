using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飞翔
/// 被不具有<飞翔>的敌人攻击时，50%%几率回避<近战><横扫>，25%%几率回避<远程>
/// </summary>
public class Flying : SkillInBattle
{
    /// <summary>
    /// 闪避远程/近战/横扫
    /// </summary>
    [TriggerEffect(@"^Replace\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
    }

    /// <summary>
    /// 判断是否是本怪兽，对方是否有飞翔，造成伤害的技能是远程/近战/横扫
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if(skillInBattle.gameObject.TryGetComponent(out Flying _))
        {
            return false;
        }

        if (monsterBeHurt == gameObject)
        {
            if (skillInBattle is Melee || skillInBattle is Thrash)
            {
                int r = RandomUtils.GetRandomNumber(1, 2);
                return r <= 1;
            }

            if (skillInBattle is Ranged)
            {
                int r = RandomUtils.GetRandomNumber(1, 4);
                return r <= 1;
            }
        }
        return false;
    }
}