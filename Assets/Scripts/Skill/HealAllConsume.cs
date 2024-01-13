using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 群体治疗（消耗品）
/// 使用后，使所有己方怪兽恢复%d点生命值
/// </summary>
public class HealAllConsume : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 2; j >= 0; j--)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] != null)
                    {
                        //治疗
                        Dictionary<string, object> treatParameter = new();
                        //当前技能
                        treatParameter.Add("LaunchedSkill", this);
                        //效果名称
                        treatParameter.Add("EffectName", "Effect1");
                        //受到治疗的怪兽
                        treatParameter.Add("EffectTarget", systemPlayerData.monsterGameObjectArray[j]);
                        //治疗数值
                        treatParameter.Add("TreatValue", GetSkillValue());

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = treatParameter;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, parameterNode1));
                        //yield return null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 判断是否被使用的是此卡、我方有没有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //使用手牌的玩家
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
                Debug.Log("群体侵染判断2");
                return false;
            }
        }
        //装备
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                Debug.Log("群体侵染判断3");
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
            if (systemPlayerData.perspectivePlayer == player && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                return false;
            }
        }

        return true;
    }
}
