using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 坚毅
/// 免疫“疾病”，“束缚”，“沉默”，“恫吓”和“法术抑制”状态。
/// 使用后，清除“疾病”、“被束缚”、“被沉默”、“恫吓（衍生）”和“法术抑制（衍生）”，清除“魔法”里的“法术抑制”来源，清除“远程”和“近战”里的“恫吓”来源
/// 防止获得“疾病”、“被束缚”、“被沉默”、“恫吓（衍生）”和“法术抑制（衍生）”，防止来源为“法术抑制”的“魔法”，防止来源为“恫吓”的“远程”和“近战”
/// </summary>
public class Determination : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$",  "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
        List<SkillInBattle> skillList = monsterInBattle.skillList;

        foreach (SkillInBattle skillInBattle in skillList)
        {
            if (skillInBattle is DiseaseDerive || skillInBattle is EntangleDerive || skillInBattle is SilenceDerive || skillInBattle is DemoralizeDerive || skillInBattle is AntimagicDerive)
            {
                var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillClassName='" + skillInBattle.GetType().Name + "'")[0];
                var skillEnglishName = skillConfig["SkillEnglishName"];

                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "Effect1");
                parameter1.Add("SkillName", skillEnglishName);
                parameter1.Add("SkillValue", -skillInBattle.GetSkillValue());
                parameter1.Add("Source", "Skill.Determination.Effect1");

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                yield return null;
            }
        }

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect1");
        parameter2.Add("SkillName", "magic");
        parameter2.Add("Source", "Skill.Antimagic");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
        yield return null;

        Dictionary<string, object> parameter3 = new();
        parameter3.Add("LaunchedSkill", this);
        parameter3.Add("EffectName", "Effect1");
        parameter3.Add("SkillName", "melee");
        parameter3.Add("Source", "Skill.Demoralize");

        ParameterNode parameterNode3 = parameterNode.AddNodeInMethod();
        parameterNode3.parameter = parameter3;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode3));
        yield return null;

        Dictionary<string, object> parameter4 = new();
        parameter4.Add("LaunchedSkill", this);
        parameter4.Add("EffectName", "Effect1");
        parameter4.Add("SkillName", "ranged");
        parameter4.Add("Source", "Skill.Demoralize");

        ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
        parameterNode4.parameter = parameter4;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode4));
        yield return null;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、有没有“疾病”、“被束缚”、“被沉默”、“恫吓（衍生）”和“法术抑制（衍生）”
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

        GameObject go = null;
        if (result.ContainsKey("MonsterBeGenerated"))
        {
            go = (GameObject)result["MonsterBeGenerated"];
        }
        if (result.ContainsKey("MonsterBeEquipped"))
        {
            go = (GameObject)result["MonsterBeEquipped"];
        }

        if (go == null || go != gameObject)
        {
            return false;
        }

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
        List<SkillInBattle> skillList = monsterInBattle.skillList;

        foreach (SkillInBattle skillInBattle in skillList)
        {
            if (skillInBattle is DiseaseDerive || skillInBattle is EntangleDerive || skillInBattle is SilenceDerive || skillInBattle is DemoralizeDerive || skillInBattle is AntimagicDerive)
            {
                return true;
            }
        }

        return false;
    }

    [TriggerEffect(@"^Replace\.MonsterInBattle\.AddSkill$",  "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// 判断技能是不是“疾病”、“被束缚”、“被沉默”、“恫吓（衍生）”、“法术抑制（衍生）”、来源为“法术抑制”的“魔法”或来源为“恫吓”的“远程”和“近战”
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (monsterInBattle.gameObject == gameObject)
        {
            if (skillName.Equals("disease_derive") || skillName.Equals("entangle_derive") || skillName.Equals("silence_derive") || skillName.Equals("demoralize_derive") || skillName.Equals("antimagic_derive"))
            {
                return true;
            }

            if (skillName.Equals("magic") && source.Equals("Skill.Antimagic"))
            {
                return true;
            }

            if ((skillName.Equals("melee") || skillName.Equals("ranged")) && source.Equals("Skill.Demoralize"))
            {
                return true;
            }
        }

        return false;
    }
}
