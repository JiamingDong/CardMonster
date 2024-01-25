using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔力增幅
/// 使用时，所有盟友的<魔法>值+%d直到回合结束，清除所有盟友的“沉默”状态
/// </summary>
public class MagicOutburst : SkillInBattle
{
    int launchMark = 0;

    public override int AddValue(string source, int value)
    {
        launchMark = value;

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
                    GameObject go = systemPlayerData.monsterGameObjectArray[j];
                    if (go != null)
                    {
                        MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();

                        if (go.TryGetComponent<SilenceDerive>(out var silenceDerive))
                        {
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
                                parameter4.Add("SkillName", "silence_derive");
                                parameter4.Add("Source", item);

                                ParameterNode parameterNode4 = new();
                                parameterNode4.parameter = parameter4;

                                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode4));
                            }
                        }

                        if (go.TryGetComponent<Magic>(out var magic))
                        {
                            Dictionary<string, object> parameter1 = new();
                            parameter1.Add("LaunchedSkill", this);
                            parameter1.Add("EffectName", "Effect1");
                            parameter1.Add("SkillName", "magic");
                            parameter1.Add("SkillValue", launchMark);
                            parameter1.Add("Source", "Skill.MagicOutburst.Effect1");

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter1;

                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("LaunchedSkill", this);
                            parameter2.Add("EffectName", "Effect1");
                            parameter2.Add("SkillName", "magic_outburst_derive");
                            parameter2.Add("SkillValue", 0);
                            parameter2.Add("Source", "Skill.MagicOutburst.Effect1");

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
                        }
                    }
                }
            }
        }

        launchMark = 0;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、我方有没有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        Debug.Log("launchMark2=" + launchMark);
        if (launchMark < 1)
        {
            return false;
        }

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
            if (systemPlayerData.perspectivePlayer == player && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                return false;
            }
        }

        return true;
    }
}
