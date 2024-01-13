using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Melee melee = gameObject.GetComponent<Melee>();

        string fullName = "Melee.Effect1";

        ParameterNode parameterNode1 = new();
        parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
        parameterNode1.opportunity = "InRoundBattle";
        parameterNode1.result.Add("isAdditionalExecute", true);

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(melee, fullName, parameterNode1, melee.Effect1));
    }

    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;

        Debug.Log("二次攻击");
        if (parameterNode.Parent != null)
        {
            Debug.Log(parameterNode.Parent.creator.GetType().Name);

            if (parameterNode.Parent.EffectChild != null)
            {
                Debug.Log(parameterNode.Parent.EffectChild.creator.GetType().Name);

                if (parameterNode.Parent.EffectChild.nodeInMethodList.Count > 0)
                {
                    Debug.Log(parameterNode.Parent.EffectChild.nodeInMethodList[0].creator.GetType().Name);

                    if (parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild != null)
                    {
                        Debug.Log(parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.creator.GetType().Name);

                        if (parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.result != null)
                        {
                            Debug.Log("二次攻击----" + result);
                            foreach (var item in parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.result)
                            {
                                Debug.Log(item.Key + "=" + item.Value);
                            }
                        }
                    }
                }
            }
        }

        Dictionary<string, object> result2 = parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.result;

        if (result.ContainsKey("isAdditionalExecute"))
        {
            Debug.Log("isAdditionalExecute");
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
            Debug.Log("go != gameObject");
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
                        Debug.Log("launchedRecord.ContainsKey(playerData.roundNumber)");
                        return false;
                    }
                }
            }

            if (isEnemy)
            {
                if (playerData.monsterGameObjectArray[0] == null)
                {
                    Debug.Log("playerData.monsterGameObjectArray[0] == null");
                    return false;
                }
            }
        }

        return true;
    }
}