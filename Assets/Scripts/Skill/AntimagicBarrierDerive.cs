using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ħ��������������
/// �ܵ���ħ���˺�����%d�㡣ֱ���Է��غϽ���������������ֵ��Ч��
/// �ܵ���ħ���˺�����%d�㡣�Է��غϽ���ʱʧȥ�˼��ܡ�
/// </summary>
public class AntimagicBarrierDerive : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
        effectList.Add(Effect2);
    }

    [TriggerEffectCondition("Before.GameAction.HurtMonster", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];

        if (effectTarget == gameObject)
        {
            int damageValue = (int)parameter["DamageValue"];
            parameter["DamageValue"] = damageValue - GetSKillValue();
        }
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        if (monsterBeHurt != gameObject)
        {
            return false;
        }

        object launchedSkill = parameter["LaunchedSkill"];
        if (launchedSkill is not Magic)
        {
            return false;
        }

        return true;
    }

    [TriggerEffectCondition("InRoundReady", compareMethodName = "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = gameObject.gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter = new();
        parameter.Add("SkillName", "AntimagicBarrierDerive");
        parameter.Add("SkillValue", -GetSKillValue());
        parameter.Add("Source", "AntimagicBarrierDerive.Effect2");
        yield return StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameter));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��ǵ�ǰ�غϽ�ɫ
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}