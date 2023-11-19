using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToCollectionAndDeck : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("CollectionAndDeckScene");//切换场景到CardDeckScene
    }
}
