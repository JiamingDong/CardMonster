using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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
            instance = GameObject.Find("BattleProcessSystem").GetComponent<RuleEvent>();
        return instance;
    }

    void Start()
    {
        effectList.Add(InitializeBothDeck);
        effectList.Add(EnterRoundReady);
        effectList.Add(Crystal);
    }

    /// <summary>
    /// 战斗前的初始化
    /// 开启接收对方消息的线程
    /// 确定先后手
    /// 传递双方卡组
    /// 加载手牌、英雄技能
    /// </summary>
    [TriggerEffectCondition("GameReady")]
    public IEnumerator InitializeBothDeck(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //开启接收对方消息的线程
        SocketTool.acceptMessageThread.Start();
        Debug.Log("RuleEvent.initializeBothDeck:开启接收对方消息的线程");
        yield return null;

        //传递双方卡组
        Dictionary<string, object> priorityNumberAndAllyDeck = new();
        string defaultDeckId = Database.cardMonster.Query("PlayerData", "and PlayerID='1'")[0]["DefaultDeckID"];
        var deckList = Database.cardMonster.Query("PlayerDeck", "and DeckID='" + defaultDeckId + "'");
        if (deckList.Count == 0)
        {
            deckList = Database.cardMonster.Query("PlayerDeck", "");
        }
        Dictionary<string, string> allyDeck = deckList[0];

        //洗牌
        string allyDeckCard = allyDeck["DeckCard"];
        Dictionary<string, object> allyDeckCardD = JsonConvert.DeserializeObject<Dictionary<string, object>>(allyDeckCard);
        List<string> allyMonster = ((JArray)allyDeckCardD["monster"]).ToObject<List<string>>();
        Debug.Log("allyMonster=");
        Debug.Log(allyMonster);
        List<string> allyItem = ((JArray)allyDeckCardD["item"]).ToObject<List<string>>();

        System.Random random = new();
        for (int i = 0; i < 8; i++)
        {
            int a = random.Next(i, 8);
            (allyMonster[a], allyMonster[i]) = (allyMonster[i], allyMonster[a]);
            a = random.Next(i, 8);
            (allyItem[a], allyItem[i]) = (allyItem[i], allyItem[a]);
        }

        Dictionary<string, object> allyDeckCardSend = new()
        {
            { "Monster", allyMonster },
            { "Item", allyItem }
        };
        priorityNumberAndAllyDeck.Add("DeckCard", allyDeckCardSend);

        string allyHeroSkillID = allyDeck["HeroSkillID"];
        priorityNumberAndAllyDeck.Add("HeroSkillID", allyHeroSkillID);
        Debug.Log("RuleEvent.initializeBothDeck:我方卡组已添加");
        yield return null;

        NetworkMessage enemyDeckMessage;
        if (SocketTool.clientOrListening)
        {
            //确定先后手，0己方，1对方
            int priorityNumber = new System.Random().Next(2);
            string priorityString = priorityNumber.ToString();
            priorityNumberAndAllyDeck.Add("Priority", priorityString);
            battleProcess.nextTurn = priorityNumber == 0 ? Player.Ally : Player.Enemy;

            NetworkMessage priorityNumberAndAllydeck = new(NetworkMessageType.PriorityAndDeck, priorityNumberAndAllyDeck);
            SocketTool.SendMessage(priorityNumberAndAllydeck);
            do
            {
                enemyDeckMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (enemyDeckMessage == null);
        }
        else
        {
            do
            {
                enemyDeckMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (enemyDeckMessage == null);
            NetworkMessage priorityNumberAndAllydeck = new(NetworkMessageType.PriorityAndDeck, priorityNumberAndAllyDeck);
            SocketTool.SendMessage(priorityNumberAndAllydeck);

            battleProcess.nextTurn = enemyDeckMessage.Parameter["Priority"].Equals("1") ? Player.Ally : Player.Enemy;
        }

        Debug.Log("RuleEvent.initializeBothDeck:确定先后手，发送和接收卡组数据");
        yield return null;

        //加载双方卡组和英雄技能
        Dictionary<string, object> enemyCardD = (Dictionary<string, object>)enemyDeckMessage.Parameter["DeckCard"];
        List<string> enemyMonster = (List<string>)enemyCardD["Monster"];
        List<string> enemyItem = (List<string>)enemyCardD["Item"];

        for (int i = 0; i < 8; i++)
        {
            //己方卡组
            Dictionary<string, string> cardDictionary1 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + allyMonster[i] + "'")[0];
            battleProcess.allyPlayerData.monsterDeck.Add(cardDictionary1);
            Dictionary<string, string> cardDictionary2 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + allyItem[i] + "'")[0];
            battleProcess.allyPlayerData.itemDeck.Add(cardDictionary2);

            //对方卡组
            Dictionary<string, string> cardDictionary3 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + enemyMonster[i] + "'")[0];
            battleProcess.enemyPlayerData.monsterDeck.Add(cardDictionary3);
            Dictionary<string, string> cardDictionary4 = Database.cardMonster.Query("AllCardConfig", "and CardID='" + enemyItem[i] + "'")[0];
            battleProcess.enemyPlayerData.itemDeck.Add(cardDictionary4);
        }

        //我方英雄技能，把技能类挂到物体上
        GameObject allyHeroSkill = battleProcess.allyPlayerData.heroSkillGameObject;
        allyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(allyHeroSkillID);

        //对方英雄技能，把技能类挂到物体上
        string enemyHeroSkillID = (string)enemyDeckMessage.Parameter["HeroSkillID"];

        GameObject enemyHeroSkill = battleProcess.enemyPlayerData.heroSkillGameObject;
        enemyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(enemyHeroSkillID);

        Debug.Log("RuleEvent.initializeBothDeck:加载双方卡组和英雄技能");
        yield return null;

        //加载手牌
        for (int j = 0; j < battleProcess.systemPlayerData.Length; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                Dictionary<string, object> parameter1 = new();
                parameter1.Add("HandPanelNumber", i);
                parameter1.Add("PlayerData", battleProcess.systemPlayerData[j]);
                yield return StartCoroutine(gameAction.DoAction(gameAction.SupplyAHandCard, parameter));
            }
        }

        Debug.Log("RuleEvent.initializeBothDeck:加载手牌，执行完毕");
    }

    /// <summary>
    /// 进入回合准备阶段
    /// 时机 Opportunity.EnterTurnReady
    /// </summary>
    [TriggerEffectCondition("EnterRoundReady")]
    public IEnumerator EnterRoundReady(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //切换系统视角
        battleProcess.SwitchSystemPerspective();

        battleProcess.gamePhase = GamePhase.RoundReady;

        //指定下个回合是哪个玩家的
        if (battleProcess.allyPlayerData.perspectivePlayer == Player.Ally)
        {
            Debug.Log("RuleEvent.EnterTurnReady:我方回合准备阶段开始");

            battleProcess.nextTurn = Player.Enemy;
        }
        else if (battleProcess.enemyPlayerData.perspectivePlayer == Player.Ally)
        {
            Debug.Log("RuleEvent.EnterTurnReady：对方回合准备阶段开始");

            battleProcess.nextTurn = Player.Ally;
        }

        //在准备阶段
        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "InRoundReady";

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        //加3水晶
        Dictionary<string, object> parameter1 = new();

        parameter1.Add("CrystalAmount", 3);
        parameter1.Add("Player", Player.Ally);

        yield return StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameter1));

        battleProcess.gamePhase = GamePhase.InRound;

        if (battleProcess.allyPlayerData.perspectivePlayer == Player.Ally)
        {
            GameObject allyStartBattleButtonImage = GameObject.Find("AllyStartBattleButtonImage");
            RawImage rawImage = allyStartBattleButtonImage.GetComponent<RawImage>();
            rawImage.texture = Resources.Load<Texture2D>("Image/AllyStartBattleButton");

            GameObject sacrificeButtonImage = GameObject.Find("SacrificeButtonImage");
            RawImage sacrificeButtonRawImage = sacrificeButtonImage.GetComponent<RawImage>();
            sacrificeButtonRawImage.texture = Resources.Load<Texture2D>("Image/SacrificeEnableButton");

            //可以使用手牌
            battleProcess.allyPlayerData.canUseHandCard = true;

            Debug.Log("RuleEvent.EnterTurnReady：我方回合准备阶段结束");
        }
        else if (battleProcess.enemyPlayerData.perspectivePlayer == Player.Ally)
        {
            //开启接收对方消息的协程
            StartCoroutine(AcceptEnemyInTurnMessage());

            GameObject allyStartBattleButtonImage = GameObject.Find("AllyStartBattleButtonImage");
            RawImage rawImage = allyStartBattleButtonImage.GetComponent<RawImage>();

            rawImage.texture = Resources.Load<Texture2D>("Image/EnemyStartBattleButton");

            GameObject sacrificeButtonImage = GameObject.Find("SacrificeButtonImage");
            RawImage sacrificeButtonRawImage = sacrificeButtonImage.GetComponent<RawImage>();
            sacrificeButtonRawImage.texture = Resources.Load<Texture2D>("Image/SacrificeDisableButton");

            //可以使用手牌
            battleProcess.enemyPlayerData.canUseHandCard = true;

            Debug.Log("RuleEvent.EnterTurnReady：对方回合准备阶段结束");
        }
    }

    /// <summary>
    /// 在对方回合中，接收对方使用手牌、发动英雄技能、进入战斗阶段的消息
    /// </summary>
    private IEnumerator AcceptEnemyInTurnMessage()
    {
        PlayerAction playerAction = PlayerAction.GetInstance();
        BattleProcess battleProcess = BattleProcess.GetInstance();

        while (battleProcess.enemyPlayerData.perspectivePlayer == Player.Ally)
        {
            NetworkMessage networkMessage = SocketTool.GetNetworkMessage();
            if (networkMessage != null)
            {
                Dictionary<string, object> parameter = new();

                //对方使用手牌
                if (networkMessage.Type == NetworkMessageType.DragHandCard)
                {
                    yield return StartCoroutine(playerAction.DoAction(playerAction.UseACard, parameter));
                    //yield return StartCoroutine(playerAction.UseACard(parameter));
                }
                //对方发动英雄技能
                if (networkMessage.Type == NetworkMessageType.LaunchHeroSkill)
                {
                    yield return StartCoroutine(playerAction.DoAction(playerAction.LaunchHeroSkill, parameter));
                    //yield return StartCoroutine(playerAction.LaunchHeroSkill(parameter));

                }
                //对方进入战斗阶段
                else if (networkMessage.Type == NetworkMessageType.StartAttackPhase)
                {
                    yield return StartCoroutine(playerAction.DoAction(playerAction.StartRoundBattle, parameter));
                    //yield return StartCoroutine(playerAction.StartRoundBattle(parameter));
                }
            }
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// 触发手牌中的“额外水晶”
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffectCondition("Before.GameAction.Sacrifice", compareMethodName = "Compare1")]
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

                string cardP = handCard["CardP"];
                Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);

                int crystalAmount = (int)pd["crystal"];

                Dictionary<string, object> parameter1 = new();
                parameter1.Add("CrystalAmount", crystalAmount);
                parameter1.Add("Player", player);
                yield return StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameter1));
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
    public bool Compare1(ParameterNode parameterNode)
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
                        string cardP = handCard["CardP"];
                        if (cardP != null && !cardP.Equals(""))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("crystal") && (int)pd["crystal"] > 0)
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
}