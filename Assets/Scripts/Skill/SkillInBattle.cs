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
    /// 获取技能数值，如果sourceAndValue为空返回-1，否则，小于0的会返回0
    /// </summary>
    /// <returns>技能数值</returns>
    public virtual int GetSkillValue()
    {
        if (sourceAndValue.Count == 0)
        {
            return -1;
        }

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
    public virtual int AddValue(string source, int value)
    {
        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] += value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    /// <summary>
    /// 移除某个来源的数值，如失去装备
    /// </summary>
    /// <param name="source">来源</param>
    public void RemoveValue(string source)
    {
        sourceAndValue.Remove(source);
    }
}