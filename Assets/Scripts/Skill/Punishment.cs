using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ͷ�
/// ʹ�ú����׼����������ڵ���3�������г��˿�����Ĺ��ޣ������������׼��㣬����ѳ��˿�����ĳ�����ֻ���޵ķ���-%d
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
    /// �ж��Ƿ�ʹ�õ��Ǵ˿����׼�����ڵ���3�������г��˿�����Ĺ���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchMark < 1)
        {
            return false;
        }

        //����Ʒ����
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        //����
        else if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        //װ��
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