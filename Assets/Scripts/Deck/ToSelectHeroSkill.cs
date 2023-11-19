using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 收藏界面打开选英雄技能的界面
/// </summary>
public class ToSelectHeroSkill : MonoBehaviour
{
    public void OnClick()
    {

        GameObject selectHeroSkillPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("SelectHeroSkillPrefab");
        GameObject CollectionAndDeckCanvas = GameObject.Find("CollectionAndDeckCanvas");
        GameObject SelectHeroSkillPrefabInstantiation = Instantiate(selectHeroSkillPrefab, CollectionAndDeckCanvas.transform);
        SelectHeroSkillPrefabInstantiation.name = "SelectHeroSkillPrefabInstantiation";
        SelectHeroSkillPrefabInstantiation.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

        GameObject selectHeroSkillCanvas = SelectHeroSkillPrefabInstantiation.transform.GetChild(0).gameObject;
        selectHeroSkillCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        selectHeroSkillCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        selectHeroSkillCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
}
