using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ƻ���
/// ���Ƽ������޺󣬸ù���������%s��Ļ��ܡ�ħ�����ܻ���ʥ�ܣ�ֱ���з��غϽ�������ÿ������ÿ���غ�ֻ�ܻ��һ�Σ�
/// </summary>
public class HealShield : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.TreatMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = effectTarget.GetComponent<MonsterInBattle>();

        //Dictionary<string, object> parameter1 = new();
        //parameter1.Add("LaunchedSkill", this);
        //parameter1.Add("EffectName", "Effect1");
        //parameter1.Add("SkillValue", GetSkillValue());
        //parameter1.Add("Source", "Skill.HealShield.Effect1");

        //int r = RandomUtils.GetRandomNumber(1, 3);
        //switch (r)
        //{
        //    case 1:
        //        parameter1.Add("SkillName", "shield");
        //        break;
        //    case 2:
        //        parameter1.Add("SkillName", "magic_shield");
        //        break;
        //    case 3:
        //        parameter1.Add("SkillName", "power_shield");
        //        break;
        //}

        //ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        //parameterNode1.parameter = parameter1;

        //yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        //yield return null;

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect1");
        parameter2.Add("SkillValue", GetSkillValue());
        parameter2.Add("Source", "Skill.HealShield.Effect1");
        parameter2.Add("SkillName", "heal_shield_derive");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
    }

    /// <summary>
    /// �ж��Ƿ��ǵ�ǰ�غϽ�ɫ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];

        if (effectTarget == null)
        {
            return false;
        }

        if (skillInBattle.gameObject != gameObject)
        {
            return false;
        }

        return true;
    }
}