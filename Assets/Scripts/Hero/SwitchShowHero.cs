using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ӣ�۴�ͼ���л�չʾ�ļ���
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
