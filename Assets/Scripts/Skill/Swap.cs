using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 恐惧
/// </summary>
public class Swap : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //对方玩家
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        Dictionary<string, object> parameter = new();
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect1");

        if (oppositePlayerMessage.monsterGameObjectArray[2] == null)
        {
            parameter.Add("MonsterMove1", oppositePlayerMessage.monsterGameObjectArray[0]);
            parameter.Add("MonsterMove2", oppositePlayerMessage.monsterGameObjectArray[1]);
        }
        else
        {
            int r = RandomUtils.GetRandomNumber(1, 2);
            parameter.Add("MonsterMove1", oppositePlayerMessage.monsterGameObjectArray[0]);
            parameter.Add("MonsterMove2", oppositePlayerMessage.monsterGameObjectArray[r]);
        }

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.SwapMonsterPosition, parameterNode1));

        MonsterInBattle monsterInBattle1 = gameObject.GetComponent<MonsterInBattle>();

        List<string> needRemoveSource = new();
        foreach (KeyValuePair<string, int> keyValuePair in sourceAndValue)
        {
            needRemoveSource.Add(keyValuePair.Key);
        }

        foreach (var item in needRemoveSource)
        {
            Dictionary<string, object> parameter4 = new();
            parameter4.Add("LaunchedSkill", this);
            parameter4.Add("EffectName", "Effect1");
            parameter4.Add("SkillName", "swap");
            parameter4.Add("Source", item);

            ParameterNode parameterNode4 = new();
            parameterNode4.parameter = parameter4;

            yield return battleProcess.StartCoroutine(monsterInBattle1.DoAction(monsterInBattle1.DeleteSkillSource, parameterNode4));
        }
    }

    /// <summary>
    /// 判断是否被使用的是此卡，对方有至少两个怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //消耗品物体
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        //怪兽
        else if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        //装备
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
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
            if (systemPlayerData.perspectivePlayer != player && systemPlayerData.monsterGameObjectArray[1] == null)
            {
                return false;
            }
        }

        return true;
    }
}