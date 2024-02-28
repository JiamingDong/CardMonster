using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ϣ���棬������ƣ��л���Ϣ
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
