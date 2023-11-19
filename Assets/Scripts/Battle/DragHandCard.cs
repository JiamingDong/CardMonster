using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �϶����ƣ�����Ҫ���ʹ��
/// player��Player��ʹ�����Ƶ����
/// card��Card��ʹ�õĿ�������
/// battlePanelNumber��int��Ŀ�곡��λ�ã�012�ҷ���345�Է�
/// handPanelNumber��int������λ�ã�01���ޣ�23����
/// </summary>
public class DragHandCard : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    Vector3 transformPosition;
    int transformSiblingIndex;
    int transformParentSiblingIndex;

    /// <summary>
    /// ��갴�£���¼����ԭλ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        if (battleProcess.allyPlayerData.perspectivePlayer == Player.Ally && eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("DragHandCard.OnPointerDown:��갴�¡�");
            transformPosition = transform.position;
            transformSiblingIndex = transform.GetSiblingIndex();
            transformParentSiblingIndex = transform.parent.GetSiblingIndex();

            transform.parent.SetAsLastSibling();
            transform.SetAsLastSibling();
            transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// �϶��У������Ƹ������
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
    /// ��갴��̧��ʱ���ж��ǲ�����Чʹ����
    /// �Ȱ�����ͼ��Ż�ԭλ��Ȼ���ж�
    /// ���ޣ����������п�λ���ŵ�λ���ǵ�һ������һ��λ���й���
    /// װ�����ŵ�λ�����ҷ�����
    /// ����Ʒ���ŵ�λ���й���
    /// Ȼ��ȥPlayerAction��UseACard(Dictionary<string, object> parameter)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("DragHandCard.OnPointerUp:���̧��");

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //���ڲ���ʹ�����Ƶ�״̬���򷵻�
        if (battleProcess.allyPlayerData.perspectivePlayer != Player.Ally || battleProcess.allyPlayerData.canUseHandCard == false)
            return;

        PlayerAction playerAction = PlayerAction.GetInstance();

        transform.position = transformPosition;
        transform.SetSiblingIndex(transformSiblingIndex);
        transform.parent.SetSiblingIndex(transformParentSiblingIndex);

        ParameterNode parameterNode = new();

        parameterNode.parameter.Add("Player", Player.Ally);

        //�ж�����Ƿ�������ϵ����Ϲ��޵�λ��
        Player targetPlayer;

        int battlePanelNumber = -1;
        Vector3 mousePosition = Input.mousePosition;
        for (int i = 0; i < 3; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(battleProcess.allyPlayerData.monsterInBattlePanel[i].GetComponent<RectTransform>(), mousePosition))
            {
                battlePanelNumber = i;
                parameterNode.parameter.Add("BattlePanelNumber", i);
                parameterNode.parameter.Add("TargetPlayer", Player.Ally);
                targetPlayer = Player.Ally;
                goto a;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(battleProcess.enemyPlayerData.monsterInBattlePanel[i].GetComponent<RectTransform>(), mousePosition))
            {
                battlePanelNumber = i + 3;
                parameterNode.parameter.Add("BattlePanelNumber", i);
                parameterNode.parameter.Add("TargetPlayer", Player.Enemy);
                targetPlayer = Player.Enemy;
                goto a;
            }
        }
        return;
    //��ǩa
    a:;

        //���ʹ�õ����ĸ�λ�õ�����
        int handPanelNumber = -1;
        for (int i = 0; i < 2; i++)
        {
            if (transform.parent.gameObject == battleProcess.allyPlayerData.handMonsterPanel[i])
            {
                handPanelNumber = i;

                parameterNode.parameter.Add("HandPanelNumber", i);
                goto b;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            if (transform.parent.gameObject == battleProcess.allyPlayerData.handItemPanel[i])
            {
                handPanelNumber = i + 2;

                parameterNode.parameter.Add("HandPanelNumber", i + 2);
                goto b;
            }
        }
    //��ǩb
    b:;

        Debug.Log("handPanelNumber=" + handPanelNumber);
        Debug.Log("battlePanelNumber=" + battlePanelNumber);
        //������λ���ж������ǹ��ޣ��ҳ����п�λ
        if ((handPanelNumber == 0 || handPanelNumber == 1) && battlePanelNumber < 3 && battleProcess.allyPlayerData.monsterGameObjectArray[2] == null)
        {
            Debug.Log("DragHandCard.OnPointerUp:�����ǹ��ޣ��ҳ����п�λ");
            goto c;
        }
        //�ǵ���
        else if (handPanelNumber == 2 || handPanelNumber == 3)
        {
            //������Ʒ
            if (transform.GetComponent<CardForShow>().type.Equals("consume"))
            {
                if (targetPlayer == Player.Ally)
                {
                    GameObject monsterGameObject1 = battleProcess.allyPlayerData.monsterGameObjectArray[battlePanelNumber];
                    if (monsterGameObject1 != null)
                    {
                        Debug.Log("DragHandCard.OnPointerUp:����������Ʒ��Ŀ�����ҷ�����");
                        goto c;
                    }
                }

                if (targetPlayer == Player.Enemy)
                {
                    GameObject monsterGameObject2 = battleProcess.enemyPlayerData.monsterGameObjectArray[battlePanelNumber];
                    if (monsterGameObject2 != null)
                    {
                        Debug.Log("DragHandCard.OnPointerUp:����������Ʒ��Ŀ���ǶԷ�����");
                        goto c;
                    }
                }
            }
            //��װ��
            else if (transform.GetComponent<CardForShow>().type.Equals("equip"))
            {
                if (battlePanelNumber < 3)
                {
                    GameObject monsterGameObject1 = battleProcess.allyPlayerData.monsterGameObjectArray[battlePanelNumber];
                    if (monsterGameObject1 != null)
                    {
                        Debug.Log("DragHandCard.OnPointerUp:������װ���������ҷ�����");
                        goto c;
                    }
                }
            }
        }
        return;

    //��ǩc
    c:;

        CardForShow cardForShow = transform.GetComponent<CardForShow>();

        //����Ǿ�Ӣ��
        if (cardForShow.flags.Contains("\"2\""))
        {
            //����ѡ��Ӣ��Ӫ�Ľ���
            GameObject selectEliteKindPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("SelectEliteKindPrefab");
            GameObject battleSceneCanvas = GameObject.Find("BattleSceneCanvas");
            GameObject selectEliteKindPrefabInstantiation = Instantiate(selectEliteKindPrefab, battleSceneCanvas.transform);
            selectEliteKindPrefabInstantiation.name = "SelectEliteKindPrefabInstantiation";
            selectEliteKindPrefabInstantiation.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

            GameObject selectEliteKindCanvas = selectEliteKindPrefabInstantiation.transform.Find("SelectEliteKindCanvas").gameObject;
            selectEliteKindCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            selectEliteKindCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            selectEliteKindCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            //�������ſ��ϵ�ͼ���ѡ�����¼�
            Dictionary<string, string> cardData = cardForShow.GetCardData();

            string cardEP = cardData["CardEP"];
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

            //��
            Transform eliteLCardForShowPrefab = selectEliteKindCanvas.transform.Find("EliteLCardForShowPrefab");
            CardForShow eliteLCardForShow = eliteLCardForShowPrefab.gameObject.GetComponent<CardForShow>();

            Dictionary<string, object> ePDL = new();
            ePDL.Add("leftSkill", leftSkill);
            cardDataL["CardEP"] = JsonConvert.SerializeObject(ePDL);

            Dictionary<string, string> cardKindDL = new();
            cardKindDL.Add("leftKind", leftKind);
            cardDataL["CardKind"] = JsonConvert.SerializeObject(cardKindDL);
            //Debug.Log(cardData["CardKind"]);

            eliteLCardForShow.SetAllAttribute(cardDataL);

            SelectEliteKind selectEliteKindL = eliteLCardForShowPrefab.Find("Canvas").gameObject.GetComponent<SelectEliteKind>();
            selectEliteKindL.parameterNode = parameterNode;

            //��
            Transform eliteRCardForShowPrefab = selectEliteKindCanvas.transform.Find("EliteRCardForShowPrefab");
            CardForShow eliteRCardForShow = eliteRCardForShowPrefab.gameObject.GetComponent<CardForShow>();

            Dictionary<string, object> ePDR = new();
            ePDR.Add("leftSkill", rightSkill);
            cardDataR["CardEP"] = JsonConvert.SerializeObject(ePDR);

            Dictionary<string, string> cardKindDR = new();
            cardKindDR.Add("leftKind", rightKind);
            cardDataR["CardKind"] = JsonConvert.SerializeObject(cardKindDR);
            //Debug.Log(cardData["CardKind"]);

            eliteRCardForShow.SetAllAttribute(cardDataR);

            SelectEliteKind selectEliteKindR = eliteRCardForShowPrefab.Find("Canvas").gameObject.GetComponent<SelectEliteKind>();
            selectEliteKindR.parameterNode = parameterNode;
        }
        else
        {
            Dictionary<string, string> cardData = cardForShow.GetCardData();

            string cardP = cardData["CardP"];
            JObject cardPD = JsonConvert.DeserializeObject<JObject>(cardP);

            Dictionary<string, int> cardPDictionary = new();
            foreach (var item in cardPD)
            {
                cardPDictionary.Add(item.Key, Convert.ToInt32(item.Value));
            }

            string CardKind = cardData["CardKind"];
            JObject CardKindD = JsonConvert.DeserializeObject<JObject>(CardKind);

            string kind = CardKindD["leftKind"].ToString();

            string cardEP = cardData["CardEP"];
            JObject cardEPD = JsonConvert.DeserializeObject<JObject>(cardEP);

            JToken leftSkill = cardEPD["leftSkill"];
            string leftSkillName = leftSkill["name"].ToString();
            string leftSkillValue = leftSkill["value"].ToString();

            JToken rightSkill = cardEPD["rightSkill"];
            string rightSkillName = rightSkill["name"].ToString();
            string rightSkillValue = rightSkill["value"].ToString();

            Dictionary<string, Dictionary<string, object>> cardEPDictionary = new() { { "leftSkill", new() { { "name", leftSkillName }, { "name", rightSkillName } } }, { "rightSkill", new() { { "name", rightSkillName }, { "name", rightSkillValue } } } };

            Dictionary<string, object> cardDataInBattle = new()
            {
                { "CardID", cardData["CardID"] },
                { "CardName", cardData["CardName"] },
                { "CardType", cardData["CardType"] },
                { "CardKind", kind },
                { "CardRace", cardData["CardRace"] },
                { "CardHP", Convert.ToInt32(cardData["CardHP"]) },
                { "CardSkinID", cardData["CardSkinID"] },
                { "CardCost", Convert.ToInt32(cardData["CardCost"]) },
                { "CardP", cardPDictionary },
                { "CardEP", cardEPDictionary }
            };

            parameterNode.parameter.Add("CardDataInBattle", cardDataInBattle);

            SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.DragHandCard, parameterNode.parameter));

            StartCoroutine(playerAction.DoAction(playerAction.UseACard, parameterNode.parameter));
        }
    }
}