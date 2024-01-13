using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ս���Ϲ��ڹ����ϡ������е�����Ʒ�ϡ�Ӣ�ۼ����ϵļ��ܹ�����ĸ���
/// </summary>
public class GameObjectInBattle : MonoBehaviour
{
    /// <summary>
    /// ���м��ܺ�״̬
    /// </summary>
    public List<SkillInBattle> skillList = new();

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="opportunity">ʱ��</param>
    public IEnumerator LaunchSkill(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        //ѭ�������п����м��ܱ��Ƴ�
        HashSet<SkillInBattle> hasLaunchedSkill = new();
        var maxCount = skillList.Count * 2;
        var j = 0;
        while (j < maxCount)
        {
            bool a = true;

            for (int i = 0; i < skillList.Count; i++)
            {
                SkillInBattle skill = skillList[i];
                if (!hasLaunchedSkill.Contains(skill))
                {
                    yield return battleProcess.StartCoroutine(skill.ExecuteEligibleEffect(parameterNode));
                    hasLaunchedSkill.Add(skill);
                    a = false;
                    break;
                }
            }

            if (a)
            {
                break;
            }
        }
    }

    public PlayerData GetPlayerDataBelongTo()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            for (int j = 2; j > -1; j--)
            {
                if (playerData.monsterGameObjectArray[j] == gameObject)
                {
                    return playerData;
                }
            }

            if (playerData.heroSkillGameObject == gameObject)
            {
                return playerData;
            }

            if (playerData.consumeGameObject == gameObject)
            {
                return playerData;
            }
        }

        return null;
    }
}
