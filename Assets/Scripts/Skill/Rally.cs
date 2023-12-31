using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʿ��
/// �ҷ�ս���׶ο�ʼʱ���������ѵ�<��ս>ֵ+%d�������ֵ��Ч��������<�־�>���������з��غϽ���
/// </summary>
public class Rally : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                GameObject monsterGameObject = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                if (monsterGameObject == gameObject)
                {
                    for (int k = 2; k > -1; k--)
                    {
                        GameObject go = battleProcess.systemPlayerData[i].monsterGameObjectArray[k];
                        if (go != null)
                        {
                            MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();

                            Dictionary<string, object> parameter1 = new();
                            parameter1.Add("LaunchedSkill", this);
                            parameter1.Add("EffectName", "Effect1");
                            parameter1.Add("SkillName", "melee");
                            parameter1.Add("SkillValue", GetSkillValue());
                            parameter1.Add("Source", "Skill.Rally.Effect1");

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter1;
                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("LaunchedSkill", this);
                            parameter2.Add("EffectName", "Effect1");
                            parameter2.Add("SkillName", "rally_derive");
                            parameter2.Add("SkillValue", 0);
                            parameter2.Add("Source", "Skill.Rally.Effect1");

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;
                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
                        }
                    }

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ��ǵ�ǰ�غϽ�ɫ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
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