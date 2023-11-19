using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 收藏转到道具
/// </summary>
public class CollectionToItemButton : MonoBehaviour
{
    public void OnClick()
    {
        List<Dictionary<string, string>> cardList = Database.cardMonster.Query("AllCardConfig", "and CardType<>'monster' and CardFlags not like '%\"1\"%' order by CardType asc,CardKind asc,CardCost asc");
        CollectionCard loadCollectionCard = GameObject.Find("CardCollectionBackgroundPanel").GetComponent<CollectionCard>();
        loadCollectionCard.ChangeCardData(cardList);
    }
}
