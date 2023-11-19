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
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                GameObject monsterGameObject = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];
                MonsterInBattle monsterInBattle = monsterGameObject.GetComponent<MonsterInBattle>();

                if (monsterGameObject == gameObject)
                {
                    for (int k = 2; k > -1; k--)
                    {
                        if (monsterGameObject != null)
                        {
                            Dictionary<string, object> parameter = new();
                            parameter.Add("SkillName", "AntimagicBarrierDerive");
                            parameter.Add("SkillValue", GetSKillValue());
                            parameter.Add("Source", "Skill.AntimagicBarrier");
                            yield return StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameter));
                        }
                    }
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��ǵ�ǰ�غϽ�ɫ
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
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