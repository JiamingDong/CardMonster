using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玉衣
/// 装备的装备牌被破坏时，获得X水晶。
/// </summary>
public class BeautySuit : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("After.GameAction.DestroyEquipment", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, object> parameter = new();
                    parameter.Add("CrystalAmount", GetSKillValue());
                    parameter.Add("Player", systemPlayerData.perspectivePlayer);

                    yield return StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameter));
                }
            }
        }


        yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monster = (GameObject)parameter["MonsterBeDestroyEquipment"];
        if (monster == gameObject)
        {
            return true;
        }

        return false;
    }
}
