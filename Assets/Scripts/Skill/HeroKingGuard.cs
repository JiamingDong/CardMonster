using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������ػ�
/// �ҷ����޽���ʱ�������+2��
/// Ӣ�ۼ���
/// </summary>
public class HeroKingGuard : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.MonsterEnterBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.result;
        GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];

        MonsterInBattle monsterInBattle = monsterBeGenerated.GetComponent<MonsterInBattle>();
        monsterInBattle.maxHp += 2;
        int currentHp = monsterInBattle.GetCurrentHp();
        monsterInBattle.SetCurrentHp(currentHp + 2);

        yield break;
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ�������
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.result;
        GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        Player? thisPlayer = null;
        Player? targetPlayer = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            if (playerData.heroSkillGameObject == gameObject)
            {
                thisPlayer = playerData.perspectivePlayer;
            }

            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == monsterBeGenerated)
                {
                    targetPlayer = playerData.perspectivePlayer;
                }
            }
        }

        return targetPlayer == thisPlayer;
    }
}