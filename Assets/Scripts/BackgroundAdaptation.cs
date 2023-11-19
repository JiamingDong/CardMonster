using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAdaptation : MonoBehaviour
{
    private void Start()
    {
        Resize();
    }

    void Resize()
    {
        float width = GetComponent<SpriteRenderer>().bounds.size.x;
        Vector2 scale = transform.localScale;
        scale.x = (Camera.main.orthographicSize * 2 / Screen.height * Screen.width) / width;//屏幕宽/屏幕高=相机宽/相机高,相机宽=图片宽*图片在x轴的缩放系数
        transform.localScale = scale;
    }
    
}
