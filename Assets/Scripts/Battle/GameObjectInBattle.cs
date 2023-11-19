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
        foreach (SkillInBattle skill in skillList)
        {
            yield return StartCoroutine(skill.ExecuteEligibleEffect(parameterNode));
            yield return null;
        }

        yield return null;
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
