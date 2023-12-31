using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ᴩ
/// <ħ��>���ӵ��˵�<��������>��<��������>��<��������>���ӵ��˵�<��������>��
/// </summary>
public class PenetrateMagic : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Reflect\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Mshield\.Effect1$", "Compare2")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// �жϴ����Է�����������˺��ļ��ܵķ������Ƿ��Ǳ����ޡ�������ħ��
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        //��ɴ�������������˺��ļ���
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        return skillInBattle.gameObject == gameObject && skillInBattle is Magic;
    }

    /// <summary>
    /// �жϴ����Է��������ܵ��˺��ļ��ܵķ������Ƿ��Ǳ����ޡ�������ħ��/��������
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        //��ɴ����������ܵ��˺��ļ���
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        return skillInBattle.gameObject == gameObject && (skillInBattle is Magic || skillInBattle is MagicAoe);
    }
}
