public class SkillUtils
{
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
        string name = skillInBattle.GetType().Name;
        if (name.Equals("Chance") || name.Equals("Magic") || name.Equals("Ranged") || name.Equals("Melee"))
        {
            return true;
        }
        return false;
    }
}
