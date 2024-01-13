using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �
/// ���ƻ�ʱ�����ҷ����й������%d����ʵ�˺���
/// </summary>
public class SmallBurst : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.DestroyEquipment$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    for (int k = 0; k < systemPlayerData.monsterGameObjectArray.Length; k++)
                    {
                        GameObject go = systemPlayerData.monsterGameObjectArray[k];
                        if (go != null)
                        {
                            MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();
                            if (monsterInBattle.GetCurrentHp() > 0)
                            {
                                Dictionary<string, object> damageParameter = new();
                                damageParameter.Add("LaunchedSkill", this);
                                damageParameter.Add("EffectName", "Effect1");
                                damageParameter.Add("EffectTarget", systemPlayerData.monsterGameObjectArray[k]);
                                damageParameter.Add("DamageValue", GetSkillValue());
                                damageParameter.Add("DamageType", DamageType.Real);

                                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                                parameterNode2.parameter = damageParameter;

                                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode2));
                            }
                        }
                    }
                }
            }
        }

        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monster = (GameObject)parameter["EffectTarget"];
        if (monster == gameObject)
        {
            return true;
        }

        return false;
    }

    [TriggerEffect(@"^Before\.GameAction\.DestroyMonster$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    for (int k = 0; k < systemPlayerData.monsterGameObjectArray.Length; k++)
                    {
                        GameObject go = systemPlayerData.monsterGameObjectArray[k];
                        if (go != null && go != gameObject)
                        {
                            MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();
                            if (monsterInBattle.GetCurrentHp() > 0)
                            {
                                Dictionary<string, object> damageParameter = new();
                                damageParameter.Add("LaunchedSkill", this);
                                damageParameter.Add("EffectName", "Effect1");
                                damageParameter.Add("EffectTarget", systemPlayerData.monsterGameObjectArray[k]);
                                damageParameter.Add("DamageValue", GetSkillValue());
                                damageParameter.Add("DamageType", DamageType.Real);

                                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                                parameterNode2.parameter = damageParameter;

                                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode2));
                            }
                        }
                    }
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monster = (GameObject)parameter["EffectTarget"];
        if (monster == gameObject)
        {
            return true;
        }

        return false;
    }
}
