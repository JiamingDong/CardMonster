using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// <��ս><Զ��>��������<����>�ĵ���ʱ�����˺�+%d���޷���<����>�ر�
/// </summary>
public class Antiair : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        parameter["DamageValue"] = damageValue + GetSkillValue();

        yield break;
        //yield return null;
    }

    /// <summary>
    /// �жϽ�ս��Զ�̣����з���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];
        return (launchedSkill is Melee || launchedSkill is Ranged) && launchedSkill.gameObject == gameObject && monsterBeHurt.TryGetComponent<Flying>(out _);
    }

    [TriggerEffect(@"^Replace\.Flying\.Effect1$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// �жϴ����Է�������˺��ļ��ܵķ������Ƿ��Ǳ�����
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        //��ɴ���������˺��ļ���
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if ((skillInBattle is Melee || skillInBattle is Ranged) && skillInBattle.gameObject == gameObject)
        {
            return true;
        }

        return false;
    }
}