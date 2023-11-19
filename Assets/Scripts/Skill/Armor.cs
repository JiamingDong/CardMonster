using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// �ܵ���ʵ�������˺�ʱ�����ٵ�������ֵ��������ֵ�����ױ��ƻ�ʱ�������ṩ���׵�װ������ʣ���˺���
/// 25%%���ʻرܡ�Զ�̡�����
/// </summary>
public class Armor : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("Replace.GameAction.HurtMonster", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        MonsterInBattle monsterInBattle = gameObject.AddComponent<MonsterInBattle>();

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("SkillName", "Armor");
        parameter2.Add("SkillValue", -damageValue);
        parameter2.Add("Source", "Damage");
        yield return StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameter2));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ��˺������������˺�����ʵ�˺�
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        DamageType damageType = (DamageType)parameter["DamageType"];
        if (monsterBeHurt == gameObject && (damageType == DamageType.Physics || damageType == DamageType.Real))
        {
            return true;
        }
        return false;
    }


    [TriggerEffectCondition("Replace.GameAction.HurtMonster", compareMethodName = "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        MonsterInBattle monsterInBattle = gameObject.AddComponent<MonsterInBattle>();

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("SkillName", "Armor");
        parameter2.Add("SkillValue", -damageValue);
        parameter2.Add("Source", "Damage");
        yield return StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameter2));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�����˺��ļ�����Զ��
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (monsterBeHurt == gameObject && skillInBattle is Ranged)
        {
            return true;
        }
        return false;
    }
}