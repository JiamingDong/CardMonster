using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Զ��
/// �ڻغ�ս���У������ڹ���λ��Ϊ1��λ���ԶԷ�1��������ɵ��ڼ�����ֵ�������˺������ȼ�Ϊ1��2��3��
/// ��Ϊ2��λ�����ȼ�Ϊ3��2��1����Ϊ3��λ�����ȼ�Ϊ2��3��1��
/// </summary>
public class Ranged : SkillInBattle
{
    [TriggerEffect("^InRoundBattle$", "Compare1")]
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
        List<GameObject> nontargetList = new();
        List<GameObject> priorTargetList = new();
        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("NontargetList", nontargetList);
        parameter1.Add("PriorTargetList", priorTargetList);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.SelectEffectTarget, parameterNode1));
        yield return null;

        GameObject effectTarget = null;
        if (priorTargetList.Count > 0)
        {
            effectTarget = priorTargetList[0];
        }
        else
        {
            int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 2, 1, 0 }, new int[] { 1, 2, 0 } };

            for (int i = 0; i < 3; i++)
            {
                effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];
                if (effectTarget == null)
                {
                    continue;
                }

                bool isNontarget = false;
                foreach (GameObject go in nontargetList)
                {
                    if (go == effectTarget)
                    {
                        isNontarget = true;
                        break;
                    }
                }

                if (isNontarget && i != 2)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        //�˺�����
        Dictionary<string, object> damageParameter = new();
        //��ǰ����
        damageParameter.Add("LaunchedSkill", this);
        //Ч������
        damageParameter.Add("EffectName", "Effect1");
        //�ܵ��˺��Ĺ���
        damageParameter.Add("EffectTarget", effectTarget);
        //�˺���ֵ
        damageParameter.Add("DamageValue", GetSkillValue());
        //�˺�����
        damageParameter.Add("DamageType", DamageType.Physics);

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode2));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غϣ���2��3��λ���Է������й���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        bool isAlly = false;
        bool enemyHasMonster = false;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 1; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        isAlly = true;
                    }
                }
            }

            if (systemPlayerData.perspectivePlayer == Player.Enemy && systemPlayerData.monsterGameObjectArray[0] != null)
            {
                enemyHasMonster = true;
            }
        }

        return isAlly && enemyHasMonster;
    }
}