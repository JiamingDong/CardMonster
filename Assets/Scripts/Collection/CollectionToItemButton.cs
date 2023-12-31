using UnityEngine;

/// <summary>
/// 收藏转到道具
/// </summary>
public class CollectionToItemButton : MonoBehaviour
{
    public void OnClick()
    {
        CollectionCard loadCollectionCard = GameObject.Find("CardCollectionBackgroundPanel").GetComponent<CollectionCard>();
        loadCollectionCard.cardType = "item";
        loadCollectionCard.ChangeCardData();
    }
}
