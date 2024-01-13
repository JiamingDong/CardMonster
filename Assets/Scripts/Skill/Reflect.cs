using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 法术反射
/// 受到<魔法>或<法术反射>的伤害时，75%%几率改为对攻击者造成等量法术伤害
/// </summary>
public class Reflect : SkillInBattle
{
    [TriggerEffect(@"^Replace\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        int damageValue = (int)parameter["DamageValue"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        result.Add("BeReplaced", true);

        //伤害参数
        Dictionary<string, object> damageParameter = new();
        //当前技能
        damageParameter.Add("LaunchedSkill", this);
        //效果名称
        damageParameter.Add("EffectName", "Effect1");
        //受到伤害的怪兽
        damageParameter.Add("EffectTarget", skillInBattle.gameObject);
        //伤害数值
        damageParameter.Add("DamageValue", damageValue);
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Magic);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
    }

    /// <summary>
    /// 判断是否是本怪兽，造成伤害的技能是魔法
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

        if (monsterBeHurt == gameObject && (skillInBattle is Magic || skillInBattle is Reflect))
        {
            int r = RandomUtils.GetRandomNumber(1, 4);
            return r <= 3;
        }

        return false;
    }
}
