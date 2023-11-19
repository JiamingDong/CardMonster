using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 收藏界面卡组的英雄技能、战斗界面的英雄技能
/// </summary>
public class HeroSkill : MonoBehaviour
{
    public string heroSkillId;
    public SkillInBattle skillInBattle;

    public Text heroSkillNameText;

    /// <summary>
    /// 加载或改变英雄技能图像和文字
    /// </summary>
    /// <param name="id">英雄技能id</param>
    public void ChangeHeroSkill(string id)
    {
        heroSkillId = id;
        if (id.Equals(""))
        {
            heroSkillNameText.text = "";
        }
        else
        {
            Dictionary<string, string> heroSkillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillID='" + heroSkillId + "'")[0];
            heroSkillNameText.text = heroSkillConfig["SkillChineseName"];
        }

    }
}
