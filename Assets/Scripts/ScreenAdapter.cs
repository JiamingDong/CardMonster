using UnityEngine;

public class ScreenAdapter : MonoBehaviour
{
    public int w = 16;
    public int h = 9;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        AdaptScreen();
    }

    void AdaptScreen()
    {
        //Debug.Log(Screen.width);
        //Debug.Log(Screen.height);
        //Debug.Log(rectTransform.sizeDelta.x);
        //Debug.Log(rectTransform.sizeDelta.y);

        int a = Screen.width * h;
        int b = Screen.height * w;

        if (a > b)
        {
            //Debug.Log(1);
            //Debug.Log(rectTransform.sizeDelta.y * Screen.width / Screen.height);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y * w / h, rectTransform.sizeDelta.y);
        }
        else if (a == b)
        {
            //Debug.Log(2);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        }
        else
        {
            //Debug.Log(3);
            //Debug.Log(rectTransform.sizeDelta.x * Screen.height / Screen.width);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x * h / w);
        }
    }
}