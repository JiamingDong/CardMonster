using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenCardDetailInterface : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    public Dictionary<string, string> cardData;

    float unscaledTime;
    Vector3 mousePosition;

    void OpenInterface()
    {
        GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardDetailInterfacePrefab");
        GameObject instance = Instantiate(prefab);
        instance.name = "CardDetailInterfacePrefab";
        instance.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        instance.GetComponent<CardDetailInterface>().Init(new() { cardData });

        GameObject canvas = instance.transform.Find("Canvas").gameObject;
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        canvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OpenInterface();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        unscaledTime = Time.unscaledTime;
        mousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 mousePosition2 = Input.mousePosition;
        float l = (mousePosition2.x - mousePosition.x) * (mousePosition2.x - mousePosition.x) + (mousePosition2.y - mousePosition.y) * (mousePosition2.y - mousePosition.y);
        Debug.Log(l);
        if (Time.unscaledTime - unscaledTime > 0.5 && l < 2500)
        {
            OpenInterface();
        }
    }
}
