using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ≤‚ ‘
/// </summary>
public class BattleTest : MonoBehaviour
{
    public void OnClick()
    {
        GameObject gameDefeatParfab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("GameDefeatParfab");
        GameObject battleSceneCanvas = GameObject.Find("BattleSceneCanvas");
        GameObject gameDefeatParfabInstantiation = Instantiate(gameDefeatParfab, battleSceneCanvas.transform);
        gameDefeatParfabInstantiation.name = "GameDefeatParfabInstantiation";
        gameDefeatParfabInstantiation.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

        GameObject selectHeroSkillCanvas = gameDefeatParfabInstantiation.transform.GetChild(0).gameObject;
        selectHeroSkillCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        selectHeroSkillCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        selectHeroSkillCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
}
