using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// 二次攻击
/// 一回合一次，如果本次<近战>未造成生命值伤害，则再进行一次<近战>攻击
/// </summary>
public class DoubleMelee : SkillInBattle
{
    Dictionary<int, int> launchedRecord = new();

    [TriggerEffect(@"^After\.Melee\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.Parent.creator;
        GameObject go = skillInBattle.gameObject;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == gameObject)
                {
                    launchedRecord.Add(playerData.roundNumber, 1);
                    break;
                }
            }
        }

        string fullName = "Melee.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            ParameterNode parameterNode1 = new();
            parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
            parameterNode1.opportunity = "InRoundBattle";
            parameterNode1.result.Add("isAdditionalExecute", true);

            if (go != null && go.TryGetComponent<Melee>(out var melee) && melee.CompareCondition(melee.Effect1, parameterNode1))
            {
                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(melee, fullName, parameterNode1, melee.Effect1));
                yield return null;
            }
        }
    }

    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;

        foreach (var item in parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.parameter)
        {
            Debug.Log(item.Key + "=" + item.Value);
        }

        Dictionary<string, object> result2 = parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.result;

        if (result.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        if (result2.ContainsKey("CauseDamageToHealth"))
        {
            Debug.Log("CauseDamageToHealth");
            return false;
        }

        BattleProcess battleProcess = BattleProcess.GetInstance();

        GameObject go = skillInBattle.gameObject;
        if (go != gameObject)
        {
            return false;
        }

        if (!gameObject.TryGetComponent<Ranged>(out _))
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            bool isEnemy = true;
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == gameObject)
                {
                    isEnemy = false;

                    if (launchedRecord.ContainsKey(playerData.roundNumber))
                    {
                        return false;
                    }
                }
            }

            if (isEnemy)
            {
                if (playerData.monsterGameObjectArray[0] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}