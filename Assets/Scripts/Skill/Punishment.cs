using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 惩罚
/// 使用后，若献祭点数量大于等于3、场上有除此卡以外的怪兽，则消耗三个献祭点，随机把除此卡以外的场上三只怪兽的费用-%d
/// </summary>
public class Punishment : SkillInBattle
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

        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                GameObject go = systemPlayerData.monsterGameObjectArray[j];
                if (go == gameObject)
                {
                    int sacrificeNumber = (int)systemPlayerData.marker["SacrificeNumber"];

                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("Player", systemPlayerData.perspectivePlayer);
                    parameter1.Add("EffectKey", "SacrificeNumber");
                    parameter1.Add("EffectValue", sacrificeNumber - 3);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangePlayerDataMarker, parameterNode1));
                    yield return null;
                }
                if (go != null && go != gameObject)
                {
                    gameObjects.Add(go);
                }
            }
            if (systemPlayerData.consumeGameObject == gameObject)
            {
                int sacrificeNumber = (int)systemPlayerData.marker["SacrificeNumber"];

                Dictionary<string, object> parameter1 = new();
                parameter1.Add("Player", systemPlayerData.perspectivePlayer);
                parameter1.Add("EffectKey", "SacrificeNumber");
                parameter1.Add("EffectValue", sacrificeNumber - 3);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangePlayerDataMarker, parameterNode1));
                //yield return null;
            }
        }

        //if (gameObjects.Count > 3)
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        int r = RandomUtils.GetRandomNumber(i, gameObjects.Count - 1);
        //        (gameObjects[i], gameObjects[r]) = (gameObjects[r], gameObjects[i]);
        //    }
        //}

        //for (int i = 0; i < gameObjects.Count && i < 3; i++)
        //{
        //    Dictionary<string, object> parameter1 = new();
        //    parameter1.Add("LaunchedSkill", this);
        //    parameter1.Add("EffectName", "Effect1");
        //    parameter1.Add("EffectTarget", gameObjects[i]);
        //    parameter1.Add("EffectValue", -launchMark);

        //    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        //    parameterNode1.parameter = parameter1;

        //    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeMonsterCost, parameterNode1));
        //    //yield return null;
        //}

        launchMark = 0;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、献祭点大于等于3、场上有除此卡以外的怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

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

        bool hasMonster = false;
        bool enoughSacrificeNumber = false;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                GameObject go = systemPlayerData.monsterGameObjectArray[j];
                if (go == gameObject)
                {
                    if (systemPlayerData.marker.ContainsKey("SacrificeNumber") && (int)systemPlayerData.marker["SacrificeNumber"] > 2)
                    {
                        enoughSacrificeNumber = true;
                    }
                }
                if (go != null && go != gameObject)
                {
                    hasMonster = true;
                }
            }
            if (systemPlayerData.consumeGameObject == gameObject)
            {
                if (systemPlayerData.marker.ContainsKey("SacrificeNumber") && (int)systemPlayerData.marker["SacrificeNumber"] > 2)
                {
                    enoughSacrificeNumber = true;
                }
            }
        }

        return hasMonster && enoughSacrificeNumber;
    }
}