using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 灵魂尖啸
/// 使用时，优先将正对面敌人洗入敌方牌库
/// </summary>
public class SoulShriek : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //所在怪兽的位置
        int position = -1;
        //对方玩家
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    position = j;
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 1, 0, 2 }, new int[] { 2, 0, 1 } };
        GameObject effectTarget = null;
        for (int i = 0; i < 3; i++)
        {
            effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];

            if (effectTarget != null)
            {
                break;
            }
        }


        MonsterInBattle monsterInBattle = effectTarget.GetComponent<MonsterInBattle>();

        Dictionary<string, string> cardData = monsterInBattle.cardData;

        Dictionary<string, object> parameter = new();

        parameter.Add("CardData", cardData);

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == effectTarget)
                {
                    parameter.Add("Player", battleProcess.systemPlayerData[i].perspectivePlayer);
                    break;
                }
            }
        }

        parameter.Add("EffectTarget", effectTarget);
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.MonsterInBattleToDeck, parameterNode2));
        //yield return null;
    }

    /// <summary>
    /// 判断是否被使用的是此卡，对方有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //怪兽
        if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                return false;
            }
        }

        return true;
    }
}
