using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 荆棘
/// 被<近战>命中时，对攻击者造成%d点真实伤害
/// </summary>
public class Thorns : SkillInBattle
{
    /// <summary>
    /// <see cref="Melee.Effect1"/>
    /// </summary>
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        GameObject effectTarget = skillInBattle.gameObject;

        Dictionary<string, object> damageParameter = new();
        damageParameter.Add("LaunchedSkill", this);
        damageParameter.Add("EffectName", "Effect1");
        damageParameter.Add("EffectTarget", effectTarget);
        damageParameter.Add("DamageValue", GetSkillValue());
        damageParameter.Add("DamageType", DamageType.Real);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断是本怪兽、伤害来源是近战
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        if (monsterBeHurt == gameObject && skillInBattle is Melee && effectName.Equals("Effect1") && skillInBattle.gameObject != null)
        {
            return true;
        }

        return false;
    }
}
