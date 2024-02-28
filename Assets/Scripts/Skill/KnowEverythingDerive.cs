using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 万物通（衍生）
/// 本回合中，我方使用的下一张消耗品获得<征募%s>，并且会在相同的位置额外生效1次。
/// </summary>
public class KnowEverythingDerive : SkillInBattle
{
    /// <summary>
    /// 征募点数
    /// </summary>
    int launchMark1 = 0;
    /// <summary>
    /// 额外生效次数
    /// </summary>
    int launchMark2 = 0;
    /// <summary>
    /// 记录卡牌数据
    /// </summary>
    ParameterNode parameterNodeRecord;

    public override int AddValue(string source, int value)
    {
        launchMark1 += value;
        launchMark2++;

        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] += value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    [TriggerEffect(@"^Before\.GameAction\.ConsumeEnterBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];

        string cardSkill = cardData["CardSkill"];
        Dictionary<string, int> dic = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardSkill);

        if (dic.ContainsKey("draft"))
        {
            dic["draft"] += launchMark1;
        }
        else
        {
            dic.Add("draft", launchMark1);
        }

        cardData["CardSkill"] = JsonConvert.SerializeObject(dic);

        parameterNodeRecord = parameterNode;

        launchMark1 = 0;

        yield break;
    }

    /// <summary>
    /// 判断是己方的消耗品进场
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchMark1 < 1)
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.skillList.Count; j++)
                {
                    SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                    if (skillInBattle == this)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [TriggerEffect(@"^Before\.GameAction\.ConsumeLeave$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        int l = launchMark2;
        launchMark2 = 0;

        for (int j = 0; j < l; j++)
        {
            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                //询问消耗品
                GameObject consumeGameObject = battleProcess.systemPlayerData[i].consumeGameObject;
                if (consumeGameObject != null)
                {
                    //重新生成消耗品上的技能
                    //Debug.Log("重新生成消耗品上的技能");
                    ConsumeInBattle consumeInBattle = consumeGameObject.GetComponent<ConsumeInBattle>();
                    foreach (var item in consumeInBattle.skillList)
                    {
                        Destroy(item);
                    }
                    consumeInBattle.skillList = new();

                    yield return consumeInBattle.Generate(consumeInBattle.cardData);

                    //重新设定目标
                    ParameterNode parameterNode2 = parameterNodeRecord.Parent.Parent.superiorNode.Parent.AfterChild;

                    int battlePanelNumber = (int)parameterNode2.Parent.parameter["BattlePanelNumber"];
                    parameterNode2.Parent.EffectChild.nodeInMethodList[1].EffectChild.result["ConsumeTarget"] = battleProcess.systemPlayerData[(i + 1) % 2].monsterGameObjectArray[battlePanelNumber];

                    yield return battleProcess.StartCoroutine(consumeInBattle.LaunchSkill(parameterNode2));
                }
            }
        }
    }

    /// <summary>
    /// 判断是己方的消耗品离场
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchMark2 < 1)
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.skillList.Count; j++)
                {
                    SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                    if (skillInBattle == this)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [TriggerEffect("^AfterRoundBattle$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.skillList.Count; j++)
            {
                SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                if (skillInBattle == this)
                {
                    systemPlayerData.skillList.Remove(skillInBattle);
                    Destroy(this);
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是己方回合
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.skillList.Count; j++)
            {
                SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                if (skillInBattle == this)
                {
                    return true;
                }
            }
        }

        return false;
    }
}