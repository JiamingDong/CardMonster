using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌信息界面，点击卡牌，切换信息
/// </summary>
public class CardInDetailInterfaceClick : MonoBehaviour
{
    public Dictionary<string, string> cardData;

    public void OnClick()
    {
        CardDetailInterface initCardDetailInterface = GameObject.Find("CardDetailInterfacePrefab").GetComponent<CardDetailInterface>();
        initCardDetailInterface.SwitchInfo(cardData);
    }
}
