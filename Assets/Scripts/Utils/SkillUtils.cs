using System.Collections.Generic;

public class SkillUtils
{
    public static HashSet<string> basicAttackEffectSet = new() { "magic", "melee", "ranged", "chance" };

    /// <summary>
    /// ��ü�������
    /// </summary>
    /// <param name="skillName">������</param>
    /// <returns></returns>
    public static SkillType GetSkillType(string skillName)
    {
        return skillName switch
        {
            "armor" => SkillType.Armor,
            "chance" or "magic" or "ranged" or "melee" or "heal" or "heal_all" or "heal_consume" or "heal_all_consume" or "lightning" or "lightning_all" or "damage" or "damage_all" => SkillType.BasicSkill,
            _ => SkillType.NonbasicSkill,
        };
    }

    /// <summary>
    /// �Ƿ��ǻ�������Ч��
    /// </summary>
    /// <param name="effectName"></param>
    /// <returns></returns>
    public static bool IsBasicAttackEffect(SkillInBattle skillInBattle)
    {
        return skillInBattle is Chance || skillInBattle is Magic || skillInBattle is Ranged || skillInBattle is Melee;
    }
}
