using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 侵染
/// 造成真实伤害或物理攻击命中时，敌人获得%d点“疾病”效果
/// </summary>
public class Disease : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$",  "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillName", "disease_derive");
        parameter1.Add("SkillValue", GetSkillValue());
        parameter1.Add("Source", "Skill.Disease.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断伤害来源是否是本怪兽/消耗品
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        DamageType damageType = (DamageType)parameter["DamageType"];
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];

        if (monsterBeHurt == null)
        {
            return false;
        }

        if (skillInBattle.gameObject != gameObject)
        {
            return false;
        }

        if (damageType != DamageType.Real && damageType != DamageType.Physics)
        {
            return false;
        }

        return true;
    }
}
