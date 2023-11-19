using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ����ս�������ڽ����е�����Ʒ
/// </summary>
public class ConsumeInBattle : GameObjectInBattle
{
    public string id;
    public string cardName;
    public string skinID;
    public string kind;
    public string type;
    public int cost;
    public string race;

    /// <summary>
    /// ���ɵ�ʱ�����
    /// </summary>
    public void GenerateConsume(Dictionary<string, string> cardData)
    {
        id = cardData["CardID"];
        cardName = cardData["CardName"];
        type = cardData["CardType"];
        kind = cardData["CardKind"];
        race = cardData["CardRace"];
        skinID = cardData["CardSkinID"];
        cost = Convert.ToInt32(cardData["CardName"]);
        string pN = cardData["CardPN"];
        string pV = cardData["CardPV"];

        string[] pNArray = pN.Substring(1, pN.Length - 2).Split('|');
        string[] pVArray = pV.Substring(1, pV.Length - 2).Split('|');
        for (int i = 0; i < pNArray.Length; i++)
        {
            Dictionary<string, string> skillDetile = Database.cardMonster.Query("AllSkillConfig", "and SkillID='" + pNArray[i] + "'")[0];
            AddSkill(skillDetile["SkillClassNmae"], Convert.ToInt32(pNArray[i]), "monster");
        }

    }

    /// <summary>
    /// ��Ӽ���
    /// </summary>
    /// <param name="skillName">������</param>
    /// <param name="skillValue">��ֵ</param>
    public void AddSkill(string skillName, int skillValue, string source)
    {
        foreach (SkillInBattle skillInCard in skillList)
        {
            if (skillInCard.GetType().Name.Equals(skillName))
            {
                skillInCard.AddValue("Consume", skillValue);
                return;
            }
        }

        //��ȡ��ǰ����
        Assembly assembly = Assembly.GetExecutingAssembly();
        //�������ʵ��������Ϊ object ���ͣ���Ҫǿ������ת��
        SkillInBattle skill = (SkillInBattle)assembly.CreateInstance(skillName);
        skill.AddValue(source, skillValue);
        skillList.Add(skill);
    }

}
