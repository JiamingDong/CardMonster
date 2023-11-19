using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 玩家的动作类
/// </summary>
public class PlayerAction : MonoBehaviour
{
    /// <summary>
    /// 本类实例
    /// </summary>
    private static PlayerAction playerAction;
    /// <summary>
    /// 获得本类实例
    /// </summary>
    /// <returns>本类实例</returns>
    public static PlayerAction GetInstance()
    {
        if (playerAction == null)
        {
            playerAction = GameObject.Find("BattleProcessSystem").GetComponent<PlayerAction>();
        }
        return playerAction;
    }

    /// <summary>
    /// 执行对应动作
    /// </summary>
    /// <param name="effectName">效果方法委托</param>
    /// <param name="parameter">传给效果的参数</param>
    /// <returns></returns>
    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter)
    {
        string fullName = "PlayerAction." + effect.Method.Name;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, effect, parameter, fullName));
        yield return null;
    }

    /// <summary>
    /// 使用一张卡牌，先判断目标是否合法，合法则结算
    /// 怪兽、消耗品、装备，分别结算
    /// battlePanelName 场上位置的名称
    /// handPanelName 手牌位置的名称
    /// cardAttribute 被使用的卡牌属性
    /// </summary>
    public IEnumerator UseACard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        GameAction gameAction = GameAction.GetInstance();
        BattleProcess battleProcess = BattleProcess.GetInstance();

        //使用的卡牌属性
        Dictionary<string, object> cardDataInBattle = (Dictionary<string, object>)parameter["CardDataInBattle"];
        //使用手牌的玩家
        Player player = (Player)parameter["Player"];
        //目标场上位置，012
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        //手牌目标玩家
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        //手牌位置，01怪兽，23道具
        int handPanelNumber = (int)parameter["HandPanelNumber"];

        //获取使用手牌的玩家的PlayerData
        PlayerData useCardPlayerData = new();
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == player)
            {
                useCardPlayerData = battleProcess.systemPlayerData[i];
                break;
            }
        }

        //禁止使用手牌
        useCardPlayerData.canUseHandCard = false;

        //判断目标不符合规则
        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "CardTargetNoncomplianceRule";

        List<string> noncomplianceRuleReasonsList = new();
        //parameterNode1.result.Add("NoncomplianceRuleReasons", noncomplianceRuleReasonsList);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
        yield return null;

        //判断目标符合规则
        ParameterNode parameterNode2 = new();
        parameterNode2.opportunity = "CardTargetComplianceRule";

        Dictionary<string, object> complianceRuleParameter = new();
        //parameterNode2.result.Add("ComplianceRuleReasons", noncomplianceRuleReasonsList);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode2));
        yield return null;

        if (noncomplianceRuleReasonsList.Count > 0)
        {
            useCardPlayerData.canUseHandCard = true;
            yield break;
        }

        //判断费用够不够
        if (useCardPlayerData.surplusCrystal < Convert.ToInt32(cardDataInBattle["CardCost"]))
        {
            useCardPlayerData.canUseHandCard = true;
            yield break;
        }

        //扣除卡牌的费用
        Dictionary<string, object> parameter2 = new();

        parameter2.Add("CrystalAmount", -Convert.ToInt32(cardDataInBattle["CardCost"]));
        parameter2.Add("Player", player);

        yield return StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameter2));
        yield return null;

        //怪兽
        if (cardDataInBattle["CardType"].Equals("monster"))
        {
            //场上生成怪兽
            Dictionary<string, object> monsterParameter = new();
            monsterParameter.Add("PreviousParameter", parameter);
            monsterParameter.Add("CardData", cardDataInBattle);
            monsterParameter.Add("Player", player);
            monsterParameter.Add("BattlePanelNumber", battlePanelNumber);

            yield return StartCoroutine(gameAction.DoAction(gameAction.MonsterEnterBattle, monsterParameter));
            yield return null;

            Debug.Log("场上生成怪兽");

            //卡牌被使用
            ParameterNode parameterNode4 = new();
            parameterNode4.opportunity = "AfterACardBeUsed";

            parameterNode4.parameter.Add("PreviousParameter", parameter);

            yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
            yield return null;

            Debug.Log("卡牌被使用");
        }

        //装备
        if (cardDataInBattle["CardType"].Equals("equip"))
        {
            Dictionary<string, object> monsterParameter = new();
            //monsterParameter.Add("PreviousParameter", parameter);
            monsterParameter.Add("CardData", cardDataInBattle);

            GameObject targetMonster = null;

            if (player == Player.Ally)
            {
                targetMonster = battleProcess.allyPlayerData.monsterGameObjectArray[battlePanelNumber];
            }
            else if (player == Player.Enemy)
            {
                targetMonster = battleProcess.enemyPlayerData.monsterGameObjectArray[battlePanelNumber];
            }

            monsterParameter.Add("EquipmentTarget", targetMonster);

            //场上生成装备
            yield return StartCoroutine(gameAction.DoAction(gameAction.EquipmentEnterBattle, monsterParameter));
            yield return null;

            //卡牌被使用
            ParameterNode parameterNode4 = new();
            parameterNode4.opportunity = "AfterACardBeUsed";

            //parameterNode4.condition.Add("RelevantGameObject", targetMonster);

            parameterNode4.parameter.Add("PreviousParameter", parameter);

            yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
            yield return null;
        }

        //消耗品
        if (cardDataInBattle["CardType"].Equals("consume"))
        {
            //先保存目标怪兽
            GameObject targetMonster = null;

            for (int i = 0, length = battleProcess.systemPlayerData.Length; i < length; i++)
            {
                PlayerData playerData1 = battleProcess.systemPlayerData[i];
                if (playerData1 == useCardPlayerData)
                {
                    if (targetPlayer == Player.Ally)
                    {
                        targetMonster = playerData1.monsterGameObjectArray[battlePanelNumber];
                    }
                    else if (targetPlayer == Player.Enemy)
                    {
                        PlayerData playerData2 = battleProcess.systemPlayerData[(i + 1) % length];
                        targetMonster = playerData2.monsterGameObjectArray[battlePanelNumber];
                    }

                }
            }

            //初始化预制体放入结算区
            GameObject settlementAreaPanel = GameObject.Find("SettlementAreaPanel");
            GameObject cardForShowPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardForShowPrefab");

            GameObject consume = Instantiate(cardForShowPrefab, settlementAreaPanel.transform);
            consume.name = "ConsumeInSettlement";
            consume.GetComponent<Transform>().localPosition = new Vector3(110, -153, 0);

            Dictionary<string, string> cardData = new()
            {
                { "CardID", (string)cardDataInBattle["CardID"] },
                { "CardName", (string)cardDataInBattle["CardName"] },
                { "CardType", (string)cardDataInBattle["CardType"] },
                { "CardKind", (string)cardDataInBattle["CardKind"] },
                { "CardRace", (string)cardDataInBattle["CardRace"] },
                { "CardHP", cardDataInBattle["CardHP"].ToString() },
                { "CardFlags", (string)cardDataInBattle["CardFlags"] },
                { "CardSkinID", (string)cardDataInBattle["CardSkinID"] },
                { "CardCost", cardDataInBattle["CardCost"].ToString() },
                { "CardP", (string)cardDataInBattle["CardP"] },
                { "CardEP", null }
            };
            consume.GetComponent<CardForShow>().SetAllAttribute(cardData);
            consume.AddComponent<ConsumeInBattle>().GenerateConsume(cardData);

            GameObject cardCanvas = consume.transform.Find("Canvas").gameObject;
            RectTransform rectTransform = cardCanvas.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(220.5f, 308);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            //卡牌数据进入玩家信息
            useCardPlayerData.consumeGameObject = consume;

            //卡牌被使用（结算消耗品）
            ParameterNode parameterNode4 = new();
            parameterNode4.opportunity = "AfterACardBeUsed";

            //parameterNode4.condition.Add("RelevantGameObject", consume);

            parameterNode4.parameter.Add("PreviousParameter", parameter);
            parameterNode4.parameter.Add("TargetMonster", targetMonster);
            parameterNode4.parameter.Add("RelevantGameObject", consume);

            yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
            yield return null;

            //卡牌数据移出玩家信息
            useCardPlayerData.consumeGameObject = null;

            //清除生成的物体
            Destroy(consume);
        }

        //销毁对应手牌
        Dictionary<string, object> destroyAHandCardParameter = new();
        destroyAHandCardParameter.Add("PreviousParameter", parameter);
        destroyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
        destroyAHandCardParameter.Add("Player", player);

        yield return StartCoroutine(gameAction.DoAction(gameAction.DestroyAHandCard, destroyAHandCardParameter));
        yield return null;

        //补一张手牌
        Dictionary<string, object> supplyAHandCardParameter = new();
        supplyAHandCardParameter.Add("PreviousParameter", parameter);
        supplyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
        supplyAHandCardParameter.Add("PlayerData", useCardPlayerData);

        yield return StartCoroutine(gameAction.DoAction(gameAction.SupplyAHandCard, supplyAHandCardParameter));
        yield return null;

        //可以使用手牌
        useCardPlayerData.canUseHandCard = true;
        yield return null;
    }

    /// <summary>
    /// 发动英雄技能
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator LaunchHeroSkill(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["PlayerLaunchHeroSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        foreach (PlayerData playerData in battleProcess.systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                GameObject gameObject = playerData.heroSkillGameObject;
                HeroSkillInBattle heroSkillInBattle = gameObject.GetComponent<HeroSkillInBattle>();
                SkillInBattle skillInBattle = heroSkillInBattle.skillList[0];

                ParameterNode parameterNode1 = new();
                parameterNode1.opportunity = "LaunchHeroSkill";

                parameterNode1.parameter.Add("PreviousParameter", parameter);

                yield return StartCoroutine(skillInBattle.ExecuteEligibleEffect(parameterNode1));
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 进入回合战斗阶段
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator StartRoundBattle(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            battleProcess.systemPlayerData[i].canUseHandCard = false;
        }

        //战斗阶段开始前
        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "BeforeRoundBattle";

        parameterNode1.parameter.Add("PreviousParameter", parameter);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
        yield return null;

        battleProcess.gamePhase = GamePhase.RoundBattle;

        //战斗阶段
        ParameterNode parameterNode2 = new();
        parameterNode2.opportunity = "InRoundBattle";

        parameterNode2.parameter.Add("PreviousParameter", parameter);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode2));
        yield return null;

        //进入下个回合
        ParameterNode parameterNode3 = new();
        parameterNode3.opportunity = "EnterRoundReady";

        parameterNode3.parameter.Add("PreviousParameter", parameter);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode3));
        yield return null;
    }
}
