using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        yield return null;
    }
}