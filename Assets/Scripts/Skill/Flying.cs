using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// ��������<����>�ĵ��˹���ʱ��50%%���ʻر�<��ս><��ɨ>��25%%���ʻر�<Զ��>
/// </summary>
public class Flying : SkillInBattle
{
    [TriggerEffect(@"^Replace\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�����˺��ļ�����Զ��/��ս/��ɨ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        if (monsterBeHurt == gameObject)
        {
            if (skillInBattle is Melee || skillInBattle is Thrash)
            {
                int r = RandomUtils.GetRandomNumber(1, 2);
                return r <= 1;
            }

            if (skillInBattle is Ranged)
            {
                int r = RandomUtils.GetRandomNumber(1, 4);
                return r <= 1;
            }
        }
        return false;
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
        if (skillInBattle.gameObject == gameObject)
        {
            return true;
        }
        return false;
    }
}