using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 散射
/// 此卡进行“随机伤害”攻击时，可以额外多进行%d次。
/// </summary>
public class ComboChance : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("After.Chance.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        parameterNode.parameter.Add("isAdditionalExecute", true);

        Func<ParameterNode, IEnumerator> effect = ((Chance)parameterNode.creator).Effect1;
        string fullName = "Chance.Effect1";
        for (int i = 0; i < GetSKillValue(); i++)
        {
            yield return StartCoroutine(battleProcess.ExecuteEffect(parameterNode.creator, effect, parameterNode.parameter, fullName));
            yield return null;
        }
    }

    /// <summary>
    /// 判断是否是本怪兽
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
