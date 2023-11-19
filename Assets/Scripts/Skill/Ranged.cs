using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Զ��
/// Ч��1���ڻغ�ս���У������ڹ���λ��Ϊ1��λ���ԶԷ�1��������ɵ��ڼ�����ֵ�������˺������ȼ�Ϊ1��2��3��
/// ��Ϊ2��λ�����ȼ�Ϊ3��2��1����Ϊ3��λ�����ȼ�Ϊ2��3��1��
/// Ч��2������Զ�̵�Ч��1������Ч��Ϊ��Ч��
/// </summary>
public class Ranged : SkillInBattle
{
    public Ranged(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
        effectList.Add(Effect2);

    }

    [TriggerEffectCondition("InRoundBattle")]
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

        //����1��λû���޾ͷ���
        if (oppositePlayerMessage.monsterGameObjectArray[0] == null)
        {
            yield break;
        }

        //ѡȡ����Ŀ��
        int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 2, 1, 0 }, new int[] { 1, 2, 0 } };
        GameObject effectTarget = null;
        for (int i = 0; i < 3; i++)
        {
            effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];
            if (effectTarget != null)
            {
                ParameterNode parameterNode1 = new();
                parameterNode1.opportunity = "Ranged.Effect1.ChoiceTarget";

                //parameterNode.condition.Add("LaunchedSkill", this);

                parameterNode1.parameter.Add("LaunchedSkill", this);
                parameterNode1.parameter.Add("SkillTarget", effectTarget);

                yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
            }
        }

        yield return null;

        //�˺�����
        Dictionary<string, object> damageParameter = new();
        //��ǰ����
        damageParameter.Add("LaunchedSkill", this);
        //Ч������
        damageParameter.Add("EffectName", "Effect1");
        //�ܵ��˺��Ĺ���
        damageParameter.Add("EffectTarget", effectTarget);
        //�˺���ֵ
        damageParameter.Add("DamageValue", GetSKillValue());
        //�˺�����
        damageParameter.Add("DamageType", DamageType.Physics);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

        yield return null;
    }

    [TriggerEffectCondition("Replace.Ranged.Effect1")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();

        List<string> replaceReason = (List<string>)parameter["ReplaceReason"];
        List<string> antiReplaceReason = (List<string>)parameter["AntiReplaceReason"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string replaceEffectName = (string)parameter["ReplaceEffectName"];

        if (skillInBattle != this)
        {
            yield break;
        }

        if (!replaceEffectName.Equals("Effect1"))
        {
            yield break;
        }

        GameObject monsterOfLaunchingSkill = skillInBattle.gameObject;

        int position = -1;//�������ܵĹ���λ��
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == monsterOfLaunchingSkill)
                {
                    position = j;
                    goto end;
                }
            }
        }
    end:;

        if (position == 0 && !antiReplaceReason.Contains("Ranged.Effect2") && replaceReason.Count == 0)
        {
            replaceReason.Add("Melee.Effect2");
        }

        yield return null;
    }
}