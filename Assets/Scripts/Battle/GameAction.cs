using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏中所有的动作
/// </summary>
public class GameAction : MonoBehaviour
{
    /// <summary>
    /// 本类的实例
    /// </summary>
    private static GameAction gameAction;

    /// <summary>
    /// 获取本类的实例
    /// </summary>
    /// <returns>本类实例</returns>
    public static GameAction GetInstance()
    {
        if (gameAction == null)
        {
            gameAction = GameObject.Find("BattleProcessSystem").GetComponent<GameAction>();
        }
        return gameAction;
    }

    /// <summary>
    /// 实现对应效果。
    /// </summary>
    /// <param name="effect">效果方法委托</param>
    /// <param name="parameter">传给效果的参数</param>
    /// <returns></returns>
    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string effectName = effect.Method.Name;
        string fullName = "GameAction." + effectName;

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, fullName, parameterNode, effect));
        //yield return null;
    }

    /// <summary>
    /// 使用手牌
    /// </summary>
    public IEnumerator UseACard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        GameAction gameAction = GetInstance();
        BattleProcess battleProcess = BattleProcess.GetInstance();

        //Dictionary<string, object> cardDataInBattle = (Dictionary<string, object>)parameter["CardDataInBattle"];
        //使用手牌的玩家
        Player player = (Player)parameter["Player"];
        //目标场上位置，012
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        //手牌目标玩家
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        //手牌位置，01怪兽，23道具
        int handPanelNumber = (int)parameter["HandPanelNumber"];

        //使用的卡牌属性
        Dictionary<string, string> cardData = new();

        //获取使用手牌的玩家的PlayerData
        PlayerData useCardPlayerData = null;
        PlayerData targetPlayerData = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            if (playerData.perspectivePlayer == player)
            {
                useCardPlayerData = playerData;

                cardData = handPanelNumber < 2 ? playerData.handMonster[handPanelNumber] : playerData.handItem[handPanelNumber - 2];
                //Debug.Log(cardData["CardName"]);
                string cardFlags = cardData["CardFlags"];
                if (cardFlags != null && cardFlags != "")
                {
                    List<string> flags = JsonConvert.DeserializeObject<List<string>>(cardData["CardFlags"]);
                    if (flags.Contains("2"))
                    {
                        Dictionary<string, string> kind = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardData["CardKind"]);
                        Dictionary<string, int> skill = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardData["CardSkill"]);
                        //Debug.Log(cardData["CardEliteSkill"]);
                        Dictionary<string, Dictionary<string, object>> eliteSkill = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(cardData["CardEliteSkill"]);

                        int cardIndex = (int)parameter["CardIndexBeSelect"];
                        switch (cardIndex)
                        {
                            case 0:
                                Dictionary<string, string> newKind1 = new();
                                newKind1.Add("leftKind", kind["leftKind"]);
                                cardData["CardKind"] = JsonConvert.SerializeObject(newKind1);

                                Dictionary<string, int> newSkill1 = new();
                                newSkill1.Add(eliteSkill["leftSkill"]["name"].ToString(), Convert.ToInt32(eliteSkill["leftSkill"]["value"].ToString()));
                                foreach (var item in skill)
                                {
                                    newSkill1.Add(item.Key, item.Value);
                                }
                                cardData["CardSkill"] = JsonConvert.SerializeObject(newSkill1);

                                cardData["CardEliteSkill"] = null;

                                break;
                            default:
                                Dictionary<string, string> newKind2 = new();
                                newKind2.Add("leftKind", kind["rightKind"]);
                                cardData["CardKind"] = JsonConvert.SerializeObject(newKind2);

                                Dictionary<string, int> newSkill2 = new();
                                newSkill2.Add(eliteSkill["rightSkill"]["name"].ToString(), Convert.ToInt32(eliteSkill["rightSkill"]["value"].ToString()));
                                foreach (var item in skill)
                                {
                                    newSkill2.Add(item.Key, item.Value);
                                }
                                cardData["CardSkill"] = JsonConvert.SerializeObject(newSkill2);

                                cardData["CardEliteSkill"] = null;

                                break;
                        }
                    }
                    else if (flags.Contains("5"))
                    {
                        Dictionary<string, string> kind = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardData["CardKind"]);
                        Dictionary<string, int> skill = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardData["CardSkill"]);
                        Dictionary<string, Dictionary<string, object>> eliteSkill = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(cardData["CardEliteSkill"]);

                        string monsterKind = playerData.monsterGameObjectArray[battlePanelNumber].GetComponent<MonsterInBattle>().kind;

                        if (kind["leftKind"] == monsterKind)
                        {
                            Dictionary<string, string> newKind1 = new();
                            newKind1.Add("leftKind", kind["leftKind"]);
                            cardData["CardKind"] = JsonConvert.SerializeObject(newKind1);

                            Dictionary<string, int> newSkill1 = new();
                            newSkill1.Add(eliteSkill["leftSkill"]["name"].ToString(), Convert.ToInt32(eliteSkill["leftSkill"]["value"].ToString()));
                            foreach (var item in skill)
                            {
                                newSkill1.Add(item.Key, item.Value);
                            }
                            cardData["CardSkill"] = JsonConvert.SerializeObject(newSkill1);

                            cardData["CardEliteSkill"] = null;
                        }
                        else if (kind["rightKind"] == monsterKind)
                        {
                            Dictionary<string, string> newKind2 = new();
                            newKind2.Add("leftKind", kind["rightKind"]);
                            cardData["CardKind"] = JsonConvert.SerializeObject(newKind2);

                            Dictionary<string, int> newSkill2 = new();
                            newSkill2.Add(eliteSkill["rightSkill"]["name"].ToString(), Convert.ToInt32(eliteSkill["rightSkill"]["value"].ToString()));
                            foreach (var item in skill)
                            {
                                newSkill2.Add(item.Key, item.Value);
                            }
                            cardData["CardSkill"] = JsonConvert.SerializeObject(newSkill2);

                            cardData["CardEliteSkill"] = null;
                        }
                    }
                }
            }

            if (playerData.perspectivePlayer == targetPlayer)
            {
                targetPlayerData = playerData;
            }
        }

        //扣除卡牌的费用
        Dictionary<string, object> parameter2 = new();
        parameter2.Add("CrystalAmount", -Convert.ToInt32(cardData["CardCost"]));
        parameter2.Add("Player", player);

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameterNode2));

        //怪兽
        if (cardData["CardType"].Equals("monster"))
        {
            //场上生成怪兽
            Dictionary<string, object> parameter3 = new();
            parameter3.Add("CardData", cardData);
            parameter3.Add("Player", player);
            parameter3.Add("BattlePanelNumber", battlePanelNumber);

            ParameterNode parameterNode3 = parameterNode.AddNodeInMethod();
            parameterNode3.parameter = parameter3;

            yield return StartCoroutine(DoAction(MonsterEnterBattle, parameterNode3));
        }

        GameObject targetMonster = targetPlayerData.monsterGameObjectArray[battlePanelNumber];

        //装备
        if (cardData["CardType"].Equals("equip"))
        {
            //场上生成装备
            Dictionary<string, object> parameter4 = new();
            parameter4.Add("CardData", cardData);
            parameter4.Add("EquipmentTarget", targetMonster);

            ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
            parameterNode4.parameter = parameter4;

            yield return StartCoroutine(DoAction(EquipmentEnterBattle, parameterNode4));
        }

        //消耗品
        if (cardData["CardType"].Equals("consume"))
        {
            //场上生成装备
            Dictionary<string, object> parameter5 = new();
            parameter5.Add("CardData", cardData);
            parameter5.Add("Player", player);
            parameter5.Add("BattlePanelNumber", battlePanelNumber);
            parameter5.Add("TargetPlayer", targetPlayer);

            ParameterNode parameterNode5 = parameterNode.AddNodeInMethod();
            parameterNode5.parameter = parameter5;

            yield return StartCoroutine(DoAction(ConsumeEnterBattle, parameterNode5));
        }

        //销毁对应手牌
        Dictionary<string, object> parameter6 = new();
        parameter6.Add("HandPanelNumber", handPanelNumber);
        parameter6.Add("Player", player);

        ParameterNode parameterNode6 = parameterNode.AddNodeInMethod();
        parameterNode6.parameter = parameter6;

        yield return StartCoroutine(DoAction(DestroyAHandCard, parameterNode6));
    }

    /// <summary>
    /// 怪兽进入战场
    /// </summary>
    public IEnumerator MonsterEnterBattle(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        Player player = (Player)parameter["Player"];
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                //移动后面的怪兽
                for (int k = 2; k > battlePanelNumber; k--)
                {
                    if (playerData.monsterGameObjectArray[k - 1] != null)
                    {
                        Dictionary<string, object> moveParameter = new();
                        moveParameter.Add("MonsterMove", playerData.monsterGameObjectArray[k - 1]);
                        moveParameter.Add("TargetPosition", k);
                        moveParameter.Add("Player", player);

                        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                        parameterNode2.parameter = moveParameter;

                        yield return StartCoroutine(DoAction(MoveMonsterPosition, parameterNode2));
                    }
                }

                GameObject cardInBattlePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("MonsterInBattlePrefab");
                GameObject battlePanel = playerData.monsterInBattlePanel[battlePanelNumber];
                GameObject cardInBattle = Instantiate(cardInBattlePrefab, battlePanel.transform);
                cardInBattle.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
                yield return cardInBattle;

                playerData.monsterGameObjectArray[battlePanelNumber] = cardInBattle;

                yield return StartCoroutine(cardInBattle.GetComponent<MonsterInBattle>().Generate(cardData));
                GameObject cardCanvas = cardInBattle.transform.GetChild(0).gameObject;
                cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                result.Add("MonsterBeGenerated", cardInBattle);
                yield break;
            }
        }
    }

    /// <summary>
    /// 装备进入战场
    /// </summary>
    public IEnumerator EquipmentEnterBattle(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        GameObject targetMonster = (GameObject)parameter["EquipmentTarget"];
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];

        MonsterInBattle monsterInBattle = targetMonster.GetComponent<MonsterInBattle>();

        result.Add("MonsterBeEquipped", monsterInBattle.gameObject);

        yield return StartCoroutine(monsterInBattle.AddEquipment(cardData));
        //yield return null;
    }

    /// <summary>
    /// 消耗品进入战场
    /// </summary>
    public IEnumerator ConsumeEnterBattle(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        Player player = (Player)parameter["Player"];
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];
        Player targetPlayer = (Player)parameter["TargetPlayer"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                GameObject settlementAreaPanel = GameObject.Find("SettlementAreaPanel");
                GameObject cardForShowPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardForShowPrefab");

                GameObject consume = Instantiate(cardForShowPrefab, settlementAreaPanel.transform);
                consume.name = "ConsumeInSettlement";
                consume.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
                consume.GetComponent<Transform>().localScale = new Vector3(0.8f, 0.8f, 1);
                yield return consume;

                consume.GetComponent<CardForShow>().SetAllAttribute(cardData);
                StartCoroutine(consume.AddComponent<ConsumeInBattle>().Generate(cardData));
                GameObject cardCanvas = consume.transform.Find("Canvas").gameObject;
                RectTransform rectTransform = cardCanvas.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(220.5f, 308);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                //卡牌数据进入玩家信息
                playerData.consumeGameObject = consume;
                result.Add("ConsumeBeGenerated", consume);
            }

            if (playerData.perspectivePlayer == targetPlayer)
            {
                GameObject targetMonster = playerData.monsterGameObjectArray[battlePanelNumber];
                result.Add("ConsumeTarget", targetMonster);
            }
        }
    }

    /// <summary>
    /// 消耗品离场
    /// </summary>
    public IEnumerator ConsumeLeave(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                Destroy(playerData.consumeGameObject);
                playerData.consumeGameObject = null;
                yield break;
            }
        }
    }

    /// <summary>
    /// 加减水晶
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator ChangeCrystalAmount(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int crystalAmount = (int)parameter["CrystalAmount"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        foreach (PlayerData playerMessage in BattleProcess.GetInstance().systemPlayerData)
        {
            if (playerMessage.perspectivePlayer == player)
            {
                string color = playerMessage.actualPlayer == Player.Ally ? "#00ff00" : "#ff0000";
                string playerC = playerMessage.actualPlayer == Player.Ally ? "我方" : "敌方";

                int c1 = playerMessage.surplusCrystal;
                playerMessage.surplusCrystal = (playerMessage.surplusCrystal + crystalAmount) > 0 ? playerMessage.surplusCrystal + crystalAmount : 0;

                int c2 = playerMessage.surplusCrystal - c1;
                battleProcess.Log($"<color={color}>{playerC}</color>{(c2 < 0 ? "失去" : "获得")}<color=#ffff00>{(c2 > 0 ? c2 : -c2)}</color>水晶");

                if (playerMessage.surplusCrystalText != null)
                {
                    playerMessage.surplusCrystalText.text = playerMessage.surplusCrystal.ToString();
                }

                break;
            }
        }

        yield break;
        //yield return null;
    }

    /// <summary>
    /// 消灭一张手牌
    /// </summary>
    public IEnumerator DestroyAHandCard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int handPanelNumber = (int)parameter["HandPanelNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        foreach (PlayerData playerData in BattleProcess.GetInstance().systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                switch (handPanelNumber)
                {
                    case 0:
                        playerData.handMonster[0] = null;

                        playerData.surplusMonsterAmountText.text = (playerData.monsterDeck.Count + (playerData.handMonster[0] == null ? 0 : 1) + (playerData.handMonster[1] == null ? 0 : 1)).ToString();

                        if (playerData.handMonsterPanel != null)
                        {
                            Destroy(playerData.handMonsterPanel[0].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }

                        if (playerData.monsterDeck.Count > 0)
                        {
                            int r = RandomUtils.GetRandomNumber(0, playerData.monsterDeck.Count - 1);

                            //加载手牌
                            Dictionary<string, object> supplyAHandCardParameter = new();
                            supplyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
                            supplyAHandCardParameter.Add("Player", player);
                            supplyAHandCardParameter.Add("Index", r);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = supplyAHandCardParameter;

                            yield return StartCoroutine(DoAction(SupplyAHandCard, parameterNode1));

                            //移除牌库牌
                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("Player", player);
                            parameter2.Add("Type", "monster");
                            parameter2.Add("Index", r);

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode2));
                        }
                        break;
                    case 1:
                        playerData.handMonster[1] = null;

                        playerData.surplusMonsterAmountText.text = (playerData.monsterDeck.Count + (playerData.handMonster[0] == null ? 0 : 1) + (playerData.handMonster[1] == null ? 0 : 1)).ToString();

                        if (playerData.handMonsterPanel != null)
                        {
                            Destroy(playerData.handMonsterPanel[1].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }

                        if (playerData.monsterDeck.Count > 0)
                        {
                            int r = RandomUtils.GetRandomNumber(0, playerData.monsterDeck.Count - 1);

                            //加载手牌
                            Dictionary<string, object> supplyAHandCardParameter = new();
                            supplyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
                            supplyAHandCardParameter.Add("Player", player);
                            supplyAHandCardParameter.Add("Index", r);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = supplyAHandCardParameter;

                            yield return StartCoroutine(DoAction(SupplyAHandCard, parameterNode1));

                            //移除牌库牌
                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("Player", player);
                            parameter2.Add("Type", "monster");
                            parameter2.Add("Index", r);

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode2));
                        }
                        break;
                    case 2:
                        playerData.handItem[0] = null;

                        playerData.surplusItemAmountText.text = (playerData.itemDeck.Count + (playerData.handItem[0] == null ? 0 : 1) + (playerData.handItem[1] == null ? 0 : 1)).ToString();

                        if (playerData.handItemPanel != null)
                        {
                            Destroy(playerData.handItemPanel[0].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }

                        if (playerData.itemDeck.Count > 0)
                        {
                            int r = RandomUtils.GetRandomNumber(0, playerData.itemDeck.Count - 1);

                            //加载手牌
                            Dictionary<string, object> supplyAHandCardParameter = new();
                            supplyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
                            supplyAHandCardParameter.Add("Player", player);
                            supplyAHandCardParameter.Add("Index", r);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = supplyAHandCardParameter;

                            yield return StartCoroutine(DoAction(SupplyAHandCard, parameterNode1));

                            //移除牌库牌
                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("Player", player);
                            parameter2.Add("Type", "item");
                            parameter2.Add("Index", r);

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode2));
                        }
                        break;
                    case 3:
                        playerData.handItem[1] = null;

                        playerData.surplusItemAmountText.text = (playerData.itemDeck.Count + (playerData.handItem[0] == null ? 0 : 1) + (playerData.handItem[1] == null ? 0 : 1)).ToString();

                        if (playerData.handItemPanel != null)
                        {
                            Destroy(playerData.handItemPanel[1].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }

                        if (playerData.itemDeck.Count > 0)
                        {
                            int r = RandomUtils.GetRandomNumber(0, playerData.itemDeck.Count - 1);

                            //加载手牌
                            Dictionary<string, object> supplyAHandCardParameter = new();
                            supplyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
                            supplyAHandCardParameter.Add("Player", player);
                            supplyAHandCardParameter.Add("Index", r);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = supplyAHandCardParameter;

                            yield return StartCoroutine(DoAction(SupplyAHandCard, parameterNode1));

                            //移除牌库牌
                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("Player", player);
                            parameter2.Add("Type", "item");
                            parameter2.Add("Index", r);

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode2));
                        }
                        break;
                }

                bool noMonsterInBattle = true;
                for (int k = 0; k < playerData.monsterGameObjectArray.Length; k++)
                {
                    if (playerData.monsterGameObjectArray[k] != null)
                    {
                        noMonsterInBattle = false;
                    }
                }

                //Debug.Log(playerData.monsterDeck.Count + "---" + (playerData.handMonster[0] == null) + "----" + (playerData.handMonster[1] == null) + "----" + noMonsterInBattle);
                if (playerData.monsterDeck.Count == 0 && playerData.handMonster[0] == null && playerData.handMonster[1] == null && noMonsterInBattle)
                {
                    switch (playerData.actualPlayer)
                    {
                        case Player.Enemy:
                            Debug.Log("我方胜利");
                            battleProcess.GameVictory();
                            break;
                        case Player.Ally:
                            Debug.Log("敌方胜利");
                            battleProcess.GameDefeat();
                            break;
                    }

                    yield return null;
                }

                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 随机消灭牌库中一张牌
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator DestroyADeckCard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string type = (string)parameter["Type"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        foreach (PlayerData playerData in battleProcess.systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                if (type == "monster")
                {
                    int r = RandomUtils.GetRandomNumber(0, playerData.monsterDeck.Count - 1);

                    Dictionary<string, string> cardData = playerData.monsterDeck[r];
                    string cardName = cardData["CardName"];

                    switch (playerData.actualPlayer)
                    {
                        case Player.Ally:
                            battleProcess.Log($"<color=#00ff00>我方</color>牌库怪兽<color=#ffff00>{cardName}</color>被消灭");
                            break;
                        case Player.Enemy:
                            battleProcess.Log($"<color=#ff0000>敌方</color>牌库怪兽<color=#ffff00>{cardName}</color>被消灭");
                            break;
                    }

                    Dictionary<string, object> parameter1 = new(parameter);
                    parameter1.Add("Index", r);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode1));
                }
                else
                {
                    int r = RandomUtils.GetRandomNumber(0, playerData.itemDeck.Count - 1);

                    Dictionary<string, string> cardData = playerData.itemDeck[r];
                    string cardName = cardData["CardName"];

                    switch (playerData.actualPlayer)
                    {
                        case Player.Ally:
                            battleProcess.Log($"<color=#00ff00>我方</color>牌库道具<color=#ffff00>{cardName}</color>被消灭");
                            break;
                        case Player.Enemy:
                            battleProcess.Log($"<color=#00ff00>敌方</color>牌库道具<color=#ffff00>{cardName}</color>被消灭");
                            break;
                    }

                    Dictionary<string, object> parameter1 = new(parameter);
                    parameter1.Add("Index", r);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode1));
                }

                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 卡牌离开牌库
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator CardLeaveLibrary(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string type = (string)parameter["Type"];
        Player player = (Player)parameter["Player"];
        int index = (int)parameter["Index"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        foreach (PlayerData playerData in battleProcess.systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                if (type == "monster")
                {
                    playerData.monsterDeck.RemoveAt(index);

                    playerData.surplusMonsterAmountText.text = (playerData.monsterDeck.Count + (playerData.handMonster[0] == null ? 0 : 1) + (playerData.handMonster[1] == null ? 0 : 1)).ToString();
                }
                else
                {

                    playerData.itemDeck.RemoveAt(index);

                    playerData.surplusItemAmountText.text = (playerData.itemDeck.Count + (playerData.handItem[0] == null ? 0 : 1) + (playerData.handItem[1] == null ? 0 : 1)).ToString();
                }

                break;
            }
        }

        yield break;
    }

    /// <summary>
    /// 场上怪兽回到牌库
    /// damageParameter.Add("LaunchedSkill", this);
    /// damageParameter.Add("EffectName", "Effect1");
    /// damageParameter.Add("EffectTarget", effectTarget);
    /// </summary>
    public IEnumerator MonsterInBattleToDeck(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return StartCoroutine(DoAction(MonsterLeave, parameterNode1));

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter;

        yield return StartCoroutine(DoAction(AddCardToDeck, parameterNode2));

        yield return null;
    }

    /// <summary>
    /// 场上怪兽离场
    /// damageParameter.Add("EffectTarget", effectTarget);
    /// </summary>
    public IEnumerator MonsterLeave(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                GameObject[] monsterGameObjectArray = playerData.monsterGameObjectArray;
                if (monsterGameObjectArray[j] == effectTarget)
                {
                    Destroy(effectTarget);
                    monsterGameObjectArray[j] = null;

                    bool noMonsterInBattle = true;
                    for (int k = 0; k < playerData.monsterGameObjectArray.Length; k++)
                    {
                        if (playerData.monsterGameObjectArray[k] != null)
                        {
                            noMonsterInBattle = false;
                        }
                    }

                    //Debug.Log(playerData.monsterDeck.Count + "---" + (playerData.handMonster[0] == null) + "----" + (playerData.handMonster[1] == null) + "----" + noMonsterInBattle);
                    if (playerData.monsterDeck.Count == 0 && playerData.handMonster[0] == null && playerData.handMonster[1] == null && noMonsterInBattle)
                    {
                        switch (playerData.actualPlayer)
                        {
                            case Player.Enemy:
                                Debug.Log("我方胜利");
                                battleProcess.GameVictory();
                                break;
                            case Player.Ally:
                                Debug.Log("敌方胜利");
                                battleProcess.GameDefeat();
                                break;
                        }

                        yield return null;
                    }

                    //移动后面的怪兽
                    for (int k = j + 1; k < monsterGameObjectArray.Length; k++)
                    {
                        if (monsterGameObjectArray[k] != null)
                        {
                            Dictionary<string, object> moveParameter = new();
                            moveParameter.Add("MonsterMove", monsterGameObjectArray[k]);
                            moveParameter.Add("TargetPosition", k - 1);
                            moveParameter.Add("Player", playerData.perspectivePlayer);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = moveParameter;

                            yield return StartCoroutine(DoAction(MoveMonsterPosition, parameterNode1));
                            yield return null;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 装备离场
    /// </summary>
    public IEnumerator EquipmentLeave(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject targetMonster = (GameObject)parameter["EffectTarget"];

        MonsterInBattle monsterInBattle = targetMonster.GetComponent<MonsterInBattle>();

        yield return StartCoroutine(monsterInBattle.RemoveEquipment());
        yield return null;
    }

    /// <summary>
    /// 将牌洗入牌库
    /// </summary>
    public IEnumerator AddCardToDeck(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        string cardType = cardData["CardType"];

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                if (cardType.Equals("monster"))
                {
                    int r = RandomUtils.GetRandomNumber(0, playerData.monsterDeck.Count);
                    playerData.monsterDeck.Insert(r, cardData);

                    playerData.surplusMonsterAmountText.text = (playerData.monsterDeck.Count + (playerData.handMonster[0] == null ? 0 : 1) + (playerData.handMonster[1] == null ? 0 : 1)).ToString();

                    for (int j = 0; j < playerData.handMonster.Length; j++)
                    {
                        if (playerData.handMonster[j] == null)
                        {
                            int r2 = RandomUtils.GetRandomNumber(0, playerData.monsterDeck.Count - 1);

                            Dictionary<string, object> parameter1 = new();
                            parameter1.Add("HandPanelNumber", j);
                            parameter1.Add("Player", player);
                            parameter1.Add("Index", r2);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter1;

                            yield return StartCoroutine(DoAction(SupplyAHandCard, parameterNode1));

                            //移除牌库牌
                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("Player", player);
                            parameter2.Add("Type", "monster");
                            parameter2.Add("Index", r2);

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode2));

                            yield break;
                        }
                    }
                }
                else
                {
                    int r = RandomUtils.GetRandomNumber(0, playerData.itemDeck.Count);
                    playerData.itemDeck.Insert(r, cardData);
                    //Debug.Log("道具数量" + playerData.itemDeck.Count + (playerData.handItem[0] == null ? 0 : 1) + (playerData.handItem[1] == null ? 0 : 1));
                    playerData.surplusItemAmountText.text = (playerData.itemDeck.Count + (playerData.handItem[0] == null ? 0 : 1) + (playerData.handItem[1] == null ? 0 : 1)).ToString();

                    for (int j = 0; j < playerData.handItem.Length; j++)
                    {
                        if (playerData.handItem[j] == null)
                        {
                            int r2 = RandomUtils.GetRandomNumber(0, playerData.itemDeck.Count - 1);

                            Dictionary<string, object> parameter1 = new();
                            parameter1.Add("HandPanelNumber", j + 2);
                            parameter1.Add("Player", player);
                            parameter1.Add("Index", r2);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter1;

                            yield return StartCoroutine(DoAction(SupplyAHandCard, parameterNode1));

                            //移除牌库牌
                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("Player", player);
                            parameter2.Add("Type", "item");
                            parameter2.Add("Index", r2);

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(DoAction(CardLeaveLibrary, parameterNode2));

                            yield break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 补一张手牌
    /// handPanelNumber 手牌位置的序号，01怪兽，23道具
    /// player 哪个玩家
    /// </summary>
    public IEnumerator SupplyAHandCard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int handPanelNumber = (int)parameter["HandPanelNumber"];
        Player player = (Player)parameter["Player"];
        int index = (int)parameter["Index"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            if (playerData.perspectivePlayer == player)
            {
                switch (handPanelNumber)
                {
                    case 0:
                        playerData.handMonster[0] = new(playerData.monsterDeck[index]);

                        if (playerData.handMonsterPanel != null)
                        {
                            LoadACardToHand(playerData.handMonster[0], playerData.handMonsterPanel[0]);
                        }
                        break;
                    case 1:
                        playerData.handMonster[1] = new(playerData.monsterDeck[index]);

                        if (playerData.handMonsterPanel != null)
                        {
                            LoadACardToHand(playerData.handMonster[1], playerData.handMonsterPanel[1]);
                        }
                        break;
                    case 2:
                        playerData.handItem[0] = new(playerData.itemDeck[index]);

                        if (playerData.handItemPanel != null)
                        {
                            LoadACardToHand(playerData.handItem[0], playerData.handItemPanel[0]);
                        }
                        break;
                    case 3:
                        playerData.handItem[1] = new(playerData.itemDeck[index]);

                        if (playerData.handItemPanel != null)
                        {
                            LoadACardToHand(playerData.handItem[1], playerData.handItemPanel[1]);
                        }
                        break;
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// 加载手牌图像
    /// </summary>
    /// <param name="card">卡牌数据</param>
    /// <param name="panel">所在的位置</param>
    private void LoadACardToHand(Dictionary<string, string> card, GameObject panel)
    {
        //Debug.Log("加载手牌图像");
        GameObject cardForShowPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardForShowPrefab");
        GameObject aCard = Instantiate(cardForShowPrefab, panel.transform);
        aCard.name = "CardForShow";
        aCard.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        aCard.GetComponent<Transform>().localScale = new Vector3(0.8f, 0.8f, 1);
        aCard.GetComponent<CardForShow>().SetAllAttribute(card);
        aCard.AddComponent<DragHandCard>();
        GameObject cardCanvas = aCard.transform.GetChild(0).gameObject;
        cardCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(220.5f, 308);
        cardCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    /// <summary>
    /// 消灭怪兽上的装备
    /// </summary>
    public IEnumerator DestroyEquipment(ParameterNode parameterNode)
    {
        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameterNode.parameter;

        yield return StartCoroutine(DoAction(EquipmentLeave, parameterNode1));
        yield return null;
    }

    /// <summary>
    /// 消灭怪兽
    /// Destroyer消灭怪兽的怪兽或消耗品
    /// </summary>
    public IEnumerator DestroyMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeDestroy = (GameObject)parameter["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        Player? player = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == monsterBeDestroy)
                {
                    player = systemPlayerData.perspectivePlayer;
                }
            }
        }

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("EffectTarget", monsterBeDestroy);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return StartCoroutine(DoAction(MonsterLeave, parameterNode1));

        //获得1水晶
        Dictionary<string, object> parameter2 = new();
        parameter2.Add("CrystalAmount", 1);
        parameter2.Add("Player", player);

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameterNode2));
        yield return null;
    }

    /// <summary>
    /// 对怪兽造成伤害
    /// damageParameter.Add("LaunchedSkill", this);
    /// damageParameter.Add("EffectName", "Effect1");
    /// damageParameter.Add("EffectTarget", effectTarget);
    /// damageParameter.Add("DamageValue", skillValue);
    /// damageParameter.Add("DamageType", DamageType.Real);
    /// </summary>
    public IEnumerator HurtMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        int damageValue = (int)parameter["DamageValue"];
        OpportunityEffect opportunityEffect = (OpportunityEffect)parameter["LaunchedSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();

        //画箭头
        yield return StartCoroutine(ArrowUtils.CreateArrow(opportunityEffect.gameObject.transform.position, monsterBeHurt.transform.position));
        if (opportunityEffect.gameObject.TryGetComponent(out MonsterInBattle monsterInBattle1))
        {
            battleProcess.Log($"<color=#00ff00>{monsterInBattle1.cardName}</color>造成{damageValue}点伤害");
        }
        else if (opportunityEffect.gameObject.TryGetComponent(out ConsumeInBattle consumeInBattle))
        {
            battleProcess.Log($"<color=#00ff00>{consumeInBattle.cardName}</color>造成{damageValue}点伤害");
        }
        else if (opportunityEffect.gameObject.TryGetComponent(out HeroSkill heroSkill))
        {
            battleProcess.Log($"<color=#00ff00>{heroSkill.heroSkillNameText.text}</color>造成{damageValue}点伤害");
        }
        else if (opportunityEffect.gameObject.TryGetComponent(out RuleEvent _))
        {
            battleProcess.Log($"<color=#00ff00>规则</color>造成{damageValue}点伤害");
        }

        //画伤害值
        GameObject healthValueChangePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("HealthValueChangePrefab");
        GameObject healthValueChange = Instantiate(healthValueChangePrefab, monsterInBattle.gameObject.transform);
        healthValueChange.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        GameObject valueCanvas = healthValueChange.transform.GetChild(0).gameObject;
        valueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        healthValueChange.GetComponent<InitHealthValueChangePrefab>().Init("-" + damageValue, "#ff0000");
        yield return new WaitForSecondsRealtime(0.5f);

        damageValue = damageValue > 0 ? damageValue : 0;

        int currentHp = monsterInBattle.GetCurrentHp();
        monsterInBattle.SetCurrentHp(currentHp - damageValue);

        parameterNode.result.Add("CauseDamageToHealth", true);
        if (damageValue > currentHp)
        {
            parameterNode.result.Add("ExcessiveDamage", damageValue - currentHp);
        }

        yield return null;
    }

    /// <summary>
    /// 移动场上怪兽
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator MoveMonsterPosition(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        //移动的怪兽
        GameObject monsterMove = (GameObject)parameter["MonsterMove"];
        //移动后的位置
        int targetPosition = (int)parameter["TargetPosition"];
        //哪个玩家移动
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //Debug.Log("移动怪兽----------" + targetPosition);
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer == player)
            {
                GameObject[] monsterGameObjectArray = systemPlayerData.monsterGameObjectArray;
                //先数据上移动
                //monsterGameObjectArray[targetPosition] = monsterMove;
                for (int j = 0; j < monsterGameObjectArray.Length; j++)
                {
                    if (monsterGameObjectArray[j] == monsterMove)
                    {
                        (monsterGameObjectArray[j], monsterGameObjectArray[targetPosition]) = (monsterGameObjectArray[targetPosition], monsterGameObjectArray[j]);
                    }
                }
                //画面上的移动
                monsterMove.transform.SetParent(systemPlayerData.monsterInBattlePanel[targetPosition].transform, false);
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 交换怪兽位置
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator SwapMonsterPosition(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        //移动的怪兽
        GameObject monsterMove1 = (GameObject)parameter["MonsterMove1"];
        GameObject monsterMove2 = (GameObject)parameter["MonsterMove2"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            GameObject[] monsterGameObjectArray = systemPlayerData.monsterGameObjectArray;

            //先数据上移动
            for (int j = 0; j < monsterGameObjectArray.Length; j++)
            {
                if (monsterGameObjectArray[j] == monsterMove1)
                {
                    for (int k = 0; k < monsterGameObjectArray.Length; k++)
                    {
                        if (monsterGameObjectArray[k] == monsterMove2)
                        {
                            (monsterGameObjectArray[k], monsterGameObjectArray[j]) = (monsterGameObjectArray[j], monsterGameObjectArray[k]);

                            //画面上的移动
                            monsterMove1.transform.SetParent(systemPlayerData.monsterInBattlePanel[k].transform, false);
                            monsterMove2.transform.SetParent(systemPlayerData.monsterInBattlePanel[j].transform, false);

                            goto end;
                        }
                    }
                }
            }
        }

    end:;

        yield return null;
    }

    /// <summary>
    /// 治疗怪兽
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator TreatMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeTreat = (GameObject)parameter["EffectTarget"];
        int treatValue = (int)parameter["TreatValue"];

        treatValue = treatValue > 0 ? treatValue : 0;

        //画治疗值
        GameObject healthValueChangePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("HealthValueChangePrefab");
        GameObject healthValueChange = Instantiate(healthValueChangePrefab, monsterBeTreat.transform);
        healthValueChange.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        GameObject valueCanvas = healthValueChange.transform.GetChild(0).gameObject;
        valueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        healthValueChange.GetComponent<InitHealthValueChangePrefab>().Init("+" + treatValue, "#00ff00");
        yield return new WaitForSecondsRealtime(0.5f);

        MonsterInBattle monsterInBattle = monsterBeTreat.GetComponent<MonsterInBattle>();
        int currentHp = monsterInBattle.GetCurrentHp();
        monsterInBattle.SetCurrentHp(currentHp + treatValue > monsterInBattle.maxHp ? monsterInBattle.maxHp : currentHp + treatValue);
        yield return null;
    }

    /// <summary>
    /// 献祭
    /// ObjectBeSacrificedNumber 手牌位置的序号，01怪兽，23道具，456场上怪兽
    /// 献祭手牌时，先获取水晶再销毁手牌，献祭场上怪兽时，先销毁怪兽再
    /// </summary>
    public IEnumerator Sacrifice(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;

        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //禁用献祭按钮
        if (player == Player.Ally)
        {
            GameObject sacrificeButtonImage = GameObject.Find("SacrificeButtonImage");
            RawImage sacrificeButtonRawImage = sacrificeButtonImage.GetComponent<RawImage>();
            sacrificeButtonRawImage.texture = Resources.Load<Texture2D>("Image/SacrificeDisableButton");
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                string color = systemPlayerData.actualPlayer == Player.Ally ? "#00ff00" : "#ff0000";
                string playerC = systemPlayerData.actualPlayer == Player.Ally ? "我方" : "敌方";

                systemPlayerData.canSacrifice = false;

                //记录被献祭卡牌上额外水晶的值
                if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber < 4)
                {
                    Dictionary<string, string> handCard = null;
                    switch (objectBeSacrificedNumber)
                    {
                        case 0:
                            handCard = systemPlayerData.handMonster[0];
                            break;
                        case 1:
                            handCard = systemPlayerData.handMonster[1];
                            break;
                        case 2:
                            handCard = systemPlayerData.handItem[0];
                            break;
                        case 3:
                            handCard = systemPlayerData.handItem[1];
                            break;
                    }

                    string cardP = handCard["CardSkill"];
                    if (!string.IsNullOrEmpty(cardP))
                    {
                        Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);

                        if (pd.ContainsKey("crystal"))
                        {
                            int skillValue = Convert.ToInt32(pd["crystal"]);

                            result["CrystalSkillValue"] = skillValue;
                        }
                    }
                }
                else
                {
                    int t = objectBeSacrificedNumber - 4;

                    if (systemPlayerData.monsterGameObjectArray[t].TryGetComponent(out Crystal crystal))
                    {
                        result["CrystalSkillValue"] = crystal.GetSkillValue();
                    }
                }

                //被献祭的是手牌怪兽
                if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber <= 1)
                {
                    Dictionary<string, string> cardData = systemPlayerData.handMonster[objectBeSacrificedNumber];
                    string cardName = cardData["CardName"];
                    battleProcess.Log($"<color={color}>{playerC}</color>献祭手牌怪兽<color=#ffff00>{cardName}</color>");

                    //销毁对应手牌
                    Dictionary<string, object> destroyAHandCardParameter = new();
                    destroyAHandCardParameter.Add("HandPanelNumber", objectBeSacrificedNumber);
                    destroyAHandCardParameter.Add("Player", player);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = destroyAHandCardParameter;

                    yield return StartCoroutine(DoAction(DestroyAHandCard, parameterNode1));

                    //获得水晶
                    Dictionary<string, object> parameter2 = new();
                    parameter2.Add("LaunchedSkill", this);
                    parameter2.Add("EffectName", "Sacrifice");
                    parameter2.Add("CrystalAmount", 2);
                    parameter2.Add("Player", player);

                    ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    parameterNode2.parameter = parameter2;

                    yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameterNode2));
                }

                //被献祭的是手牌装备
                if (objectBeSacrificedNumber >= 2 && objectBeSacrificedNumber <= 3)
                {
                    Dictionary<string, string> cardData = systemPlayerData.handItem[objectBeSacrificedNumber - 2];
                    string cardName = cardData["CardName"];
                    battleProcess.Log($"<color={color}>{playerC}</color>献祭手牌道具<color=#ffff00>{cardName}</color>");

                    //销毁对应手牌
                    Dictionary<string, object> destroyAHandCardParameter = new();
                    destroyAHandCardParameter.Add("HandPanelNumber", objectBeSacrificedNumber);
                    destroyAHandCardParameter.Add("Player", player);

                    ParameterNode parameterNode3 = parameterNode.AddNodeInMethod();
                    parameterNode3.parameter = destroyAHandCardParameter;

                    yield return StartCoroutine(DoAction(DestroyAHandCard, parameterNode3));

                    //获得水晶
                    Dictionary<string, object> parameter4 = new();
                    parameter4.Add("LaunchedSkill", this);
                    parameter4.Add("EffectName", "Sacrifice");
                    parameter4.Add("CrystalAmount", 1);
                    parameter4.Add("Player", player);

                    ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
                    parameterNode4.parameter = parameter4;

                    yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameterNode4));
                }

                //被献祭的是场上怪兽
                if (objectBeSacrificedNumber >= 4 && objectBeSacrificedNumber <= 6)
                {
                    int t = objectBeSacrificedNumber - 4;
                    GameObject[] monsterGameObjectArray = systemPlayerData.monsterGameObjectArray;
                    GameObject go = monsterGameObjectArray[t];

                    MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();
                    battleProcess.Log($"<color={color}>{playerC}</color>献祭场上怪兽<color=#ffff00>{monsterInBattle.cardName}</color>");

                    //怪兽离场
                    Dictionary<string, object> parameter6 = new();
                    parameter6.Add("EffectTarget", go);

                    ParameterNode parameterNode6 = parameterNode.AddNodeInMethod();
                    parameterNode6.parameter = parameter6;

                    yield return StartCoroutine(DoAction(MonsterLeave, parameterNode6));

                    //获得水晶
                    Dictionary<string, object> parameter5 = new();
                    parameter5.Add("LaunchedSkill", this);
                    parameter5.Add("EffectName", "Sacrifice");
                    parameter5.Add("CrystalAmount", 1);
                    parameter5.Add("Player", systemPlayerData.perspectivePlayer);

                    ParameterNode parameterNode5 = parameterNode.AddNodeInMethod();
                    parameterNode5.parameter = parameter5;

                    yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameterNode5));
                }

                systemPlayerData.canUseHandCard = true;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 技能选取目标
    /// (List<GameObject>)parameter["NontargetList"];
    /// (List<GameObject>)parameter["PriorTargetList"];
    /// damageParameter.Add("LaunchedSkill", this);
    /// damageParameter.Add("EffectName", "Effect1");
    /// </summary>
    public IEnumerator SelectEffectTarget(ParameterNode parameterNode)
    {
        yield return null;
    }


    /// <summary>
    /// 变身
    /// </summary>
    public IEnumerator TransformMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];

        for (int i = 0; i < BattleProcess.GetInstance().systemPlayerData.Length; i++)
        {
            PlayerData playerData = BattleProcess.GetInstance().systemPlayerData[i];
            GameObject[] gameObjects = playerData.monsterGameObjectArray;

            for (int j = 0; j < gameObjects.Length; j++)
            {
                if (gameObjects[j] == effectTarget)
                {
                    Destroy(effectTarget);

                    GameObject cardInBattlePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("MonsterInBattlePrefab");
                    GameObject battlePanel = playerData.monsterInBattlePanel[j];
                    GameObject cardInBattle = Instantiate(cardInBattlePrefab, battlePanel.transform);
                    cardInBattle.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
                    yield return cardInBattle;
                    yield return StartCoroutine(cardInBattle.GetComponent<MonsterInBattle>().Generate(cardData));
                    GameObject cardCanvas = cardInBattle.transform.GetChild(0).gameObject;
                    cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                    playerData.monsterGameObjectArray[j] = cardInBattle;

                    yield break;
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// 改变场上怪兽的费用
    /// damageParameter.Add("LaunchedSkill", this);
    /// damageParameter.Add("EffectName", "Effect1");
    /// damageParameter.Add("EffectTarget", effectTarget);
    /// damageParameter.Add("EffectValue", skillValue);
    /// </summary>
    public IEnumerator ChangeMonsterCost(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        int effectValue = (int)parameter["EffectValue"];

        MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();
        int cost = monsterInBattle.GetCost();
        monsterInBattle.SetCost(cost + effectValue);

        yield return null;
    }

    /// <summary>
    /// 改变场上玩家数据里的标记marker
    /// </summary>
    public IEnumerator ChangePlayerDataMarker(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];
        string effectKey = (string)parameter["EffectKey"];
        object effectValue = parameter["EffectValue"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == player)
            {
                battleProcess.systemPlayerData[i].marker[effectKey] = effectValue;
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 添加玩家技能
    /// </summary>
    public IEnumerator AddPlayerSkill(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];
        string skillName = (string)parameter["SkillName"];
        int skillValue = (int)parameter["SkillValue"];
        string source = (string)parameter["Source"];
        object launchedSkill = parameter["LaunchedSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
        var skillClassName = skillConfig["SkillClassName"];
        var skillType = skillConfig["TypeInBattle"];

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                List<SkillInBattle> skillList = playerData.skillList;

                //先看看是否已有这个技能
                foreach (SkillInBattle skillInCard in skillList)
                {
                    if (skillInCard.GetType().Name.Equals(skillClassName))
                    {
                        int currentSkillValue = skillInCard.AddValue(source, skillValue);

                        yield break;
                    }
                }

                Type type = Type.GetType(skillClassName);

                if (type != null)
                {
                    SkillInBattle skill = (SkillInBattle)gameObject.AddComponent(type);

                    skillValue = skill.AddValue(source, skillValue);

                    skillList.Add(skill);

                    //排序
                    skillList.Sort((a, b) => { return Convert.ToInt32(battleProcess.skillPriority[a.GetType().Name]) > Convert.ToInt32(battleProcess.skillPriority[b.GetType().Name]) ? 1 : -1; });
                }

            }
        }
    }
}
