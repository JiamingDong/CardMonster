using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自爆
/// 回合开始时，消灭此怪兽
/// </summary>
public class SelfExplosive : SkillInBattle
{
    [TriggerEffect("^InRoundReady", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter = new();
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect1");
        parameter.Add("EffectTarget", gameObject);
        parameter.Add("Destroyer", gameObject);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyMonster, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断是否是己方回合
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
