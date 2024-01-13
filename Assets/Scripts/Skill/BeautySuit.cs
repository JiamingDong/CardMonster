using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玉衣
/// 装备的装备牌被破坏时，获得X水晶。
/// </summary>
public class BeautySuit : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.DestroyEquipment$",  "Compare1")]
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
                    parameter.Add("CrystalAmount", GetSkillValue());
                    parameter.Add("Player", systemPlayerData.perspectivePlayer);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monster = (GameObject)parameter["EffectTarget"];
        if (monster == gameObject)
        {
            return true;
        }

        return false;
    }
}
