using System;

/// <summary>
/// 被标注的方法是能被时机触发的技能效果
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TriggerEffectAttribute : Attribute
{
    /// <summary>
    /// 时机，正则表达式
    /// </summary>
    private readonly string opportunity;

    /// <summary>
    /// 用来进行比较的方法的名字
    /// </summary>
    private readonly string comparer;

    public string GetOpportunity() => opportunity;

    public string GetComparer() => comparer;

    public TriggerEffectAttribute(string opportunity)
    {
        this.opportunity = opportunity;
    }
    public TriggerEffectAttribute(string opportunity, string comparer)
    {
        this.opportunity = opportunity;
        this.comparer = comparer;
    }
}
