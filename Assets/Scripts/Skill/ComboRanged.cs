using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// ���ⷢ��%d��<Զ��>����
/// </summary>
public class ComboRanged : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("After.Ranged.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        parameterNode.parameter.Add("isAdditionalExecute", true);

        Func<ParameterNode, IEnumerator> effect = ((Ranged)parameterNode.creator).Effect1;
        string fullName = "Ranged.Effect1";
        for (int i = 0; i < GetSKillValue(); i++)
        {
            yield return StartCoroutine(battleProcess.ExecuteEffect(parameterNode.creator, effect, parameterNode.parameter, fullName));
            yield return null;
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        if (parameter.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        GameObject go = skillInBattle.gameObject;

        if (go != gameObject)
        {
            return false;
        }

        return true;
    }
}
