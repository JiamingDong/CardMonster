using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 拖动手牌，符合要求就使用
/// player，Player，使用手牌的玩家
/// card，Card，使用的卡牌属性
/// battlePanelNumber，int，目标场上位置，012我方，345对方
/// handPanelNumber，int，手牌位置，01怪兽，23道具
/// </summary>
public class DragHandCard : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    Vector3 transformPosition;
    int transformSiblingIndex;
    int transformParentSiblingIndex;

    bool isDragging = false;

    /// <summary>
    /// 鼠标按下，记录手牌原位置
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        if (battleProcess.allyPlayerData.perspectivePlayer == Player.Ally && eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;

            //Debug.Log("DragHandCard.OnPointerDown:鼠标按下。");
            transformPosition = transform.position;
            transformSiblingIndex = transform.GetSiblingIndex();
            transformParentSiblingIndex = transform.parent.GetSiblingIndex();

            transform.parent.SetAsLastSibling();
            transform.SetAsLastSibling();
            transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// 拖动中，让手牌跟着鼠标
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        if (battleProcess.allyPlayerData.perspectivePlayer == Player.Ally && eventData.button == PointerEventData.InputButton.Left)
        {
            gameObject.GetComponent<Transform>().position = Input.mousePosition;
        }
    }

    /// <summary>
    /// 鼠标按键抬起时，判断是不是有效使用牌
    /// 先把手牌图像放回原位，然后判断
    /// 怪兽：己方场上有空位，放的位置是第一个或上一个位置有怪兽
    /// 装备：放的位置有我方怪兽
    /// 消耗品：放的位置有怪兽
    /// 然后去PlayerAction的UseACard(Dictionary<string, object> parameter)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("DragHandCard.OnPointerUp:鼠标抬起。");
        if (!isDragging)
        {
            return;
        }
        isDragging = false;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        transform.position = transformPosition;
        transform.SetSiblingIndex(transformSiblingIndex);
        transform.parent.SetSiblingIndex(transformParentSiblingIndex);

        //处于不能使用手牌的状态，则返回
        if (battleProcess.allyPlayerData.perspectivePlayer != Player.Ally || battleProcess.allyPlayerData.canUseHandCard == false)
            return;

        battleProcess.allyPlayerData.canUseHandCard = false;

        Dictionary<string, object> parameter = new();
        parameter.Add("Player", Player.Ally);

        //判断鼠标是否把手牌拖到场上怪兽的位置
        Player targetPlayer;
        Vector3 mousePosition = Input.mousePosition;

        int battlePanelNumber;
        for (int i = 0; i < 3; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(battleProcess.allyPlayerData.monsterInBattlePanel[i].GetComponent<RectTransform>(), mousePosition))
            {
                battlePanelNumber = i;
                parameter.Add("BattlePanelNumber", i);
                parameter.Add("TargetPlayer", Player.Ally);
                targetPlayer = Player.Ally;
                goto a;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(battleProcess.enemyPlayerData.monsterInBattlePanel[i].GetComponent<RectTransform>(), mousePosition))
            {
                battlePanelNumber = i + 3;
                parameter.Add("BattlePanelNumber", i);
                parameter.Add("TargetPlayer", Player.Enemy);
                targetPlayer = Player.Enemy;
                goto a;
            }
        }
        battleProcess.allyPlayerData.canUseHandCard = true;
        return;
    //标签a
    a:;

        //添加使用的是哪个位置的手牌
        int handPanelNumber = -1;
        for (int i = 0; i < 2; i++)
        {
            if (transform.parent.gameObject == battleProcess.allyPlayerData.handMonsterPanel[i])
            {
                handPanelNumber = i;

                parameter.Add("HandPanelNumber", i);
                goto b;
            }

            if (transform.parent.gameObject == battleProcess.allyPlayerData.handItemPanel[i])
            {
                handPanelNumber = i + 2;

                parameter.Add("HandPanelNumber", i + 2);
                goto b;
            }
        }
    //标签b
    b:;

        //Debug.Log("handPanelNumber=" + handPanelNumber);
        //Debug.Log("battlePanelNumber=" + battlePanelNumber);
        //从手牌位置判定手牌是怪兽，且场上有空位
        if ((handPanelNumber == 0 || handPanelNumber == 1) && battlePanelNumber < 3 && battleProcess.allyPlayerData.monsterGameObjectArray[2] == null)
        {
            //Debug.Log("DragHandCard.OnPointerUp:手牌是怪兽，且场上有空位");
            goto c;
        }
        //是道具
        else if (handPanelNumber == 2 || handPanelNumber == 3)
        {
            //是消耗品
            if (transform.GetComponent<CardForShow>().type.Equals("consume"))
            {
                if (targetPlayer == Player.Ally)
                {
                    GameObject monsterGameObject1 = battleProcess.allyPlayerData.monsterGameObjectArray[battlePanelNumber];
                    if (monsterGameObject1 != null)
                    {
                        //Debug.Log("DragHandCard.OnPointerUp:手牌是消耗品，目标是我方怪兽");
                        goto c;
                    }
                }

                if (targetPlayer == Player.Enemy)
                {
                    GameObject monsterGameObject2 = battleProcess.enemyPlayerData.monsterGameObjectArray[battlePanelNumber - 3];
                    if (monsterGameObject2 != null)
                    {
                        //Debug.Log("DragHandCard.OnPointerUp:手牌是消耗品，目标是对方怪兽");
                        goto c;
                    }
                }
            }
            //是装备
            else if (transform.GetComponent<CardForShow>().type.Equals("equip"))
            {
                if (battlePanelNumber < 3)
                {
                    GameObject monsterGameObject1 = battleProcess.allyPlayerData.monsterGameObjectArray[battlePanelNumber];
                    if (monsterGameObject1 != null)
                    {
                        //Debug.Log("DragHandCard.OnPointerUp:手牌是装备，且有我方怪兽");
                        goto c;
                    }
                }
            }
        }
        battleProcess.allyPlayerData.canUseHandCard = true;
        return;

    //标签c
    c:;

        CardForShow cardForShow = transform.GetComponent<CardForShow>();
        string flags = cardForShow.flags;
        //如果是精英卡
        if (flags != null && flags != "" && flags.Contains("\"2\""))
        {
            //创建选精英阵营的界面
            GameObject selectEliteKindPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("SelectEliteKindPrefab");
            GameObject battleSceneCanvas = GameObject.Find("BattleSceneCanvas");
            GameObject selectEliteKindPrefabInstantiation = Instantiate(selectEliteKindPrefab, battleSceneCanvas.transform);
            selectEliteKindPrefabInstantiation.name = "SelectEliteKindPrefabInstantiation";
            selectEliteKindPrefabInstantiation.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

            GameObject selectEliteKindCanvas = selectEliteKindPrefabInstantiation.transform.Find("SelectEliteKindCanvas").gameObject;
            selectEliteKindCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            selectEliteKindCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            selectEliteKindCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            //加载两张卡上的图像和选择点击事件
            Dictionary<string, string> cardData = new();
            cardData.Add("CardID", cardForShow.id);
            cardData.Add("CardName", cardForShow.cardName);
            cardData.Add("CardType", cardForShow.type);
            cardData.Add("CardKind", cardForShow.kind);
            cardData.Add("CardRace", cardForShow.race);
            cardData.Add("CardHP", cardForShow.hp.ToString());
            cardData.Add("CardFlags", cardForShow.flags);
            cardData.Add("CardSkinID", cardForShow.skinID);
            cardData.Add("CardCost", cardForShow.cost.ToString());
            cardData.Add("CardSkill", cardForShow.skill);
            cardData.Add("CardEliteSkill", cardForShow.eliteSkill);

            string cardEP = cardData["CardEliteSkill"];
            string cardKind = cardData["CardKind"];

            Dictionary<string, object> ePD = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardEP);
            JObject leftSkill = (JObject)ePD["leftSkill"];
            JObject rightSkill = (JObject)ePD["rightSkill"];

            //{ "leftKind":"fortune","rightKind":"balance"}
            Dictionary<string, string> cardKindD = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardKind);
            string leftKind = cardKindD["leftKind"];
            string rightKind = cardKindD["rightKind"];
            //Debug.Log(leftKind);
            //Debug.Log(rightKind);

            Dictionary<string, string> cardDataL = new(cardData);
            Dictionary<string, string> cardDataR = new(cardData);

            //左
            Transform eliteLCardForShowPrefab = selectEliteKindCanvas.transform.Find("EliteLCardForShowPrefab");
            CardForShow eliteLCardForShow = eliteLCardForShowPrefab.gameObject.GetComponent<CardForShow>();

            Dictionary<string, object> ePDL = new();
            ePDL.Add("leftSkill", leftSkill);
            cardDataL["CardEliteSkill"] = JsonConvert.SerializeObject(ePDL);

            Dictionary<string, string> cardKindDL = new();
            cardKindDL.Add("leftKind", leftKind);
            cardDataL["CardKind"] = JsonConvert.SerializeObject(cardKindDL);
            //Debug.Log(cardData["CardKind"]);

            eliteLCardForShow.SetAllAttribute(cardDataL);

            SelectEliteKind selectEliteKindL = eliteLCardForShowPrefab.Find("Canvas").gameObject.GetComponent<SelectEliteKind>();
            selectEliteKindL.parameter = parameter;
            selectEliteKindL.cardIndex = 0;

            //右
            Transform eliteRCardForShowPrefab = selectEliteKindCanvas.transform.Find("EliteRCardForShowPrefab");
            CardForShow eliteRCardForShow = eliteRCardForShowPrefab.gameObject.GetComponent<CardForShow>();

            Dictionary<string, object> ePDR = new();
            ePDR.Add("leftSkill", rightSkill);
            cardDataR["CardEliteSkill"] = JsonConvert.SerializeObject(ePDR);

            Dictionary<string, string> cardKindDR = new();
            cardKindDR.Add("leftKind", rightKind);
            cardDataR["CardKind"] = JsonConvert.SerializeObject(cardKindDR);
            //Debug.Log(cardData["CardKind"]);

            eliteRCardForShow.SetAllAttribute(cardDataR);

            SelectEliteKind selectEliteKindR = eliteRCardForShowPrefab.Find("Canvas").gameObject.GetComponent<SelectEliteKind>();
            selectEliteKindR.parameter = parameter;
            selectEliteKindR.cardIndex = 1;
        }
        else
        {
            ParameterNode parameterNode1 = new();
            parameterNode1.opportunity = "CheckCardTarget";
            parameterNode1.parameter = parameter;

            SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.UseHandCard, parameter));

            battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
        }
    }
}