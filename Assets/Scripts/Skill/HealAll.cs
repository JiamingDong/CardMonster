using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ⱥ������
/// �ҷ�ս���׶Σ��ҷ����й��޻ظ�%d������ֵ
/// </summary>
public class HealAll : SkillInBattle
{
    [TriggerEffect("^InRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        PlayerData playerMessage = null;//�������ܵĹ����������
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    playerMessage = battleProcess.systemPlayerData[i];
                    goto end;
                }
            }
        }
    end:;

        for (int i = 2; i > -1; i--)
        {
            GameObject go = playerMessage.monsterGameObjectArray[i];
            if (go != null)
            {
                //����
                Dictionary<string, object> treatParameter = new();
                //��ǰ����
                treatParameter.Add("LaunchedSkill", this);
                //Ч������
                treatParameter.Add("EffectName", "Effect1");
                //�ܵ����ƵĹ���
                treatParameter.Add("EffectTarget", go);
                //������ֵ
                treatParameter.Add("TreatValue", GetSkillValue());

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = treatParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, parameterNode1));
                yield return null;
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ��ǵ�ǰ�غϽ�ɫ���ҷ���û�����˵Ĺ���
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Ally)
            {
                GameObject[] gameObjects = battleProcess.systemPlayerData[i].monsterGameObjectArray;
                for (int j = 0; j < gameObjects.Length; j++)
                {
                    if (gameObjects[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
