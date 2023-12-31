using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 征募
/// 此卡使用后，获得%d点水晶。
/// </summary>
public class Draft : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            bool isAlly = false;
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    isAlly = true;
                }
            }

            if (systemPlayerData.consumeGameObject == gameObject)
            {
                isAlly = true;
            }

            if (isAlly)
            {
                Dictionary<string, object> parameter = new();
                parameter.Add("CrystalAmount", GetSkillValue());
                parameter.Add("Player", systemPlayerData.perspectivePlayer);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));


                if (gameObject.TryGetComponent<MonsterInBattle>(out var monsterInBattle))
                {
                    //Dictionary<string, object> parameter1 = new();
                    //parameter1.Add("LaunchedSkill", this);
                    //parameter1.Add("EffectName", "Effect1");
                    //parameter1.Add("SkillName", "draft");
                    //parameter1.Add("SkillValue", -GetSkillValue());
                    //parameter1.Add("Source", "Skill.Draft.Effect1");

                    //ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    //parameterNode2.parameter = parameter1;

                    //yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));

                    List<string> needRemoveSource = new();
                    foreach (KeyValuePair<string, int> keyValuePair in sourceAndValue)
                    {
                        needRemoveSource.Add(keyValuePair.Key);
                    }

                    foreach (var item in needRemoveSource)
                    {
                        Dictionary<string, object> parameter2 = new();
                        parameter2.Add("LaunchedSkill", this);
                        parameter2.Add("EffectName", "Effect1");
                        parameter2.Add("SkillName", "draft");
                        parameter2.Add("Source", item);

                        ParameterNode parameterNode2 = new();
                        parameterNode2.parameter = parameter2;

                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
                    }
                }

                yield break;
            }
        }
    }

    /// <summary>
    /// 判断是否被使用的是此卡
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

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
