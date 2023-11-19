using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 收藏界面卡组左边两个切换按钮
/// </summary>
public class DeckTransform : MonoBehaviour
{
    public RawImage DeckToMonsterButtonImage;
    public RawImage DeckToItemButtonImage;

    // Start is called before the first frame update
    void Start()
    {

        DeckToMonsterButtonImage.texture = LoadAssetBundle.uiAssetBundle.LoadAsset<Texture>("DeckInMonster");
        DeckToItemButtonImage.texture = LoadAssetBundle.uiAssetBundle.LoadAsset<Texture>("DeckNotInItem");
    }
}
