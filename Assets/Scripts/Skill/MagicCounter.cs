using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 反击：魔法
/// 被<魔法>命中时，50%%几率对攻击者发动一次<近战>攻击
/// </summary>
public class MagicCounter : SkillInBattle
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

        Melee melee = gameObject.GetComponent<Melee>();

        //从Melee.Effect1改写为指定目标的效果
        IEnumerator Effect(ParameterNode parameterNode)
        {
            //选取技能目标
            GameObject effectTarget = skillInBattle.gameObject;

            //伤害参数
            Dictionary<string, object> damageParameter = new();
            //当前技能
            damageParameter.Add("LaunchedSkill", melee);
            //效果名称
            damageParameter.Add("EffectName", "Effect1");
            //受到伤害的怪兽
            damageParameter.Add("EffectTarget", effectTarget);
            //伤害数值
            damageParameter.Add("DamageValue", melee.GetSkillValue());
            //伤害类型
            damageParameter.Add("DamageType", DamageType.Physics);

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = damageParameter;

            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
            yield return null;
        }

        string fullName = "Melee.Effect1";

        ParameterNode parameterNode1 = new();
        parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(melee, fullName, parameterNode1, Effect));
        yield return null;
    }

    /// <summary>
    /// 判断是本怪兽、伤害来源是魔法、具有近战
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        if (monsterBeHurt == gameObject && skillInBattle is Magic && effectName.Equals("Effect1") && skillInBattle.gameObject != null && gameObject.TryGetComponent(out Melee _))
        {
            return true;
        }

        return false;
    }
}
