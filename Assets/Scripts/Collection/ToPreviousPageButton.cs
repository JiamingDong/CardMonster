using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ղص���һҳ
/// </summary>
public class ToPreviousPageButton : MonoBehaviour
{
    public void OnClick()
    {
        CollectionCard loadCollectionCard = GameObject.Find("CardCollectionBackgroundPanel").GetComponent<CollectionCard>();
        loadCollectionCard.ChangePageNumber(-1);
    }
}
