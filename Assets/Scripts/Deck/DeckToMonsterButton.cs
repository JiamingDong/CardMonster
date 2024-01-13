using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ¿¨×éÇÐ»»
/// </summary>
public class DeckToMonsterButton : MonoBehaviour
{
    public RawImage DeckToMonsterButtonImage;
    public RawImage DeckToItemButtonImage;

    public void OnClick()
    {
        DeckInCollection loadDeck = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        loadDeck.monsterOrItemInDeck = true;
        loadDeck.ChangeDeckCardShow();

        DeckToMonsterButtonImage.texture = LoadAssetBundle.uiAssetBundle.LoadAsset<Texture>("DeckInMonster");
        DeckToItemButtonImage.texture = LoadAssetBundle.uiAssetBundle.LoadAsset<Texture>("DeckNotInItem");
    }
}
