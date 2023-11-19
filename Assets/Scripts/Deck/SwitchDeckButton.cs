using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 收藏界面切换卡组按钮
/// </summary>
public class SwitchDeckButton : MonoBehaviour
{
    public void OnClick()
    {
        GameObject switchDeckPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("SwitchDeckPrefab");
        GameObject CollectionAndDeckCanvas = GameObject.Find("CollectionAndDeckCanvas");
        GameObject switchDeckPrefabInstantiation = Instantiate(switchDeckPrefab, CollectionAndDeckCanvas.transform);
        switchDeckPrefabInstantiation.name = "SwitchDeckPrefabInstantiation";
        switchDeckPrefabInstantiation.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

        GameObject switchDeckCanvas = switchDeckPrefabInstantiation.transform.GetChild(0).gameObject;
        switchDeckCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        switchDeckCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        switchDeckCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
}
