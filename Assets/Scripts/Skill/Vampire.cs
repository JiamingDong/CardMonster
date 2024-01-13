using System.Collections;
using UnityEngine;

/// <summary>
/// ��Ѫ
/// ������������ʱ������ֵ������ֵ����+%d������������<��ս><Զ��><ħ��><����˺�>��
/// </summary>
public class Vampire : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
        monsterInBattle.maxHp += GetSkillValue();
        monsterInBattle.SetCurrentHp(monsterInBattle.GetCurrentHp() + GetSkillValue());

        yield break;
    }

    /// <summary>
    /// �ж��˺���Դ�Ƿ��Ǳ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];

        if (monsterBeHurt == null)
        {
            return false;
        }

        if (skillInBattle.gameObject != gameObject)
        {
            return false;
        }

        if (skillInBattle is not Melee && skillInBattle is not Magic && skillInBattle is not Ranged && skillInBattle is not Chance)
        {
            return false;
        }

        return true;
    }
}
