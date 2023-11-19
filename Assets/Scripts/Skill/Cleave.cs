using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ᴩ
/// �ƻ����˻���ʱ��ʣ���˺����ᱻ���˸�
/// �������Ч�����˺����ڻ���ʱ����ɶ����˺�
/// </summary>
public class Cleave : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("Replace.Armor.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        int damageValue = (int)parameter["DamageValue"];

        MonsterInBattle monsterInBattle = monsterBeHurt.AddComponent<MonsterInBattle>();

        Armor armor = monsterInBattle.GetComponent<Armor>();

        int armorValue = armor.GetSKillValue();

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("SkillName", "Armor");
        parameter2.Add("SkillValue", -damageValue);
        parameter2.Add("Source", "Damage");
        yield return StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameter2));

        if (armorValue < damageValue)
        {
            parameter["DamageValue"] = damageValue - armorValue;

            yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameter));
        }
        yield return null;
    }

    /// <summary>
    /// �ж��˺���Դ�Ƿ��Ǳ����ޣ��˺��������������ʵ�˺�
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        DamageType damageType = (DamageType)parameter["DamageType"];

        if (skillInBattle.gameObject == gameObject && (damageType == DamageType.Real || damageType == DamageType.Physics))
        {
            return true;
        }
        return false;
    }
}
