using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// ս���Ϲ���Ӣ�ۼ���ʵ���ϵķ�������Ч������
/// </summary>
public class HeroSkillInBattle : GameObjectInBattle
{
    /// <summary>
    /// ��Ӽ���
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
