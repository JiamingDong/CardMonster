using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战场上挂在怪兽上、结算中的消耗品上、英雄技能上的技能管理类的父类
/// </summary>
public class GameObjectInBattle : MonoBehaviour
{
    /// <summary>
    /// 所有技能和状态
    /// </summary>
    public List<SkillInBattle> skillList = new();

    /// <summary>
    /// 发动技能
    /// </summary>
    /// <param name="opportunity">时机</param>
    public IEnumerator LaunchSkill(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        for (int i = 0; i < skillList.Count; i++)
        {
            SkillInBattle skill = skillList[i];
            yield return battleProcess.StartCoroutine(skill.ExecuteEligibleEffect(parameterNode));
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
