using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点击英雄大图，切换展示的技能
/// </summary>
public class SwitchShowHero : MonoBehaviour
{
    public string heroId;

    public void OnClick()
    {
        GameObject selectHeroSkill = GameObject.Find("SelectHeroSkillPrefabInstantiation");
        selectHeroSkill.GetComponent<SelectHeroSkill>().LoadHeroSkill(heroId);
        //SelectHeroSkill.LoadHeroSkill(heroId);
    }
}
