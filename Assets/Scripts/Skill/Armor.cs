using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 护甲
/// 受到真实或物理伤害时，减少等量护甲值而非生命值；护甲被破坏时，消灭提供护甲的装备，格挡剩余伤害；
/// 25%%几率回避“远程”攻击
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
    /// 判断是否是本怪兽，伤害类型是物理伤害或真实伤害
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
    /// 判断是否是本怪兽，造成伤害的技能是远程
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