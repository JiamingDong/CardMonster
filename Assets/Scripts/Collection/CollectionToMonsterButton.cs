using UnityEngine;

/// <summary>
/// �ղ�ת������
/// </summary>
public class CollectionToMonsterButton : MonoBehaviour
{
    public void OnClick()
    {
        CollectionCard loadCollectionCard = GameObject.Find("CardCollectionBackgroundPanel").GetComponent<CollectionCard>();
        loadCollectionCard.cardType = "monster";
        loadCollectionCard.ChangeCardData();
    }
}
