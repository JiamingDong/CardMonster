using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ⱥ������
/// </summary>
public class HealAll : SkillInBattle
{
    public HealAll(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle")]
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
            GameObject gameObject = playerMessage.monsterGameObjectArray[i];
            if (gameObject != null)
            {
                //����
                //ParameterNode parameterNode1 = parameterNode.AddChild();

                //parameterNode1.condition.Add("NodeCreator", this);

                Dictionary<string, object> treatParameter = new();
                //�ܵ����ƵĹ���
                treatParameter.Add("MonsterBeTreat", gameObject);
                //������ֵ
                treatParameter.Add("TreatValue", GetSKillValue());

                yield return StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, treatParameter));

                yield return null;
            }
        }
    }
}
