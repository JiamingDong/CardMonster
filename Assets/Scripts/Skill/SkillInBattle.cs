using System.Collections;
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
    /// ��ȡ������ֵ��С��0�Ļ᷵��0
    /// </summary>
    /// <returns>������ֵ</returns>
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
    /// ���Ӽ�����ֵ����������װ��
    /// </summary>
    /// <param name="source">��Դ</param>
    /// <param name="value">��ֵ</param>
    public int AddValue(string source, int value)
    {
        if (sourceAndValue.ContainsKey(source))
            sourceAndValue[source] += value;
        else
            sourceAndValue.Add(source, value);

        return GetSKillValue();
    }

    /// <summary>
    /// �Ƴ�ĳ����Դ����ֵ����ʧȥװ��
    /// </summary>
    /// <param name="source">��Դ</param>
    public void RemoveValue(string source)
    {
        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue.Remove(source);
        }
    }

    /// <summary>
    /// ��ü�������
    /// </summary>
    /// <param name="skillName">������</param>
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
    /// �Ƿ��ǻ�������Ч��
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