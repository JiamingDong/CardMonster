using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// �ҷ�ս���׶ο�ʼʱ��50%%����ʹ��������˻�á�������
/// </summary>
public class Entangle : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
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

        int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 1, 0, 2 }, new int[] { 2, 0, 1 } };
        GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][0]];

        MonsterInBattle monsterInBattle = effectTarget.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter = new();
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect1");
        parameter.Add("SkillName", "entangle_derive");
        parameter.Add("SkillValue", 0);
        parameter.Add("Source", "Skill.Entangle.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;
        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
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
                        int r = RandomUtils.GetRandomNumber(1, 2);
                        return r == 1;
                    }
                }
            }
        }

        return false;
    }
}