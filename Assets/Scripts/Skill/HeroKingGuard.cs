using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKingGuard : SkillInBattle
{
    private void Awake()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("MonsterEnterBattle")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();


        yield return null;
    }
}