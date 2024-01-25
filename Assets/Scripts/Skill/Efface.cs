using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抹消
/// 攻击时，若场上无敌人，随机移除敌方牌库中的%d张牌（对局首轮或非对战模式下无效）
/// </summary>
public class Efface : SkillInBattle
{
    [TriggerEffect("^InRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == Player.Enemy)
            {
                List<Dictionary<string, string>> monsterDeck = playerData.monsterDeck;
                List<Dictionary<string, string>> itemDeck = playerData.itemDeck;

                for (int j = 0; j < GetSkillValue(); j++)
                {
                    if (monsterDeck.Count > 0 && itemDeck.Count > 0)
                    {
                        int r = RandomUtils.GetRandomNumber(0, 1);
                        if (r == 0)
                        {
                            Dictionary<string, object> parameter1 = new();
                            parameter1.Add("Type", "monster");
                            parameter1.Add("Player", Player.Enemy);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter1;

                            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyADeckCard, parameterNode1));
                            yield return null;
                        }
                        else
                        {
                            Dictionary<string, object> parameter1 = new();
                            parameter1.Add("Type", "item");
                            parameter1.Add("Player", Player.Enemy);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter1;

                            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyADeckCard, parameterNode1));
                            yield return null;
                        }
                    }
                    else if (monsterDeck.Count > 0 && itemDeck.Count == 0)
                    {
                        Dictionary<string, object> parameter1 = new();
                        parameter1.Add("Type", "monster");
                        parameter1.Add("Player", Player.Enemy);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = parameter1;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyADeckCard, parameterNode1));
                        yield return null;
                    }
                    else if (monsterDeck.Count == 0 && itemDeck.Count > 0)
                    {
                        Dictionary<string, object> parameter1 = new();
                        parameter1.Add("Type", "item");
                        parameter1.Add("Player", Player.Enemy);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = parameter1;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyADeckCard, parameterNode1));
                        yield return null;
                    }
                    else
                    {
                        Dictionary<string, string>[] handMonster = playerData.handMonster;
                        Dictionary<string, string>[] handItem = playerData.handItem;

                        Dictionary<string, object> destroyAHandCardParameter = new();
                        destroyAHandCardParameter.Add("Player", Player.Enemy);

                        if (handItem[1] != null)
                        {
                            destroyAHandCardParameter.Add("HandPanelNumber", 3);
                        }
                        else if (handItem[0] != null)
                        {
                            destroyAHandCardParameter.Add("HandPanelNumber", 2);
                        }
                        else if (handMonster[1] != null)
                        {
                            destroyAHandCardParameter.Add("HandPanelNumber", 1);
                        }
                        else if (handMonster[0] != null)
                        {
                            destroyAHandCardParameter.Add("HandPanelNumber", 0);
                        }

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = destroyAHandCardParameter;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyAHandCard, parameterNode1));
                        yield return null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是己方回合，对方场上没有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (battleProcess.systemPlayerData[0].roundNumber < 1 || battleProcess.systemPlayerData[1].roundNumber < 1)
        {
            return false;
        }

        bool isAlly = false;
        bool enemyHasMonster = false;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject && systemPlayerData.roundNumber > 0)
                    {
                        isAlly = true;
                    }
                }
            }

            if (systemPlayerData.perspectivePlayer == Player.Enemy && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                enemyHasMonster = true;
            }
        }

        return isAlly && enemyHasMonster;
    }
}