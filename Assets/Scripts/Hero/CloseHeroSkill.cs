using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关闭选英雄技能的界面
/// </summary>
public class CloseHeroSkill : MonoBehaviour
{
    public void OnClick()
    {
        GameObject selectHeroSkillPrefabInstantiation = GameObject.Find("SelectHeroSkillPrefabInstantiation");
        Destroy(selectHeroSkillPrefabInstantiation);
    }
}
