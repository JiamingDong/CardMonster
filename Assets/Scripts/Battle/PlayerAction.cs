using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ��ҵĶ�����
/// </summary>
public class PlayerAction : MonoBehaviour
{
    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static PlayerAction playerAction;
    /// <summary>
    /// ��ñ���ʵ��
    /// </summary>
    /// <returns>����ʵ��</returns>
    public static PlayerAction GetInstance()
    {
        if (playerAction == null)
        {
            playerAction = GameObject.Find("BattleProcessSystem").GetComponent<PlayerAction>();
        }
        return playerAction;
    }

    /// <summary>
    /// ִ�ж�Ӧ����
    /// </summary>
    /// <param name="effectName">Ч������ί��</param>
    /// <param name="parameter">����Ч���Ĳ���</param>
    /// <returns></returns>
    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter)
    {
        string fullName = "PlayerAction." + effect.Method.Name;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, effect, parameter, fullName));
        yield return null;
    }

    /// <summary>
    /// ʹ��һ�ſ��ƣ����ж�Ŀ���Ƿ�Ϸ����Ϸ������
    /// ���ޡ�����Ʒ��װ�����ֱ����
    /// battlePanelName ����λ�õ�����
    /// handPanelName ����λ�õ�����
    /// cardAttribute ��ʹ�õĿ�������
    /// </summary>
    public IEnumerator UseACard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        GameAction gameAction = GameAction.GetInstance();
        BattleProcess battleProcess = BattleProcess.GetInstance();

        //ʹ�õĿ�������
        Dictionary<string, object> cardDataInBattle = (Dictionary<string, object>)parameter["CardDataInBattle"];
        //ʹ�����Ƶ����
        Player player = (Player)parameter["Player"];
        //Ŀ�곡��λ�ã�012
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        //����Ŀ�����
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        //����λ�ã�01���ޣ�23����
        int handPanelNumber = (int)parameter["HandPanelNumber"];

        //��ȡʹ�����Ƶ���ҵ�PlayerData
        PlayerData useCardPlayerData = new();
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == player)
            {
                useCardPlayerData = battleProcess.systemPlayerData[i];
                break;
            }
        }

        //��ֹʹ������
        useCardPlayerData.canUseHandCard = false;

        //�ж�Ŀ�겻���Ϲ���
        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "CardTargetNoncomplianceRule";

        List<string> noncomplianceRuleReasonsList = new();
        //parameterNode1.result.Add("NoncomplianceRuleReasons", noncomplianceRuleReasonsList);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
        yield return null;

        //�ж�Ŀ����Ϲ���
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

        //�жϷ��ù�����
        if (useCardPlayerData.surplusCrystal < Convert.ToInt32(cardDataInBattle["CardCost"]))
        {
            useCardPlayerData.canUseHandCard = true;
            yield break;
        }

        //�۳����Ƶķ���
        Dictionary<string, object> parameter2 = new();

        parameter2.Add("CrystalAmount", -Convert.ToInt32(cardDataInBattle["CardCost"]));
        parameter2.Add("Player", player);

        yield return StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameter2));
        yield return null;

        //����
        if (cardDataInBattle["CardType"].Equals("monster"))
        {
            //�������ɹ���
            Dictionary<string, object> monsterParameter = new();
            monsterParameter.Add("PreviousParameter", parameter);
            monsterParameter.Add("CardData", cardDataInBattle);
            monsterParameter.Add("Player", player);
            monsterParameter.Add("BattlePanelNumber", battlePanelNumber);

            yield return StartCoroutine(gameAction.DoAction(gameAction.MonsterEnterBattle, monsterParameter));
            yield return null;

            Debug.Log("�������ɹ���");

            //���Ʊ�ʹ��
            ParameterNode parameterNode4 = new();
            parameterNode4.opportunity = "AfterACardBeUsed";

            parameterNode4.parameter.Add("PreviousParameter", parameter);

            yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
            yield return null;

            Debug.Log("���Ʊ�ʹ��");
        }

        //װ��
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

            //��������װ��
            yield return StartCoroutine(gameAction.DoAction(gameAction.EquipmentEnterBattle, monsterParameter));
            yield return null;

            //���Ʊ�ʹ��
            ParameterNode parameterNode4 = new();
            parameterNode4.opportunity = "AfterACardBeUsed";

            //parameterNode4.condition.Add("RelevantGameObject", targetMonster);

            parameterNode4.parameter.Add("PreviousParameter", parameter);

            yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
            yield return null;
        }

        //����Ʒ
        if (cardDataInBattle["CardType"].Equals("consume"))
        {
            //�ȱ���Ŀ�����
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

            //��ʼ��Ԥ������������
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

            //�������ݽ��������Ϣ
            useCardPlayerData.consumeGameObject = consume;

            //���Ʊ�ʹ�ã���������Ʒ��
            ParameterNode parameterNode4 = new();
            parameterNode4.opportunity = "AfterACardBeUsed";

            //parameterNode4.condition.Add("RelevantGameObject", consume);

            parameterNode4.parameter.Add("PreviousParameter", parameter);
            parameterNode4.parameter.Add("TargetMonster", targetMonster);
            parameterNode4.parameter.Add("RelevantGameObject", consume);

            yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
            yield return null;

            //���������Ƴ������Ϣ
            useCardPlayerData.consumeGameObject = null;

            //������ɵ�����
            Destroy(consume);
        }

        //���ٶ�Ӧ����
        Dictionary<string, object> destroyAHandCardParameter = new();
        destroyAHandCardParameter.Add("PreviousParameter", parameter);
        destroyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
        destroyAHandCardParameter.Add("Player", player);

        yield return StartCoroutine(gameAction.DoAction(gameAction.DestroyAHandCard, destroyAHandCardParameter));
        yield return null;

        //��һ������
        Dictionary<string, object> supplyAHandCardParameter = new();
        supplyAHandCardParameter.Add("PreviousParameter", parameter);
        supplyAHandCardParameter.Add("HandPanelNumber", handPanelNumber);
        supplyAHandCardParameter.Add("PlayerData", useCardPlayerData);

        yield return StartCoroutine(gameAction.DoAction(gameAction.SupplyAHandCard, supplyAHandCardParameter));
        yield return null;

        //����ʹ������
        useCardPlayerData.canUseHandCard = true;
        yield return null;
    }

    /// <summary>
    /// ����Ӣ�ۼ���
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
    /// ����غ�ս���׶�
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

        //ս���׶ο�ʼǰ
        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "BeforeRoundBattle";

        parameterNode1.parameter.Add("PreviousParameter", parameter);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
        yield return null;

        battleProcess.gamePhase = GamePhase.RoundBattle;

        //ս���׶�
        ParameterNode parameterNode2 = new();
        parameterNode2.opportunity = "InRoundBattle";

        parameterNode2.parameter.Add("PreviousParameter", parameter);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode2));
        yield return null;

        //�����¸��غ�
        ParameterNode parameterNode3 = new();
        parameterNode3.opportunity = "EnterRoundReady";

        parameterNode3.parameter.Add("PreviousParameter", parameter);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode3));
        yield return null;
    }
}
