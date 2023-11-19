using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ¿¨×éÇÐ»»°´Å¥
/// </summary>
public class DeckToItemButton : MonoBehaviour
{
    public RawImage DeckToMonsterButtonImage;
    public RawImage DeckToItemButtonImage;

    public void OnClick()
    {
        DeckInCollection loadDeck = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        loadDeck.monsterOrItemInDeck = false;
        loadDeck.ChangeDeckCardShow();

        DeckToMonsterButtonImage.texture = LoadAssetBundle.uiAssetBundle.LoadAsset<Texture>("DeckNotInMonster");
        DeckToItemButtonImage.texture = LoadAssetBundle.uiAssetBundle.LoadAsset<Texture>("DeckInItem");

    }
}
