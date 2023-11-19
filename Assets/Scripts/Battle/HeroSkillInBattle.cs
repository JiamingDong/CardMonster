using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

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

        Dictionary<string, string> heroSkillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillID='" + id + "'")[0];
        string skillClassName = heroSkillConfig["SkillClassName"];

        Type type = Type.GetType(skillClassName);

        SkillInBattle skill = (SkillInBattle)gameObject.AddComponent(type);

        skillList.Add(skill);
    }
}
