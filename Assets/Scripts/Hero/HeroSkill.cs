using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ղؽ��濨���Ӣ�ۼ��ܡ�ս�������Ӣ�ۼ���
/// </summary>
public class HeroSkill : MonoBehaviour
{
    public string heroSkillId;
    public SkillInBattle skillInBattle;

    public Text heroSkillNameText;

    /// <summary>
    /// ���ػ�ı�Ӣ�ۼ���ͼ�������
    /// </summary>
    /// <param name="id">Ӣ�ۼ���id</param>
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
