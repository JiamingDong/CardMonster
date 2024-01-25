using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战场上挂在英雄技能实体上的发动技能效果的类
/// </summary>
public class HeroSkillInBattle : GameObjectInBattle
{
    /// <summary>
    /// 添加技能
    /// </summary>
    /// <param name="id"></param>
    public void AddSkill(string id)
    {
        HeroSkill heroSkill = gameObject.GetComponent<HeroSkill>();
        heroSkill.ChangeHeroSkill(id);

        Dictionary<string, string> skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillID='" + id + "'")[0];
        string skillClassName = skillConfig["SkillClassName"];
        string skillFlags = skillConfig["SkillFlags"];

        Type type = Type.GetType(skillClassName);

        SkillInBattle skill = (SkillInBattle)gameObject.AddComponent(type);

        skillList.Add(skill);

        if (skillFlags.Contains("|3|"))
        {
            string skillEnglishName = skillConfig["SkillEnglishName"];

            Dictionary<string, string> heroSkillConfig = Database.cardMonster.Query("HeroSkillConfig", "and skillname='" + skillEnglishName + "'")[0];
            string startClassName = heroSkillConfig["startclass"];

            GameObject go = gameObject.transform.Find("HeroSkillCanvas").gameObject;

            Type type2 = Type.GetType(startClassName);
            HeroSkillStart heroSkillStart = (HeroSkillStart)go.AddComponent(type2);

            go.AddComponent<Button>().onClick.AddListener(heroSkillStart.OnClick);
        }
    }
}
