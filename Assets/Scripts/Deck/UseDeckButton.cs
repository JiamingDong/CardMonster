using UnityEngine;

/// <summary>
/// �����ʹ�á���ť���л�����
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
