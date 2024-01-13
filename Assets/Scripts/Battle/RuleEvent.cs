using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�����¼���������Ҫʱ�����ܷ����ĵ����Ǽ��ܵĶ�����
/// </summary>
public class RuleEvent : OpportunityEffect
{
    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static RuleEvent instance;

    /// <summary>
    /// ��ñ���ʵ��
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
    /// ս��ǰ�ĳ�ʼ��
    /// �������նԷ���Ϣ���߳�
    /// ȷ���Ⱥ���
    /// ����˫������
    /// �������ơ�Ӣ�ۼ���
    /// </summary>
    [TriggerEffect("^GameReady$")]
    public IEnumerator InitializeBothDeck(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Debug.Log("ȷ���Ⱥ��֣����ͺͽ��տ�������");

        //ȡ���ҷ�����
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
            //��ʼ����������� ��ʼ������� �������������
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

            //���նԷ�����
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

            //���Ϳ���
            allyDeckDictionary.Add("HeroSkillId", allyHeroSkillId);
            allyDeckDictionary.Add("DeckCard", allyDeckCardSend);

            NetworkMessage allyDeckMessage = new(NetworkMessageType.PriorityAndDeck, allyDeckDictionary);
            SocketTool.SendMessage(allyDeckMessage);

            yield return null;

            //ȷ���Ⱥ��֣�0�Է���1����
            int priorityNumber = RandomUtils.GetRandomNumber(0, 1);
            battleProcess.nextTurn = priorityNumber == 1 ? Player.Ally : Player.Enemy;

            //ϴ��
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

            //����˫�������Ӣ�ۼ���
            //string a1 = "";
            //�Է�����
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

            //�Է�����
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

            //�ҷ�����
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

            //�ҷ�����
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

            //�Է�Ӣ�ۼ��ܣ��Ѽ�����ҵ�������
            string enemyHeroSkillId = (string)enemyDeckMessage.Parameter["HeroSkillId"];

            GameObject enemyHeroSkill = battleProcess.enemyPlayerData.heroSkillGameObject;
            enemyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(enemyHeroSkillId);

            //�ҷ�Ӣ�ۼ��ܣ��Ѽ�����ҵ�������
            GameObject allyHeroSkill = battleProcess.allyPlayerData.heroSkillGameObject;
            allyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(allyHeroSkillId);
        }
        else
        {
            //�������������
            //Debug.Log("�������������1");
            NetworkMessage networkMessage = null;
            do
            {
                networkMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (networkMessage == null || networkMessage.Type == NetworkMessageType.SendChallenge);

            RandomUtils.seed = (int)networkMessage.Parameter["RandomSeed"];
            RandomUtils.Init();

            yield return null;

            //Debug.Log("�������������2");

            //���Ϳ���
            allyDeckDictionary.Add("HeroSkillId", allyHeroSkillId);
            allyDeckDictionary.Add("DeckCard", allyDeckCardSend);

            NetworkMessage priorityNumberAndAllydeck = new(NetworkMessageType.PriorityAndDeck, allyDeckDictionary);
            SocketTool.SendMessage(priorityNumberAndAllydeck);

            yield return null;

            //���նԷ�����
            do
            {
                enemyDeckMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (enemyDeckMessage == null);

            yield return null;

            Dictionary<string, List<string>> enemyDeckDictionary = (Dictionary<string, List<string>>)enemyDeckMessage.Parameter["DeckCard"];
            enemyMonster = enemyDeckDictionary["Monster"];
            enemyItem = enemyDeckDictionary["Item"];

            //ȷ���Ⱥ��֣�0������1�Է�
            int priorityNumber = RandomUtils.GetRandomNumber(0, 1);
            battleProcess.nextTurn = priorityNumber == 0 ? Player.Ally : Player.Enemy;

            //ϴ��
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

            //����˫�������Ӣ�ۼ���
            //string a1 = "";
            //�ҷ�����
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

            //�ҷ�����
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

            //�Է�����
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

            //�Է�����
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

            //�ҷ�Ӣ�ۼ��ܣ��Ѽ�����ҵ�������
            GameObject allyHeroSkill = battleProcess.allyPlayerData.heroSkillGameObject;
            allyHeroSkill.AddComponent<HeroSkillInBattle>().AddSkill(allyHeroSkillId);

            //�Է�Ӣ�ۼ��ܣ��Ѽ�����ҵ�������
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

        Debug.Log("RuleEvent.initializeBothDeck:�������ƣ�ִ�����");
        yield return null;
    }

    /// <summary>
    /// ����غ�׼���׶�
    /// ʱ�� Opportunity.EnterTurnReady
    /// </summary>
    [TriggerEffect("^InRoundReady$")]
    public IEnumerator InRoundReady(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //�л�ϵͳ�ӽ�
        if (battleProcess.nextTurn == Player.Ally)
        {
            //Debug.Log("RuleEvent.EnterTurnReady:�ҷ��غ�׼���׶ο�ʼ");

            //�غ�ָʾ����
            GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("AllyRoundPrefab");
            GameObject battleSceneCanvas = GameObject.Find("BattleSceneCanvas");
            GameObject instance = Instantiate(prefab, battleSceneCanvas.transform);
            instance.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

            GameObject canvas = instance.transform.Find("Canvas").gameObject;
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            canvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            //�л�ϵͳ�ӽ�
            battleProcess.allyPlayerData.perspectivePlayer = Player.Ally;
            battleProcess.enemyPlayerData.perspectivePlayer = Player.Enemy;

            //ָ���¸��غ�
            battleProcess.nextTurn = Player.Enemy;
        }
        else if (battleProcess.nextTurn == Player.Enemy)
        {
            //Debug.Log("RuleEvent.EnterTurnReady���Է��غ�׼���׶ο�ʼ");

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

            //��ʼ����Է���Ϣ
            battleProcess.StartCoroutine(HandleEnemyAction());
        }

        //��3ˮ��
        Dictionary<string, object> parameter1 = new();
        parameter1.Add("CrystalAmount", 3);
        parameter1.Add("Player", Player.Ally);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));
    }

    /// <summary>
    /// �ڶԷ��غ��У�����Է�ʹ�����ơ�����Ӣ�ۼ��ܡ�����ս���׶ε���Ϣ
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
    /// ����ģʽ
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

                //�˺�����
                Dictionary<string, object> damageParameter = new();
                //��ǰ����
                damageParameter.Add("LaunchedSkill", this);
                //Ч������
                damageParameter.Add("EffectName", "Effect1");
                //�ܵ��˺��Ĺ���
                damageParameter.Add("EffectTarget", playerData.monsterGameObjectArray[0]);
                //�˺���ֵ
                damageParameter.Add("DamageValue", damageValue);
                //�˺�����
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
    /// ���������еġ�����ˮ����
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
    /// �ж��Ƿ������ƣ���ӵ�С�����ˮ����
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
    /// ʹ��һ�ſ���
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
    /// �ж��Ƿ�����ʹ����������
    /// ����Ʒ���Զ�����Ŀ��ʹ��
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare3(ParameterNode parameterNode)
    {
        //Debug.Log("����RuleEvent.CardBeUsed�ıȽ�---------");
        Dictionary<string, object> parameter = parameterNode.parameter;
        //ʹ�����Ƶ����
        Player player = (Player)parameter["Player"];
        //Ŀ�곡��λ�ã�012
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        //����Ŀ�����
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        //����λ�ã�01���ޣ�23����
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

        //��ȡʹ�����Ƶ���ҵ�PlayerData
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

        //�жϷ��ù�����
        if (useCardPlayerData.surplusCrystal < Convert.ToInt32(cardData["CardCost"]))
        {
            Debug.Log("���ò���");
            return false;
        }

        //����
        if (cardData["CardType"].Equals("monster"))
        {
            //�ǲ��Ǹ��Լ�ʹ��
            if (targetPlayer != player)
            {
                Debug.Log("���Ǹ��Լ�ʹ��");
                return false;
            }

            //������û�п�λ
            if (targetPlayerData.monsterGameObjectArray[2] != null)
            {
                Debug.Log("������û�п�λ");
                return false;
            }

            //��һ��λ�ò��ǿ�λ
            if (battlePanelNumber != 0 && targetPlayerData.monsterGameObjectArray[battlePanelNumber - 1] == null)
            {
                Debug.Log("��һ��λ�ò��ǿ�λ");
                return false;
            }
        }

        //����װ��������Ʒ��Ŀ�겻�ǿ�
        GameObject targetMonster = targetPlayerData.monsterGameObjectArray[battlePanelNumber];
        if ((cardData["CardType"].Equals("equip") || cardData["CardType"].Equals("consume")) && targetMonster == null)
        {
            Debug.Log("����װ��������Ʒ��Ŀ�겻�ǿ�");
            return false;
        }

        //װ��
        if (cardData["CardType"].Equals("equip"))
        {
            //�ж��Ƿ��Ǹ��ҷ�����
            if (targetPlayer != player)
            {
                Debug.Log("�ж��Ƿ��Ǹ��ҷ�����");
                return false;
            }

            //�ж���ɫ
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
                Debug.Log("�ж���ɫ");
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// �׼�һ�ſ���
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
    /// ����Ϊ0����װ��
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
    /// �ж��Ƿ��Ǳ����ޣ������ǻ��ף�ֵС��1
    /// </summary>
    public bool Compare5(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        return skillName == "armor" && !monsterInBattle.gameObject.TryGetComponent(out Armor _);
    }

    /// <summary>
    /// ��ʼս��ʱ������û�й��ޣ�����һ�ſ�
    /// </summary>
    [TriggerEffect(@"^InRoundBattle$", "Compare6")]
    public IEnumerator NegativePunishment(ParameterNode parameterNode)
    {
        //Debug.Log("��ʼս��ʱ������û�й��ޣ�����һ�ſ�");

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == Player.Ally)
            {
                string color = playerData.actualPlayer == Player.Ally ? "#00ff00" : "#ff0000";
                string playerC = playerData.actualPlayer == Player.Ally ? "�ҷ�" : "�з�";
                battleProcess.Log($"<color={color}>{playerC}</color>�ܵ������ͷ�");

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
    /// ����û�й���
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