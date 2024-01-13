using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ħ������
/// �ҷ�ս���׶ο�ʼʱ���ҷ����й����ܵ���ħ���˺�����%d�㡣ֱ���Է��غϽ���������������ֵ��Ч��
/// �ҷ����й��޻�á���ħ�������������ܡ���AntimagicBarrierDerive��
/// </summary>
public class AntimagicBarrier : SkillInBattle
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

                            Dictionary<string, object> parameter = new();
                            parameter.Add("LaunchedSkill", this);
                            parameter.Add("EffectName", "Effect1");
                            parameter.Add("SkillName", "antimagic_barrier_derive");
                            parameter.Add("SkillValue", GetSkillValue());
                            parameter.Add("Source", "Skill.AntimagicBarrier.Effect1");

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter;
                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                        }
                    }
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