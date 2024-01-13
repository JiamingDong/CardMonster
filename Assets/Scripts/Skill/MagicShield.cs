using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 法术护盾
/// 受到法术伤害时，此伤害-%d
/// </summary>
public class MagicShield : SkillInBattle
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
        return damageType == DamageType.Magic && monsterBeHurt == gameObject;
    }
}
