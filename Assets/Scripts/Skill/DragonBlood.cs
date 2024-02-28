using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ѫ֮��
/// �ܵ��������˺����루����ȡ������
/// </summary>
public class DragonBlood : SkillInBattle
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
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        return monsterBeHurt == gameObject;
    }
}
