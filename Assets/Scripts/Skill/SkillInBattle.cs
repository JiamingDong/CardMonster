using System.Collections.Generic;

/// <summary>
/// �����ڳ��ϵĿ��Ƽ��ܡ�Ӣ�ۼ��ܵĸ���
/// </summary>
public abstract class SkillInBattle : OpportunityEffect
{
    /// <summary>
    /// ������Դ����ֵ
    /// </summary>
    public Dictionary<string, int> sourceAndValue = new();

    /// <summary>
    /// ��ȡ������ֵ�����sourceAndValueΪ�շ���-1������С��0�Ļ᷵��0
    /// </summary>
    /// <returns>������ֵ</returns>
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
    /// ���Ӽ�����ֵ����������װ��
    /// </summary>
    /// <param name="source">��Դ</param>
    /// <param name="value">��ֵ</param>
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
    /// �Ƴ�ĳ����Դ����ֵ����ʧȥװ��
    /// </summary>
    /// <param name="source">��Դ</param>
    public void RemoveValue(string source)
    {
        sourceAndValue.Remove(source);
    }
}