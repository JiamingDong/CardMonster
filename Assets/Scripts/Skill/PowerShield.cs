using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 圣盾
/// 减少此卡受到的物理和魔法伤害
/// </summary>
public class PowerShield : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];

        if (effectTarget == gameObject)
        {
            int damageValue = (int)parameter["DamageValue"];
            parameter["DamageValue"] = damageValue - GetSkillValue();
        }
        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        DamageType damageType = (DamageType)parameter["DamageType"];
        return (damageType == DamageType.Magic || damageType == DamageType.Physics) && monsterBeHurt == gameObject;
    }
}
