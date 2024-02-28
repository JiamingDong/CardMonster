using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 延年寿（衍生）
/// 获得乘黄除“延年寿”外的所有技能，直到乘黄离场
/// </summary>
public class ProlongLifeDerive : SkillInBattle
{
    /// <summary>
    /// 标识每个乘黄与对应的技能
    /// </summary>
    Dictionary<GameObject, Dictionary<string, int>> prolongLifeMonster = new();

    /// <summary>
    /// 获得此技能时，获得SkillFromProlongLife里的技能
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        ProlongLife prolongLife = (ProlongLife)parameter["LaunchedSkill"];
        Dictionary<string, int> skillDic = (Dictionary<string, int>)parameter["SkillFromProlongLife"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        prolongLifeMonster.Add(prolongLife.gameObject, skillDic);

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        foreach (var item in skillDic)
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect1");
            parameter1.Add("SkillName", item.Key);
            parameter1.Add("SkillValue", item.Value);
            parameter1.Add("Source", "Skill.ProlongLifeDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是此技能
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        return monsterInBattle.gameObject == gameObject && skillName.Equals("prolong_life_derive");
    }

    /// <summary>
    /// 删除此技能前，删除来自此技能的魔法
    /// </summary>
    [TriggerEffect(@"^Before\.MonsterInBattle\.DeleteSkillSource$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        List<SkillInBattle> skillList = monsterInBattle.skillList;
        List<string> strings = new();
        foreach (var item in skillList)
        {
            Dictionary<string, int> sourceAndValue = item.sourceAndValue;
            if (sourceAndValue.ContainsKey("Skill.ProlongLifeDerive.Effect1"))
            {
                var SkillEngLishName = battleProcess.allSkillConfig.Where(x => x["SkillClassName"] == item.GetType().Name).FirstOrDefault()["SkillEnglishName"];
                strings.Add(SkillEngLishName);
            }
        }

        foreach (var item in strings)
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect2");
            parameter1.Add("SkillName", item);
            parameter1.Add("Source", "Skill.ProlongLifeDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
        }
    }

    /// <summary>
    /// 有怪兽离场时，若是、技能是不是此技能
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        return monsterInBattle.gameObject == gameObject && skillName == "prolong_life_derive";
    }

    /// <summary>
    /// prolongLifeMonster里的怪兽离场后，清除对应技能
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^After.GameAction.MonsterLeave$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        //最后一个乘黄离场，删除此技能，否则，删除该乘黄对应的技能
        if (prolongLifeMonster.Count == 1)
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect3");
            parameter1.Add("SkillName", "prolong_life_derive");
            parameter1.Add("Source", "Skill.ProlongLife.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
        }
        else
        {
            Dictionary<string, int> skillDic = prolongLifeMonster[effectTarget];

            foreach (var item in skillDic)
            {
                string skillName = item.Key;
                int skillVaue = item.Value;

                //检测其他乘黄有没有带来此技能，若有，减去对应数值，若没有，则直接删除来源
                bool hasSource = false;
                foreach (var m in prolongLifeMonster)
                {
                    if (m.Key != effectTarget)
                    {
                        Dictionary<string, int> d = m.Value;
                        if (d.ContainsKey(skillName))
                        {
                            hasSource = true;
                        }
                    }
                }

                if (hasSource)
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("LaunchedSkill", this);
                    parameter1.Add("EffectName", "Effect3");
                    parameter1.Add("SkillName", skillName);
                    parameter1.Add("SkillValue", -skillVaue);
                    parameter1.Add("Source", "Skill.ProlongLifeDerive.Effect1");

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                }
                else
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("LaunchedSkill", this);
                    parameter1.Add("EffectName", "Effect3");
                    parameter1.Add("SkillName", skillName);
                    parameter1.Add("Source", "Skill.ProlongLifeDerive.Effect1");

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
                }
            }

            prolongLifeMonster.Remove(effectTarget);
        }
    }

    /// <summary>
    /// 判断是否是对方回合
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];

        return prolongLifeMonster.ContainsKey(effectTarget);
    }
}
