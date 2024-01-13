using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏规则事件，所有需要时机才能发动的但不是技能的都在这
/// </summary>
public class RuleEvent : OpportunityEffect
{
    /// <summary>
    /// 本类实例
    /// </summary>
    private static RuleEvent instance;

    /// <summary>
    /// 获得本类实例
    /// </summary>
    /// <returns></returns>
    public static RuleEvent GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Find("BattleProcessSystem").GetComponent<RuleEvent>();
        }
        return instance;
    }

    /// <summary>
    /// 战斗前的初始化
    /// 开启接收对方消息的线程
    /// 确定先后手
    /// 传递双方卡组
    /// 加载手牌、英雄技能
    /// </summary>
    [TriggerEffect("^GameReady$")]
    public IEnumerator InitializeBothDeck(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Debug.Log("确定先后手，发送和接收卡组数据");

        //取出我方卡组
        Dictionary<string, object> allyDeckDictionary = new();
        string defaultDeckId = Database.cardMonster.Query("ConfigParameter", "and itemname='defalutDeckId'")[0]["itemvalue"];
        var deckList = Database.cardMonster.Query("PlayerDeck", "and DeckID='" + defaultDeckId + "'");
        if (deckList.Count == 0)
        {
            deckList = Database.cardMonster.Query("PlayerDeck", "");
        }

        Dictionary<string, string> allyDeck = deckList[0];
        string allyHeroSkillId = allyDeck["HeroSkillId"];
        string allyDeckCard = allyDeck["DeckCard"];

        Dictionary<string, List<string>> allyDeckCardD = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(allyDeckCard);
        List<string> allyMonster = allyDeckCardD["monster"];
        List<string> allyItem = allyDeckCardD["item"];

        Dictionary<string, List<string>> allyDeckCardSend = new();
        allyDeckCardSend.Add("Monster", allyMonster);
        allyDeckCardSend.Add("Item", allyItem);

        NetworkMessage enemyDeckMessage;
        List<string> enemyMonster;
        List<string> enemyItem;

        if (SocketTool.clientOrListening)
        {
            //初始化随机数种子 初始化随机数 发送随机数种子
            RandomUtils.seed = Environment.TickCount;
            RandomUtils.Init();

            Dictionary<string, object> randomSeedDictionary = new();
            randomSeedDictionary.Add("RandomSeed", RandomUtils.seed);

            NetworkMessage networkMessage = new();
            networkMessage.Type = NetworkMessageType.RandomSeed;
            networkMessage.Parameter = randomSeedDictionary;
            //Debug.Log(networkMessage.Parameter);

            SocketTool.SendMessage(networkMessage);

            yield return null;

            //接收对方卡组
            do
            {
                enemyDeckMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (enemyDeckMessage == null);

            yield return null;

            Debug.Log(enemyDeckMessage.Parameter["DeckCard"].GetType().Name);

            Dictionary<string, List<string>> enemyDeckDictionary = (Dictionary<string, List<string>>)enemyDeckMessage.Parameter["DeckCard"];
            enemyMonster = enemyDeckDictionary["Monster"];
            enemyItem = enemyDeckDictionary["Item"];

            //发送卡组
            allyDeckDictionary.Add("HeroSkillId", allyHeroSkillId);
            allyDeckDictionary.Add("DeckCard", allyDeckCardSend);

            NetworkMessage allyDeckMessage = new(NetworkMessageType.PriorityAndDeck, allyDeckDictionary);
            SocketTool.SendMessage(allyDeckMessage);

            yield return null;

            //确定先后手，0对方，1己方
            int priorityNumber = RandomUtils.GetRandomNumber(0, 1);
            battleProcess.nextTurn = priorityNumber == 1 ? Player.Ally : Player.Enemy;

            //洗牌
            for (int i = 0; i < 8; i++)
            {
                int a = RandomUtils.GetRandomNumber(i, 7);
                (enemyMonster[a], enemyMonster[i]) = (enemyMonster[i], enemyMonster[a]);
                a = RandomUtils.GetRandomNumber(i, 7);
                (enemyItem[a], enemyItem[i]) = (enemyItem[i], enemyItem[a]);
            }
            for (int i = 0; i < 8; i++)
            {
                int a = RandomUtils.GetRandomNumber(i, 7);
                (allyMonster[a], allyMonster[i]) = (allyMonster[i], allyMonster[a]);
                a = RandomUtils.GetRandomNumber(i, 7);
                (allyItem[a], allyItem[i]) = (allyItem[i], allyItem[a]);
            }

            //加载双方卡组和英雄技能
            //string a1 = "";
            //对方怪兽
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary3 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + enemyMonster[i] + "'")[0];
                //a1 += cardDictionary3["CardName"] + ";";

                Dictionary<string, object> parameter3 = new();
                parameter3.Add("LaunchedSkill", this);
                parameter3.Add("EffectName", "InitializeBothDeck");
                parameter3.Add("Player", Player.Enemy);
                parameter3.Add("CardData", cardDictionary3);

                ParameterNode parameterNode3 = parameterNode.AddNodeInMethod();
                parameterNode3.parameter = parameter3;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode3));
            }

            //对方道具
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary4 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + enemyItem[i] + "'")[0];
                //a1 += cardDictionary4["CardName"] + ";";

                Dictionary<string, object> parameter4 = new();
                parameter4.Add("LaunchedSkill", this);
                parameter4.Add("EffectName", "InitializeBothDeck");
                parameter4.Add("Player", Player.Enemy);
                parameter4.Add("CardData", cardDictionary4);

                ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
                parameterNode4.parameter = parameter4;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode4));
            }

            //我方怪兽
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary1 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + allyMonster[i] + "'")[0];
                //a1 += cardDictionary1["CardName"] + ";";

                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "InitializeBothDeck");
                parameter1.Add("Player", Player.Ally);
                parameter1.Add("CardData", cardDictionary1);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode1));
            }

            //我方道具
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary2 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + allyItem[i] + "'")[0];
                //a1 += cardDictionary2["CardName"] + ";";

                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "InitializeBothDeck");
                parameter2.Add("Player", Player.Ally);
                parameter2.Add("CardData", cardDictionary2);

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode2));
            }
            //Debug.Log(a1);

            //对方英雄技能，把技能类挂到物体上
            string enemyHeroSkillId = (string)enemyDeckMessage.Parameter["HeroSkillId"];

            GameObject enemyHeroSkill = battleProcess.enemyPlayerData.heroSkillGameObject;
            enemyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(enemyHeroSkillId);

            //我方英雄技能，把技能类挂到物体上
            GameObject allyHeroSkill = battleProcess.allyPlayerData.heroSkillGameObject;
            allyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(allyHeroSkillId);
        }
        else
        {
            //接收随机数种子
            //Debug.Log("接收随机数种子1");
            NetworkMessage networkMessage = null;
            do
            {
                networkMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (networkMessage == null || networkMessage.Type == NetworkMessageType.SendChallenge);

            RandomUtils.seed = (int)networkMessage.Parameter["RandomSeed"];
            RandomUtils.Init();

            yield return null;

            //Debug.Log("接收随机数种子2");

            //发送卡组
            allyDeckDictionary.Add("HeroSkillId", allyHeroSkillId);
            allyDeckDictionary.Add("DeckCard", allyDeckCardSend);

            NetworkMessage priorityNumberAndAllydeck = new(NetworkMessageType.PriorityAndDeck, allyDeckDictionary);
            SocketTool.SendMessage(priorityNumberAndAllydeck);

            yield return null;

            //接收对方卡组
            do
            {
                enemyDeckMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (enemyDeckMessage == null);

            yield return null;

            Dictionary<string, List<string>> enemyDeckDictionary = (Dictionary<string, List<string>>)enemyDeckMessage.Parameter["DeckCard"];
            enemyMonster = enemyDeckDictionary["Monster"];
            enemyItem = enemyDeckDictionary["Item"];

            //确定先后手，0己方，1对方
            int priorityNumber = RandomUtils.GetRandomNumber(0, 1);
            battleProcess.nextTurn = priorityNumber == 0 ? Player.Ally : Player.Enemy;

            //洗牌
            for (int i = 0; i < 8; i++)
            {
                int a = RandomUtils.GetRandomNumber(i, 7);
                (allyMonster[a], allyMonster[i]) = (allyMonster[i], allyMonster[a]);
                a = RandomUtils.GetRandomNumber(i, 7);
                (allyItem[a], allyItem[i]) = (allyItem[i], allyItem[a]);
            }
            for (int i = 0; i < 8; i++)
            {
                int a = RandomUtils.GetRandomNumber(i, 7);
                (enemyMonster[a], enemyMonster[i]) = (enemyMonster[i], enemyMonster[a]);
                a = RandomUtils.GetRandomNumber(i, 7);
                (enemyItem[a], enemyItem[i]) = (enemyItem[i], enemyItem[a]);
            }

            //加载双方卡组和英雄技能
            //string a1 = "";
            //我方怪兽
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary1 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + allyMonster[i] + "'")[0];
                //a1 += cardDictionary1["CardName"] + ";";

                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "InitializeBothDeck");
                parameter1.Add("Player", Player.Ally);
                parameter1.Add("CardData", cardDictionary1);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode1));
            }

            //我方道具
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary2 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + allyItem[i] + "'")[0];
                //a1 += cardDictionary2["CardName"] + ";";

                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "InitializeBothDeck");
                parameter2.Add("Player", Player.Ally);
                parameter2.Add("CardData", cardDictionary2);

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode2));
            }

            //对方怪兽
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary3 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + enemyMonster[i] + "'")[0];
                //a1 += cardDictionary3["CardName"] + ";";

                Dictionary<string, object> parameter3 = new();
                parameter3.Add("LaunchedSkill", this);
                parameter3.Add("EffectName", "InitializeBothDeck");
                parameter3.Add("Player", Player.Enemy);
                parameter3.Add("CardData", cardDictionary3);

                ParameterNode parameterNode3 = parameterNode.AddNodeInMethod();
                parameterNode3.parameter = parameter3;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode3));
            }

            //对方道具
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, string> cardDictionary4 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + enemyItem[i] + "'")[0];
                //a1 += cardDictionary4["CardName"] + ";";

                Dictionary<string, object> parameter4 = new();
                parameter4.Add("LaunchedSkill", this);
                parameter4.Add("EffectName", "InitializeBothDeck");
                parameter4.Add("Player", Player.Enemy);
                parameter4.Add("CardData", cardDictionary4);

                ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
                parameterNode4.parameter = parameter4;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode4));
            }
            //Debug.Log(a1);

            //我方英雄技能，把技能类挂到物体上
            GameObject allyHeroSkill = battleProcess.allyPlayerData.heroSkillGameObject;
            allyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(allyHeroSkillId);

            //对方英雄技能，把技能类挂到物体上
            string enemyHeroSkillId = (string)enemyDeckMessage.Parameter["HeroSkillId"];

            GameObject enemyHeroSkill = battleProcess.enemyPlayerData.heroSkillGameObject;
            enemyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(enemyHeroSkillId);
        }



        //string a2 = "";
        //foreach (var item in battleProcess.systemPlayerData[0].monsterDeck)
        //{
        //    a2 += item["CardName"] + ";";
        //}
        //Debug.Log(a2);
        //string a3 = "";
        //foreach (var item in battleProcess.systemPlayerData[0].itemDeck)
        //{
        //    a3 += item["CardName"] + ";";
        //}
        //Debug.Log(a3);
        //string a4 = "";
        //foreach (var item in battleProcess.systemPlayerData[1].monsterDeck)
        //{
        //    a4 += item["CardName"] + ";";
        //}
        //Debug.Log(a4);
        //string a5 = "";
        //foreach (var item in battleProcess.systemPlayerData[1].itemDeck)
        //{
        //    a5 += item["CardName"] + ";";
        //}
        //Debug.Log(a5);

        Debug.Log("RuleEvent.initializeBothDeck:加载手牌，执行完毕");
        yield return null;
    }

    /// <summary>
    /// 进入回合准备阶段
    /// 时机 Opportunity.EnterTurnReady
    /// </summary>
    [TriggerEffect("^InRoundReady$")]
    public IEnumerator InRoundReady(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //切换系统视角
        if (battleProcess.nextTurn == Player.Ally)
        {
            //Debug.Log("RuleEvent.EnterTurnReady:我方回合准备阶段开始");

            //回合指示动画
            GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("AllyRoundPrefab");
            GameObject battleSceneCanvas = GameObject.Find("BattleSceneCanvas");
            GameObject instance = Instantiate(prefab, battleSceneCanvas.transform);
            instance.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

            GameObject canvas = instance.transform.Find("Canvas").gameObject;
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            canvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            //切换系统视角
            battleProcess.allyPlayerData.perspectivePlayer = Player.Ally;
            battleProcess.enemyPlayerData.perspectivePlayer = Player.Enemy;

            //指定下个回合
            battleProcess.nextTurn = Player.Enemy;
        }
        else if (battleProcess.nextTurn == Player.Enemy)
        {
            //Debug.Log("RuleEvent.EnterTurnReady：对方回合准备阶段开始");

            GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("EnemyRoundPrefab");
            GameObject battleSceneCanvas = GameObject.Find("BattleSceneCanvas");
            GameObject instance = Instantiate(prefab, battleSceneCanvas.transform);
            instance.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

            GameObject canvas = instance.transform.Find("Canvas").gameObject;
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            canvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            battleProcess.allyPlayerData.perspectivePlayer = Player.Enemy;
            battleProcess.enemyPlayerData.perspectivePlayer = Player.Ally;

            battleProcess.nextTurn = Player.Ally;

            //开始处理对方消息
            battleProcess.StartCoroutine(HandleEnemyAction());
        }

        //加3水晶
        Dictionary<string, object> parameter1 = new();
        parameter1.Add("CrystalAmount", 3);
        parameter1.Add("Player", Player.Ally);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));
    }

    /// <summary>
    /// 在对方回合中，处理对方使用手牌、发动英雄技能、进入战斗阶段的消息
    /// </summary>
    private IEnumerator HandleEnemyAction()
    {
        PlayerAction playerAction = PlayerAction.GetInstance();
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        while (battleProcess.enemyPlayerData.perspectivePlayer == Player.Ally)
        {
            NetworkMessage networkMessage = SocketTool.GetNetworkMessage();
            if (networkMessage != null)
            {
                switch (networkMessage.Type)
                {
                    case NetworkMessageType.UseHandCard:
                        battleProcess.enemyPlayerData.canUseHandCard = false;

                        ParameterNode parameterNode1 = new();
                        parameterNode1.opportunity = "CheckCardTarget";
                        parameterNode1.parameter = networkMessage.Parameter;

                        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
                        break;
                    case NetworkMessageType.SacrificeCard:
                        battleProcess.enemyPlayerData.canUseHandCard = false;

                        ParameterNode parameterNode2 = new();
                        parameterNode2.opportunity = "CheckSacrifice";
                        parameterNode2.parameter = networkMessage.Parameter;

                        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode2));
                        break;
                    case NetworkMessageType.LaunchHeroSkill:
                        battleProcess.enemyPlayerData.canUseHandCard = false;

                        ParameterNode parameterNode3 = new();
                        parameterNode3.opportunity = "LaunchHeroSkill";
                        parameterNode3.parameter = networkMessage.Parameter;

                        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode3));
                        break;
                    case NetworkMessageType.StartAttackPhase:
                        battleProcess.enemyPlayerData.canUseHandCard = false;
                        battleProcess.enemyPlayerData.canSacrifice = false;

                        yield return battleProcess.StartCoroutine(playerAction.DoAction(playerAction.StartRoundBattle, new()));
                        break;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 死斗模式
    /// </summary>
    [TriggerEffect(@"^InRoundBattle$", "Compare1")]
    public IEnumerator FightToTheDeath(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == Player.Ally)
            {
                int roundNumber = playerData.roundNumber;
                int damageValue = roundNumber - 10;

                //伤害参数
                Dictionary<string, object> damageParameter = new();
                //当前技能
                damageParameter.Add("LaunchedSkill", this);
                //效果名称
                damageParameter.Add("EffectName", "Effect1");
                //受到伤害的怪兽
                damageParameter.Add("EffectTarget", playerData.monsterGameObjectArray[0]);
                //伤害数值
                damageParameter.Add("DamageValue", damageValue);
                //伤害类型
                damageParameter.Add("DamageType", DamageType.Real);

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode2));
                yield return null;
            }
        }
    }

    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == Player.Ally && playerData.roundNumber > 10 && playerData.monsterGameObjectArray[0] != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 触发手牌中的“额外水晶”
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^Before\.GameAction\.Sacrifice$", "Compare2")]
    public IEnumerator Crystal(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        foreach (PlayerData playerData in battleProcess.systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                Dictionary<string, string> handCard = null;
                switch (objectBeSacrificedNumber)
                {
                    case 0:
                        handCard = playerData.handMonster[0];
                        break;
                    case 1:
                        handCard = playerData.handMonster[1];
                        break;
                    case 2:
                        handCard = playerData.handItem[0];
                        break;
                    case 3:
                        handCard = playerData.handItem[1];
                        break;
                }

                string cardP = handCard["CardSkill"];
                Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);

                int crystalAmount = Convert.ToInt32(pd["crystal"]);

                Dictionary<string, object> parameter1 = new();
                parameter1.Add("CrystalAmount", crystalAmount);
                parameter1.Add("Player", player);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 判断是否是手牌，且拥有“额外水晶”
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber <= 3)
        {
            foreach (PlayerData playerData in battleProcess.systemPlayerData)
            {
                if (playerData.perspectivePlayer == player)
                {
                    Dictionary<string, string> handCard = null;
                    switch (objectBeSacrificedNumber)
                    {
                        case 0:
                            handCard = playerData.handMonster[0];
                            break;
                        case 1:
                            handCard = playerData.handMonster[1];
                            break;
                        case 2:
                            handCard = playerData.handItem[0];
                            break;
                        case 3:
                            handCard = playerData.handItem[1];
                            break;
                    }

                    if (handCard != null)
                    {
                        string cardP = handCard["CardSkill"];
                        if (cardP != null && !cardP.Equals(""))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("crystal") && Convert.ToInt32(pd["crystal"]) > 0)
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 使用一张卡牌
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^CheckCardTarget$", "Compare3")]
    public IEnumerator CardBeUsed(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameterNode.parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.UseACard, parameterNode1));
        yield return null;
    }

    /// <summary>
    /// 判断是否满足使用手牌条件
    /// 消耗品可以对任意目标使用
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare3(ParameterNode parameterNode)
    {
        //Debug.Log("进入RuleEvent.CardBeUsed的比较---------");
        Dictionary<string, object> parameter = parameterNode.parameter;
        //使用手牌的玩家
        Player player = (Player)parameter["Player"];
        //目标场上位置，012
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        //手牌目标玩家
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        //手牌位置，01怪兽，23道具
        int handPanelNumber = (int)parameter["HandPanelNumber"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        Dictionary<string, string> cardData = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                cardData = handPanelNumber < 2 ? playerData.handMonster[handPanelNumber] : playerData.handItem[handPanelNumber - 2];
            }
        }

        if (cardData == null)
        {
            return false;
        }

        //获取使用手牌的玩家的PlayerData
        PlayerData useCardPlayerData = null;
        PlayerData targetPlayerData = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer == player)
            {
                useCardPlayerData = systemPlayerData;
            }

            if (systemPlayerData.perspectivePlayer == targetPlayer)
            {
                targetPlayerData = systemPlayerData;
            }
        }

        Dictionary<string, string> cardKind = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardData["CardKind"]);

        //判断费用够不够
        if (useCardPlayerData.surplusCrystal < Convert.ToInt32(cardData["CardCost"]))
        {
            Debug.Log("费用不够");
            return false;
        }

        //怪兽
        if (cardData["CardType"].Equals("monster"))
        {
            //是不是给自己使用
            if (targetPlayer != player)
            {
                Debug.Log("不是给自己使用");
                return false;
            }

            //场上有没有空位
            if (targetPlayerData.monsterGameObjectArray[2] != null)
            {
                Debug.Log("场上有没有空位");
                return false;
            }

            //上一个位置不是空位
            if (battlePanelNumber != 0 && targetPlayerData.monsterGameObjectArray[battlePanelNumber - 1] == null)
            {
                Debug.Log("上一个位置不是空位");
                return false;
            }
        }

        //对于装备和消耗品，目标不是空
        GameObject targetMonster = targetPlayerData.monsterGameObjectArray[battlePanelNumber];
        if ((cardData["CardType"].Equals("equip") || cardData["CardType"].Equals("consume")) && targetMonster == null)
        {
            Debug.Log("对于装备和消耗品，目标不是空");
            return false;
        }

        //装备
        if (cardData["CardType"].Equals("equip"))
        {
            //判断是否是给我方怪兽
            if (targetPlayer != player)
            {
                Debug.Log("判断是否是给我方怪兽");
                return false;
            }

            //判断颜色
            MonsterInBattle monsterInBattle = targetMonster.GetComponent<MonsterInBattle>();
            string monsterKind = monsterInBattle.kind;
            bool hasSameKind = false;
            foreach (var item in cardKind)
            {
                if (monsterKind == "all" || item.Value == "all" || monsterKind == item.Value)
                {
                    hasSameKind = true;
                }
            }
            if (!hasSameKind)
            {
                Debug.Log("判断颜色");
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// 献祭一张卡牌
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^CheckSacrifice$", "Compare4")]
    public IEnumerator CardBeSacrificed(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameterNode.parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.Sacrifice, parameterNode1));
        yield return null;
    }

    public bool Compare4(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            //Debug.Log(systemPlayerData.perspectivePlayer + "----" + systemPlayerData.canSacrifice);
            if (systemPlayerData.perspectivePlayer == player && systemPlayerData.canSacrifice)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 护甲为0消灭装备
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill", "Compare5")]
    public IEnumerator DestoryEquipmentForNoArmor(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.Parent.creator;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        GameObject targetMonster = monsterInBattle.gameObject;

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "DestoryEquipmentForNoArmor");
        parameter1.Add("EffectTarget", targetMonster);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyEquipment, parameterNode1));
        yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽，技能是护甲，值小于1
    /// </summary>
    public bool Compare5(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        return skillName == "armor" && !monsterInBattle.gameObject.TryGetComponent(out Armor _);
    }

    /// <summary>
    /// 开始战斗时，场上没有怪兽，消灭一张卡
    /// </summary>
    [TriggerEffect(@"^InRoundBattle$", "Compare6")]
    public IEnumerator NegativePunishment(ParameterNode parameterNode)
    {
        //Debug.Log("开始战斗时，场上没有怪兽，消灭一张卡");

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == Player.Ally)
            {
                string color = playerData.actualPlayer == Player.Ally ? "#00ff00" : "#ff0000";
                string playerC = playerData.actualPlayer == Player.Ally ? "我方" : "敌方";
                battleProcess.Log($"<color={color}>{playerC}</color>受到消极惩罚");

                List<Dictionary<string, string>> monsterDeck = playerData.monsterDeck;
                List<Dictionary<string, string>> itemDeck = playerData.itemDeck;
                if (monsterDeck.Count > 0 && itemDeck.Count > 0)
                {
                    int r = RandomUtils.GetRandomNumber(0, 1);
                    if (r == 0)
                    {
                        Dictionary<string, object> parameter1 = new();
                        parameter1.Add("Type", "monster");
                        parameter1.Add("Player", Player.Ally);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = parameter1;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyADeckCard, parameterNode1));
                        yield return null;
                    }
                    else
                    {
                        Dictionary<string, object> parameter1 = new();
                        parameter1.Add("Type", "item");
                        parameter1.Add("Player", Player.Ally);

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
                    parameter1.Add("Player", Player.Ally);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyADeckCard, parameterNode1));
                    yield return null;
                }
                else if (monsterDeck.Count == 0 && itemDeck.Count > 0)
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("Type", "item");
                    parameter1.Add("Player", Player.Ally);

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
                    destroyAHandCardParameter.Add("Player", Player.Ally);

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

    /// <summary>
    /// 场上没有怪兽
    /// </summary>
    public bool Compare6(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == Player.Ally && playerData.monsterGameObjectArray[0] == null)
            {
                return true;
            }
        }
        return false;
    }

}