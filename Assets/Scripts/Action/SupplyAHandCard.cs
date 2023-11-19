//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 补一张手牌
///// handPanelNumber 手牌位置的序号，12怪兽，34道具
///// player 哪个玩家
///// </summary>
//public class SupplyAHandCard: MonoBehaviour
//{
//    public IEnumerator ExecuteAction(Dictionary<string, object> parameter)
//    {
//        BattleProcess battleProcess = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
//        Debug.Log("SupplyAHandCard.ExecuteAction:补一张手牌前");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("BeforeSupplyAHandCard", new Dictionary<string, object>()));

//        int handPanelNumber = (int)parameter["handPanelNumber"];
//        Player player = (Player)parameter["player"];

//        foreach (PlayerData playerMessage in battleProcess.systemPlayerData)
//        {
//            if (playerMessage.player == player)
//            {
//                switch (handPanelNumber)
//                {
//                    case 0:
//                        if (playerMessage.monsterDeck.Count > 0)
//                        {
//                            playerMessage.handMonster[0] = playerMessage.monsterDeck[0];
//                            playerMessage.monsterDeck.RemoveAt(0);

//                            playerMessage.surplusMonsterAmountText.text = playerMessage.monsterDeck.Count.ToString();
//                            if(playerMessage.handMonsterPanel != null) LoadACardToHand(playerMessage.handMonster[0], playerMessage.handMonsterPanel[0]);
//                        }
//                        break;
//                    case 1:
//                        if (playerMessage.monsterDeck.Count > 0)
//                        {
//                            playerMessage.handMonster[1] = playerMessage.monsterDeck[0];
//                            playerMessage.monsterDeck.RemoveAt(0);
//                            playerMessage.surplusMonsterAmountText.text = playerMessage.monsterDeck.Count.ToString();
//                            if (playerMessage.handMonsterPanel != null) LoadACardToHand(playerMessage.handMonster[1], playerMessage.handMonsterPanel[1]);
//                        }
//                        break;
//                    case 2:
//                        if (playerMessage.itemDeck.Count > 0)
//                        {
//                            playerMessage.handItem[0] = playerMessage.itemDeck[0];
//                            playerMessage.itemDeck.RemoveAt(0);
//                            playerMessage.surplusItemAmountText.text = playerMessage.itemDeck.Count.ToString();
//                            if (playerMessage.handItemPanel != null) LoadACardToHand(playerMessage.handItem[0], playerMessage.handItemPanel[0]);
//                        }
//                        break;
//                    case 3:
//                        if (playerMessage.itemDeck.Count > 0)
//                        {
//                            playerMessage.handItem[1] = playerMessage.itemDeck[0];
//                            playerMessage.itemDeck.RemoveAt(0);
//                            playerMessage.surplusItemAmountText.text = playerMessage.itemDeck.Count.ToString();
//                            if (playerMessage.handItemPanel != null) LoadACardToHand(playerMessage.handItem[1], playerMessage.handItemPanel[1]);
//                        }
//                        break;
//                }
//                break;
//            }
//        }

//        yield return null;

//        Debug.Log("SupplyAHandCard.ExecuteAction:补一张手牌后");
//        yield return StartCoroutine(battleProcess.ExecuteEvent("AfterSupplyAHandCard", new Dictionary<string, object>()));
//    }

//    /// <summary>
//    /// 加载手牌图像
//    /// </summary>
//    /// <param name="card">卡牌数据</param>
//    /// <param name="panel">所在的位置</param>
//    public void LoadACardToHand(Dictionary<string, string> card, GameObject panel)
//    {
//        GameObject cardForShowPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardForShowPrefab");
//        GameObject aCard = Instantiate(cardForShowPrefab, panel.transform);
//        aCard.name = "CardForShow";
//        aCard.GetComponent<Transform>().localPosition = new Vector3(110, -153, 0);
//        aCard.GetComponent<CardForShow>().SetAllAttribute(card);
//        aCard.AddComponent<DragHandCard>();
//        GameObject cardCanvas = aCard.transform.GetChild(0).gameObject;
//        cardCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(220.5f, 308);
//        cardCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
//        cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);

//    }
//}
