using System;

/// <summary>
/// ����ע�ķ������ܱ�ʱ�������ļ���Ч��
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TriggerEffectAttribute : Attribute
{
    /// <summary>
    /// ʱ����������ʽ
    /// </summary>
    private readonly string opportunity;

    /// <summary>
    /// �������бȽϵķ���������
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
