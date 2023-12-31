using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// <����ʿ��><���ָ��><ǿ��><����><ħ������>ʧЧ���ܵ�������ʱ������<����>��ֵ�����������ơ�
/// </summary>
public class DiseaseDerive : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Rally\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.ShootCommand\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.ChanceFate\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Demoralize\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Reflect\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        Dictionary<string, object> result = parameterNode.result;
        return skillInBattle.gameObject == gameObject && !result.ContainsKey("BeReplaced");
    }

    [TriggerEffect(@"^Before\.GameAction\.TreatMonster$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int treatValue = (int)parameter["TreatValue"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        int skillValue = GetSkillValue();

        if (skillValue <= treatValue)
        {
            parameter["TreatValue"] = treatValue - skillValue;

            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect2");
            parameter1.Add("SkillName", "disease_derive");
            parameter1.Add("SkillValue", -skillValue);
            parameter1.Add("Source", "GameAction.TreatMonster");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }
        else
        {
            parameter["TreatValue"] = 0;

            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect2");
            parameter1.Add("SkillName", "disease_derive");
            parameter1.Add("SkillValue", -treatValue);
            parameter1.Add("Source", "GameAction.TreatMonster");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        yield return null;
    }


    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeTreat = (GameObject)parameter["MonsterBeTreat"];

        if (monsterBeTreat == gameObject)
        {
            return true;
        }

        return false;
    }
}
