using System;
using UnityEngine;
using UnityEngine.UI;

public class InitArrowPrefab : MonoBehaviour
{
    public Image image;

    public void Init(Vector3 startPosition, Vector3 endPosition)
    {
        image.rectTransform.position = startPosition;

        image.rectTransform.sizeDelta = new(Vector3.Distance(startPosition, endPosition), image.rectTransform.sizeDelta.y);

        double angle = Math.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x);
        double degrees = angle * 180 / Math.PI;
        image.rectTransform.rotation = Quaternion.Euler(0, 0, (float)degrees);
    }
}
