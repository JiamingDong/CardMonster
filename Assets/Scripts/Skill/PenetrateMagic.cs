using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 法力贯穿
/// <魔法>无视敌人的<法术反射>和<法术护盾>。<法术溅射>无视敌人的<法术护盾>。
/// </summary>
public class PenetrateMagic : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Reflect\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Mshield\.Effect1$", "Compare2")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// 判断触发对方法术反射的伤害的技能的发动者是否是本怪兽、技能是魔法
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        //造成触发法术反射的伤害的技能
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        return skillInBattle.gameObject == gameObject && skillInBattle is Magic;
    }

    /// <summary>
    /// 判断触发对方法术护盾的伤害的技能的发动者是否是本怪兽、技能是魔法/法术溅射
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        //造成触发法术护盾的伤害的技能
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        return skillInBattle.gameObject == gameObject && (skillInBattle is Magic || skillInBattle is MagicAoe);
    }
}
