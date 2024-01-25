using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 沉默
/// 使用时，所有敌人于%d回合内获得“沉默”效果（<魔法><治疗><群体治疗>失效，可被<净化><魔力增幅>清除）
/// </summary>
public class Silence : SkillInBattle
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
            if (systemPlayerData.perspectivePlayer != player)
            {
                for (int j = 2; j >= 0; j--)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] != null)
                    {
                        MonsterInBattle monsterInBattle = systemPlayerData.monsterGameObjectArray[j].GetComponent<MonsterInBattle>();

                        Dictionary<string, object> parameter1 = new();
                        parameter1.Add("LaunchedSkill", this);
                        parameter1.Add("EffectName", "Effect1");
                        parameter1.Add("SkillName", "silence_derive");
                        parameter1.Add("SkillValue", launchMark);
                        parameter1.Add("Source", "Skill.Silence.Effect1");

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = parameter1;

                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                    }
                }
            }
        }

        launchMark = 0;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、对方有没有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //使用手牌的玩家
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

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

        return true;
    }
}
