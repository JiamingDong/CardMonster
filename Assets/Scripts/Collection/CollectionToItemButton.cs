using UnityEngine;

/// <summary>
/// �ղ�ת������
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
