using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ħ��
/// �ڻغ�ս���У������ڹ���λ��Ϊ1��λ���ԶԷ�1��������ɵ��ڼ�����ֵ�������˺������ȼ�Ϊ1��2��3��
/// ��Ϊ2��λ�����ȼ�Ϊ2��1��3����Ϊ3��λ�����ȼ�Ϊ3��1��2��
/// </summary>
public class Magic : SkillInBattle
{
    public Magic(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle", compareMethodName = "Compare1")]
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
        int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 1, 0, 2 }, new int[] { 2, 0, 1 } };
        GameObject effectTarget = null;
        for (int i = 0; i < 3; i++)
        {
            effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];
            if (effectTarget != null)
            {
                ParameterNode parameterNode1 = new();
                parameterNode1.opportunity = "Magic.Effect1.ChoiceTarget";

                //Dictionary<string, object> targetCondition = new();
                //targetCondition.Add("LaunchedSkill", this);

                Dictionary<string, object> targetParameter = new();
                targetParameter.Add("LaunchedSkill", this);
                targetParameter.Add("SkillTarget", effectTarget);

                parameterNode1.parameter = targetParameter;
                //parameterNode1.condition = targetCondition;

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
        damageParameter.Add("DamageType", DamageType.Magic);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غϣ��Է������й���
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
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
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
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
