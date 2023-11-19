using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ÊÕ²Ø×ªµ½¹ÖÊÞ
/// </summary>
public class CollectionToMonsterButton : MonoBehaviour
{
    public void OnClick()
    {
        List<Dictionary<string, string>> cardList = Database.cardMonster.Query("AllCardConfig", "and CardType='monster' and CardFlags not like '%\"1\"%' order by CardKind asc,CardCost asc");
        CollectionCard loadCollectionCard = GameObject.Find("CardCollectionBackgroundPanel").GetComponent<CollectionCard>();
        loadCollectionCard.ChangeCardData(cardList);
    }
}
