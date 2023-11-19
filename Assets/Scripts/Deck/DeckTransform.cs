using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ղؽ��濨����������л���ť
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
