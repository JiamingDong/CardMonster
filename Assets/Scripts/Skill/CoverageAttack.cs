using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ǹ���
/// ��ÿ�ֻ������ܸ���һ�Ρ���������˺�/ħ��ʱ����Ϊ�����ез����޸�����һ������˺�/ħ����Ч����
/// ֻ��ִ��Ч�������Բ��ᴥ�����䡢ɢ�䡢��Ļ��������Ч����
/// </summary>
public class CoverageAttack : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
        effectList.Add(Effect2);
    }

    [TriggerEffectCondition("Replace.Magic.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //���ڹ��޵�λ��
        int position = -1;
        //�Է����
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    position = j;
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        //ѡȡ����Ŀ��
        for (int i = 0; i < 3; i++)
        {
            GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[i];

            if (effectTarget == null)
            {
                continue;
            }

            int skillValue = gameObject.GetComponent<Magic>().GetSKillValue();

            //�˺�����
            Dictionary<string, object> damageParameter = new();
            //��ǰ����
            damageParameter.Add("LaunchedSkill", this);
            //Ч������
            damageParameter.Add("EffectName", "Effect1");
            //�ܵ��˺��Ĺ���
            damageParameter.Add("EffectTarget", effectTarget);
            //�˺���ֵ
            damageParameter.Add("DamageValue", skillValue);
            //�˺�����
            damageParameter.Add("DamageType", DamageType.Magic);

            yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

            yield return null;
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        if (parameter.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        GameObject go = skillInBattle.gameObject;

        if (go != gameObject)
        {
            return false;
        }

        return true;
    }

    [TriggerEffectCondition("Replace.Chance.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        System.Random random = new();

        //�Է����
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        //����1��λû���޾ͷ���
        if (oppositePlayerMessage.monsterGameObjectArray[0] == null)
        {
            yield break;
        }

        //ѡȡ����Ŀ��
        for (int i = 0; i < 3; i++)
        {
            GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[i];

            if (effectTarget == null)
            {
                continue;
            }

            int skillValue = gameObject.GetComponent<Chance>().GetSKillValue();

            if (skillValue > 0)
            {
                Dictionary<string, object> keyValuePairs = new();
                yield return StartCoroutine(NetworkMessageUtils.GetRandomResult(0, skillValue, keyValuePairs));
                skillValue = (int)keyValuePairs["RandomResult"];
            }

            //�˺�����
            Dictionary<string, object> damageParameter = new();
            //��ǰ����
            damageParameter.Add("LaunchedSkill", this);
            //Ч������
            damageParameter.Add("EffectName", "Effect2");
            //�ܵ��˺��Ĺ���
            damageParameter.Add("EffectTarget", effectTarget);
            //�˺���ֵ
            damageParameter.Add("DamageValue", skillValue);
            //�˺�����
            damageParameter.Add("DamageType", DamageType.Real);

            yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

            yield return null;
        }

    }
}
