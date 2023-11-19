using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public Image buttonBackgroundImage;
    public Image buttonImage;
    public Text buttonText;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonTextSize">×ÖÌå´óÐ¡</param>
    public void Initialize(Vector2 buttonBackgroundImageSizeDelta,Sprite buttonBackgroundImageSprite, bool buttonImageEnabled, Sprite buttonImageSprite, Vector2 buttonImageSizeDelta, bool buttonTextEnabled, string buttonTextText, int buttonTextSize)
    {
        buttonBackgroundImage.GetComponent<RectTransform>().sizeDelta = buttonBackgroundImageSizeDelta;
        buttonBackgroundImage.GetComponent<Image>().sprite = buttonBackgroundImageSprite;
        buttonImage.enabled = buttonImageEnabled;
        buttonImage.GetComponent<RectTransform>().sizeDelta = buttonImageSizeDelta;
        buttonImage.GetComponent<Image>().sprite = buttonImageSprite;
        buttonText.enabled = buttonTextEnabled;
        buttonText.text = buttonTextText;
        buttonText.fontSize = buttonTextSize;
    }
}
