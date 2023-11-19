using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ļ
/// ��˿���ͬ��Ӫ�Ĺ��ޣ���������������⹥��X�Ρ�
/// </summary>
public class Barrage : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("After.Chance.Effect1", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        parameterNode.parameter.Add("isAdditionalExecute", true);

        Func<ParameterNode, IEnumerator> effect = ((Chance)parameterNode.creator).Effect1;
        string fullName = "Chance.Effect1";
        for (int i = 0; i < GetSKillValue(); i++)
        {
            yield return StartCoroutine(battleProcess.ExecuteEffect(parameterNode.creator, effect, parameterNode.parameter, fullName));
            yield return null;
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ������޲�����Ӫ��ͬ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        if (parameter.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        GameObject go = skillInBattle.gameObject;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        Player? thisPlayer = null;
        Player? targetPlayer = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == gameObject)
                {
                    thisPlayer = playerData.perspectivePlayer;
                }
                if (playerData.monsterGameObjectArray[j] == go)
                {
                    targetPlayer = playerData.perspectivePlayer;
                }
            }
        }

        if (targetPlayer != thisPlayer)
        {
            return false;
        }

        MonsterInBattle thisMonster = gameObject.GetComponent<MonsterInBattle>();
        MonsterInBattle targetMonster = go.GetComponent<MonsterInBattle>();

        if (!thisMonster.kind.Equals(targetMonster.kind))
        {
            return false;
        }

        return true;
    }
}
