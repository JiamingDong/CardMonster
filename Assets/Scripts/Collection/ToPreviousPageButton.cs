using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 收藏到上一页
/// </summary>
public class ToPreviousPageButton : MonoBehaviour
{
    public void OnClick()
    {
        CollectionCard loadCollectionCard = GameObject.Find("CardCollectionBackgroundPanel").GetComponent<CollectionCard>();
        loadCollectionCard.ChangePageNumber(-1);
    }
}
