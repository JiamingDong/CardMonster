using System;

/// <summary>
/// ��������Ч����Ҫ������
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TriggerEffectConditionAttribute : Attribute
{
    /// <summary>
    /// ʱ��
    /// </summary>
    private readonly string opportunity;

    /// <summary>
    /// �������бȽϵķ���������
    /// </summary>
    public string compareMethodName;

    public string GetOpportunity() => opportunity;

    public TriggerEffectConditionAttribute(string opportunity)
    {
        this.opportunity = opportunity;
    }
}
