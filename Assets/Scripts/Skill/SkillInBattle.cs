using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 所有在场上的卡牌技能、英雄技能的父类
/// </summary>
public abstract class SkillInBattle : OpportunityEffect
{
    /// <summary>
    /// 技能来源和数值
    /// </summary>
    public Dictionary<string, int> sourceAndValue = new();

    /// <summary>
    /// 获取技能数值，小于0的会返回0
    /// </summary>
    /// <returns>技能数值</returns>
    public int GetSKillValue()
    {
        int value = 0;
        foreach (KeyValuePair<string, int> keyValuePair in sourceAndValue)
        {
            value += keyValuePair.Value;
        }
        return value >= 0 ? value : 0;
    }

    /// <summary>
    /// 增加技能数值，例如增加装备
    /// </summary>
    /// <param name="source">来源</param>
    /// <param name="value">数值</param>
    public int AddValue(string source, int value)
    {
        if (sourceAndValue.ContainsKey(source))
            sourceAndValue[source] += value;
        else
            sourceAndValue.Add(source, value);

        return GetSKillValue();
    }

    /// <summary>
    /// 移除某个来源的数值，如失去装备
    /// </summary>
    /// <param name="source">来源</param>
    public void RemoveValue(string source)
    {
        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue.Remove(source);
        }
    }

    /// <summary>
    /// 获得技能类型
    /// </summary>
    /// <param name="skillName">技能名</param>
    /// <returns></returns>
    public static SkillType GetSkillType(string skillName)
    {
        return skillName switch
        {
            "armor" => SkillType.Armor,
            "chance" or "magic" or "ranged" or "melee" or "heal" or "heal_all" or "heal_consume" or "heal_all_consume" or "lightning" or "lightning_all" or "damage" or "damage_all" => SkillType.BasicSkill,
            _ => SkillType.NonbasicSkill,
        };
    }

    /// <summary>
    /// 是否是基础攻击效果
    /// </summary>
    /// <param name="effectName"></param>
    /// <returns></returns>
    public bool IsBasicAttackEffect()
    {
        string name = GetType().Name;
        if (name.Equals("Chance") || name.Equals("Magic") || name.Equals("Ranged") || name.Equals("Melee"))
        {
            return true;
        }
        return false;
    }

}