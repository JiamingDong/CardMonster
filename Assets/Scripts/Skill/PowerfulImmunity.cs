using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerfulImmunity : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        yield return null;
    }
}
