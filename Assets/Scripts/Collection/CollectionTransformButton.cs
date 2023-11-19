using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ղ�չʾת�����޺͵��߰�ť
/// </summary>
public class CollectionTransformButton : MonoBehaviour
{
    public RawImage CollectionTransformButtonImage;
    public bool monsterOrItemInCollection = true;//���޻��ǵ��ߣ�true���ޣ�false����

    private void Start()
    {
        CollectionTransformButtonImage.texture = LoadAssetBundle.uiAssetBundle.LoadAsset<Texture>("CollectionTransformButton");
    }

    public void Onclick()
    {
        CollectionCard loadCollectionCard = GameObject.Find("CardCollectionBackgroundPanel").GetComponent<CollectionCard>();
        if (monsterOrItemInCollection)loadCollectionCard.cardData = Database.cardMonster.Query("CardConfig", "and type!='0' and (kindmark not like '%|2|%' or kindmark is null) order by type asc,kindmark desc,color asc,cost asc");
        else loadCollectionCard.cardData = Database.cardMonster.Query("CardConfig", "and type='0' and (kindmark not like '%|2|%' or kindmark is null) order by type asc,kindmark desc,color asc,cost asc");
        loadCollectionCard.ChangeCollectionCardShow();
        monsterOrItemInCollection = !monsterOrItemInCollection;
    }
}
