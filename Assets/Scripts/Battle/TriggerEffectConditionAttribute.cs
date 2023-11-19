using System;

/// <summary>
/// 触发技能效果需要的条件
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TriggerEffectConditionAttribute : Attribute
{
    /// <summary>
    /// 时机
    /// </summary>
    private readonly string opportunity;

    /// <summary>
    /// 用来进行比较的方法的名字
    /// </summary>
    public string compareMethodName;

    public string GetOpportunity() => opportunity;

    public TriggerEffectConditionAttribute(string opportunity)
    {
        this.opportunity = opportunity;
    }
}
