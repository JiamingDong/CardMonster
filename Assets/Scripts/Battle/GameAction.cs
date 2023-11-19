using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ��Ϸ�����еĶ���
/// </summary>
public class GameAction : MonoBehaviour
{
    /// <summary>
    /// �����ʵ��
    /// </summary>
    private static GameAction gameAction;

    /// <summary>
    /// ��ȡ�����ʵ��
    /// </summary>
    /// <returns>����ʵ��</returns>
    public static GameAction GetInstance()
    {
        if (gameAction == null)
        {
            gameAction = GameObject.Find("BattleProcessSystem").GetComponent<GameAction>();
        }
        return gameAction;
    }

    /// <summary>
    /// ʵ�ֶ�ӦЧ����
    /// </summary>
    /// <param name="effect">Ч������ί��</param>
    /// <param name="parameter">����Ч���Ĳ���</param>
    /// <returns></returns>
    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        string effectName = effect.Method.Name;
        string fullName = "GameAction." + effectName;

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, effect, parameter, fullName));

        yield return null;
    }

    /// <summary>
    /// �Ӽ�ˮ��
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator ChangeCrystalAmount(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int crystalAmount = (int)parameter["CrystalAmount"];
        Player player = (Player)parameter["Player"];

        foreach (PlayerData playerMessage in BattleProcess.GetInstance().systemPlayerData)
        {
            if (playerMessage.perspectivePlayer == player)
            {
                playerMessage.surplusCrystal = (playerMessage.surplusCrystal + crystalAmount) > 0 ? playerMessage.surplusCrystal + crystalAmount : 0;

                if (playerMessage.surplusCrystalText != null) playerMessage.surplusCrystalText.text = playerMessage.surplusCrystal.ToString();

                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// ���޽���ս��
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator MonsterEnterBattle(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        Dictionary<string, object> card = (Dictionary<string, object>)parameter["CardData"];

        for (int i = 0; i < BattleProcess.GetInstance().systemPlayerData.Length; i++)
        {
            PlayerData playerData = BattleProcess.GetInstance().systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                GameObject cardInBattlePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("MonsterInBattlePrefab");
                GameObject battlePanel = playerData.monsterInBattlePanel[battlePanelNumber];
                GameObject cardInBattle = Instantiate(cardInBattlePrefab, battlePanel.transform);
                cardInBattle.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
                yield return StartCoroutine(cardInBattle.GetComponent<MonsterInBattle>().GenerateMonster(card));
                GameObject cardCanvas = cardInBattle.transform.GetChild(0).gameObject;
                cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                playerData.monsterGameObjectArray[battlePanelNumber] = cardInBattle;

                parameter.Add("RelevantGameObject", cardInBattle);

                yield return null;
            }
        }

    }

    /// <summary>
    /// װ������ս��
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator EquipmentEnterBattle(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject targetMonster = (GameObject)parameter["EquipmentTarget"];
        Dictionary<string, object> cardData = (Dictionary<string, object>)parameter["CardData"];

        MonsterInBattle monsterInBattle = targetMonster.GetComponent<MonsterInBattle>();
        yield return StartCoroutine(monsterInBattle.AddEquipment(cardData));
        yield return null;
    }

    /// <summary>
    /// ����һ������
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator DestroyAHandCard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int handPanelNumber = (int)parameter["HandPanelNumber"];

        Player player = (Player)parameter["Player"];

        foreach (PlayerData playerData in BattleProcess.GetInstance().systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                switch (handPanelNumber)
                {
                    case 0:
                        playerData.handMonster[0] = null;
                        if (playerData.handMonsterPanel != null)
                        {
                            Destroy(playerData.handMonsterPanel[0].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }
                        break;
                    case 1:
                        playerData.handMonster[1] = null;
                        if (playerData.handMonsterPanel != null)
                        {
                            Destroy(playerData.handMonsterPanel[1].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }
                        break;
                    case 2:
                        playerData.handItem[0] = null;
                        if (playerData.handItemPanel != null)
                        {
                            Destroy(playerData.handItemPanel[0].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }
                        break;
                    case 3:
                        playerData.handItem[1] = null;
                        if (playerData.handItemPanel != null)
                        {
                            Destroy(playerData.handItemPanel[1].GetComponent<Transform>().Find("CardForShow").gameObject);
                        }
                        break;
                }
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// ��һ������
    /// handPanelNumber ����λ�õ���ţ�01���ޣ�23����
    /// player �ĸ����
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator SupplyAHandCard(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int handPanelNumber = (int)parameter["HandPanelNumber"];
        PlayerData playerData = (PlayerData)parameter["PlayerData"];

        switch (handPanelNumber)
        {
            case 0:
                if (playerData.monsterDeck.Count > 0)
                {
                    playerData.handMonster[0] = new(playerData.monsterDeck[0]);
                    playerData.monsterDeck.RemoveAt(0);
                    playerData.surplusMonsterAmountText.text = playerData.monsterDeck.Count.ToString();
                    if (playerData.handMonsterPanel != null) LoadACardToHand(playerData.handMonster[0], playerData.handMonsterPanel[0]);
                }
                break;
            case 1:
                if (playerData.monsterDeck.Count > 0)
                {
                    playerData.handMonster[1] = new(playerData.monsterDeck[0]);
                    playerData.monsterDeck.RemoveAt(0);
                    playerData.surplusMonsterAmountText.text = playerData.monsterDeck.Count.ToString();
                    if (playerData.handMonsterPanel != null) LoadACardToHand(playerData.handMonster[1], playerData.handMonsterPanel[1]);
                }
                break;
            case 2:
                if (playerData.itemDeck.Count > 0)
                {
                    playerData.handItem[0] = new(playerData.itemDeck[0]);
                    playerData.itemDeck.RemoveAt(0);
                    playerData.surplusItemAmountText.text = playerData.itemDeck.Count.ToString();
                    if (playerData.handItemPanel != null) LoadACardToHand(playerData.handItem[0], playerData.handItemPanel[0]);
                }
                break;
            case 3:
                if (playerData.itemDeck.Count > 0)
                {
                    playerData.handItem[1] = new(playerData.itemDeck[0]);
                    playerData.itemDeck.RemoveAt(0);
                    playerData.surplusItemAmountText.text = playerData.itemDeck.Count.ToString();
                    if (playerData.handItemPanel != null) LoadACardToHand(playerData.handItem[1], playerData.handItemPanel[1]);
                }
                break;
        }

        yield return null;
    }

    /// <summary>
    /// ��������ͼ��
    /// </summary>
    /// <param name="card">��������</param>
    /// <param name="panel">���ڵ�λ��</param>
    private void LoadACardToHand(Dictionary<string, string> card, GameObject panel)
    {
        //Debug.Log("��������ͼ��");
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
    /// ��������ϵ�װ��
    /// </summary>
    public IEnumerator DestroyEquipment(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monster = (GameObject)parameter["MonsterBeDestroyEquipment"];

        MonsterInBattle monsterInBattle = monster.GetComponent<MonsterInBattle>();
        monsterInBattle.equipment = null;
        monsterInBattle.EquipmentBackgroundLImage.enabled = false;
        monsterInBattle.EquipmentBackgroundLImage.texture = null;
        monsterInBattle.EquipmentBackgroundRImage.enabled = false;
        monsterInBattle.EquipmentBackgroundRImage.texture = null;
        monsterInBattle.EquipmentImage.enabled = false;
        monsterInBattle.EquipmentImage.texture = null;
        monsterInBattle.ArmorImage.enabled = false;
        monsterInBattle.ArmorText.enabled = false;
        monsterInBattle.ArmorText.text = null;

        yield return null;
    }

    /// <summary>
    /// �������
    /// </summary>
    public IEnumerator DestroyMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeDestroy = (GameObject)parameter["MonsterBeDestroy"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                GameObject[] monsterGameObjectArray = systemPlayerData.monsterGameObjectArray;
                if (monsterGameObjectArray[j] == monsterBeDestroy)
                {
                    Destroy(monsterBeDestroy);
                    monsterGameObjectArray[j] = null;

                    //�ƶ�����Ĺ���
                    for (int k = j + 1; k < monsterGameObjectArray.Length; k++)
                    {
                        Dictionary<string, object> moveParameter = new();
                        moveParameter.Add("MonsterMove", monsterGameObjectArray[k]);
                        moveParameter.Add("TargetPosition", k - 1);
                        moveParameter.Add("Player", systemPlayerData.perspectivePlayer);
                        yield return StartCoroutine(DoAction(MoveMonster, moveParameter));
                        yield return null;
                    }
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// �Թ�������˺�
    /// </summary>
    public IEnumerator HurtMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        int damageValue = (int)parameter["DamageValue"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        MonsterInBattle monsterInBattle = skillInBattle.gameObject.GetComponent<MonsterInBattle>();

        damageValue = damageValue > 0 ? damageValue : 0;

        monsterInBattle.currentHp -= damageValue;
        monsterInBattle.LifeText.text = monsterInBattle.currentHp.ToString();
        if (monsterInBattle.currentHp < 1)
        {
            Dictionary<string, object> destroyParameter = new();
            destroyParameter.Add("MonsterBeDestroy", monsterBeHurt);
            destroyParameter.Add("Destroyer", skillInBattle.gameObject);
            yield return StartCoroutine(DoAction(DestroyMonster, destroyParameter));
        }

        yield return null;
    }

    /// <summary>
    /// �ƶ����Ϲ���
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator MoveMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        //�ƶ��Ĺ���
        GameObject monsterMove = (GameObject)parameter["MonsterMove"];
        //�ƶ����λ��
        int targetPosition = (int)parameter["TargetPosition"];
        //�ĸ�����ƶ�
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer == player)
            {
                GameObject[] monsterGameObjectArray = systemPlayerData.monsterGameObjectArray;
                if (monsterGameObjectArray[targetPosition] == null)
                {
                    //���������ƶ�
                    for (int j = 0; j < 3; j++)
                    {
                        if (monsterGameObjectArray[j] == monsterMove)
                        {
                            monsterGameObjectArray[targetPosition] = monsterMove;
                            monsterGameObjectArray[j] = null;
                            break;
                        }
                    }
                    //�����ϵ��ƶ�
                    monsterMove.transform.parent = systemPlayerData.monsterInBattlePanel[targetPosition].transform;
                }
            }
        }


        yield return null;
    }

    /// <summary>
    /// ���ƹ���
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator TreatMonster(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeTreat = (GameObject)parameter["MonsterBeTreat"];
        int treatValue = (int)parameter["TreatValue"];

        treatValue = treatValue > 0 ? treatValue : 0;

        MonsterInBattle monsterInBattle = monsterBeTreat.GetComponent<MonsterInBattle>();
        monsterInBattle.currentHp = monsterInBattle.currentHp + treatValue > monsterInBattle.maximumHp ? monsterInBattle.maximumHp : monsterInBattle.currentHp + treatValue;

        yield return null;
    }

    /// <summary>
    /// �׼�
    /// ObjectBeSacrificedNumber ����λ�õ���ţ�01���ޣ�23���ߣ�456���Ϲ���
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator Sacrifice(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                //���׼��������ƹ���
                if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber <= 1)
                {
                    //���ٶ�Ӧ����
                    Dictionary<string, object> destroyAHandCardParameter = new();
                    destroyAHandCardParameter.Add("PreviousParameter", parameter);
                    destroyAHandCardParameter.Add("HandPanelNumber", objectBeSacrificedNumber);
                    destroyAHandCardParameter.Add("Player", player);

                    yield return StartCoroutine(gameAction.DoAction(gameAction.DestroyAHandCard, destroyAHandCardParameter));
                    yield return null;

                    //��һ������
                    Dictionary<string, object> supplyAHandCardParameter = new();
                    supplyAHandCardParameter.Add("PreviousParameter", parameter);
                    supplyAHandCardParameter.Add("HandPanelNumber", objectBeSacrificedNumber);
                    supplyAHandCardParameter.Add("PlayerData", systemPlayerData);

                    yield return StartCoroutine(gameAction.DoAction(gameAction.SupplyAHandCard, supplyAHandCardParameter));
                    yield return null;

                    //���ˮ��
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("CrystalAmount", 2);
                    parameter1.Add("Player", player);
                    yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameter1));
                    yield return null;
                }

                //���׼���������װ��
                if (objectBeSacrificedNumber >= 2 && objectBeSacrificedNumber <= 3)
                {
                    //���ٶ�Ӧ����
                    Dictionary<string, object> destroyAHandCardParameter = new();
                    destroyAHandCardParameter.Add("PreviousParameter", parameter);
                    destroyAHandCardParameter.Add("HandPanelNumber", objectBeSacrificedNumber);
                    destroyAHandCardParameter.Add("Player", player);

                    yield return StartCoroutine(gameAction.DoAction(gameAction.DestroyAHandCard, destroyAHandCardParameter));
                    yield return null;

                    //��һ������
                    Dictionary<string, object> supplyAHandCardParameter = new();
                    supplyAHandCardParameter.Add("PreviousParameter", parameter);
                    supplyAHandCardParameter.Add("HandPanelNumber", objectBeSacrificedNumber);
                    supplyAHandCardParameter.Add("PlayerData", systemPlayerData);

                    yield return StartCoroutine(gameAction.DoAction(gameAction.SupplyAHandCard, supplyAHandCardParameter));
                    yield return null;

                    //���ˮ��
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("CrystalAmount", 1);
                    parameter1.Add("Player", player);
                    yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameter1));
                    yield return null;
                }

                //���׼����ǳ��Ϲ���
                if (objectBeSacrificedNumber >= 4 && objectBeSacrificedNumber <= 6)
                {
                    int t = objectBeSacrificedNumber - 4;

                    GameObject[] monsterGameObjectArray = systemPlayerData.monsterGameObjectArray;

                    Destroy(monsterGameObjectArray[t]);
                    monsterGameObjectArray[t] = null;

                    //�ƶ�����Ĺ���
                    for (int k = t + 1; k < monsterGameObjectArray.Length; k++)
                    {
                        Dictionary<string, object> moveParameter = new();
                        moveParameter.Add("MonsterMove", monsterGameObjectArray[k]);
                        moveParameter.Add("TargetPosition", k - 1);
                        moveParameter.Add("Player", systemPlayerData.perspectivePlayer);
                        yield return StartCoroutine(DoAction(MoveMonster, moveParameter));
                        yield return null;
                    }

                    //���ˮ��
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("CrystalAmount", 2);
                    parameter1.Add("Player", systemPlayerData.perspectivePlayer);
                    yield return StartCoroutine(DoAction(ChangeCrystalAmount, parameter1));
                    yield return null;
                }
            }
        }

        yield return null;
    }


}
