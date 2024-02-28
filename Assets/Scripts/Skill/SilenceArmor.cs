using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 寂静之铠
/// 所受到的魔法伤害减半；（带有龙血之铠技能时此技能不生效）
/// </summary>
public class SilenceArmor : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        parameter["DamageValue"] = damageValue / 2;

        yield break;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        DamageType damageType = (DamageType)parameter["DamageType"];
        return damageType == DamageType.Magic && monsterBeHurt == gameObject && !gameObject.TryGetComponent(out DragonBlood _);
    }
}
