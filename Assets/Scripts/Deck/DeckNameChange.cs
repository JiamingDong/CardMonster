using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckNameChange : MonoBehaviour
{
    /// <summary>
    /// �ı���ֵ�ı�
    /// </summary>
    public void OnChange()
    {
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        deckInCollection.deckName = transform.GetComponent<InputField>().text;
    }
}
