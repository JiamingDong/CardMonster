using UnityEngine;

/// <summary>
/// 点击“使用”按钮，切换卡组
/// </summary>
public class UseDeckButton : MonoBehaviour
{
    public string deckId;

    public void OnClick()
    {
        GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>().SwitchDeck(deckId);

        Destroy(GameObject.Find("SwitchDeckPrefabInstantiation"));
    }
}
