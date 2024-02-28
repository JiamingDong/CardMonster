using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 战斗阵型（衍生）
/// 战斗阵型状态
/// </summary>
public class FormationDerive : SkillInBattle
{
    public override int AddValue(string source, int value)
    {
        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] = sourceAndValue[source] > value ? sourceAndValue[source] : value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    public override int GetSkillValue()
    {
        return sourceAndValue.Count == 0 ? -1 : sourceAndValue.Values.Max();
    }

    /// <summary>
    /// 获得此技能时，若来源为此技能的近战数值小于此技能值，获得等于差值的近战
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int skillValue = (int)parameter["SkillValue"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        if (gameObject.TryGetComponent(out Melee melee))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = melee.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.FormationDerive.Effect1"))
            {
                currentValue = keyValuePairs["Skill.FormationDerive.Effect1"];
            }

            if (currentValue < GetSkillValue())
            {
                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "Effect1");
                parameter1.Add("SkillName", "melee");
                parameter1.Add("SkillValue", GetSkillValue() - currentValue);
                parameter1.Add("Source", "Skill.FormationDerive.Effect1");

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
            }
        }

        if (gameObject.TryGetComponent(out Ranged ranged))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = ranged.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.FormationDerive.Effect1"))
            {
                currentValue = keyValuePairs["Skill.FormationDerive.Effect1"];
            }

            if (currentValue < GetSkillValue())
            {
                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "Effect1");
                parameter1.Add("SkillName", "ranged");
                parameter1.Add("SkillValue", GetSkillValue() - currentValue);
                parameter1.Add("Source", "Skill.FormationDerive.Effect1");

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
            }
        }
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是此技能、来源为此技能的近战数值小于此技能值
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        if (monsterInBattle.gameObject != gameObject || skillName != "formation_derive")
        {
            return false;
        }

        if (gameObject.TryGetComponent(out Melee melee))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = melee.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.FormationDerive.Effect1"))
            {
                currentValue = keyValuePairs["Skill.FormationDerive.Effect1"];
            }

            if (currentValue < GetSkillValue())
            {
                return true;
            }
        }

        if (gameObject.TryGetComponent(out Ranged ranged))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = ranged.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.FormationDerive.Effect1"))
            {
                currentValue = keyValuePairs["Skill.FormationDerive.Effect1"];
            }

            if (currentValue < GetSkillValue())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 原来没有魔法，刚获得了魔法，则获得等于技能数值的魔法
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        if (skillName == "melee")
        {
            Melee melee = gameObject.GetComponent<Melee>();
            Dictionary<string, int> sourceAndValue = melee.sourceAndValue;

            if (sourceAndValue.Count == 1)
            {
                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "Effect2");
                parameter1.Add("SkillName", "melee");
                parameter1.Add("SkillValue", GetSkillValue());
                parameter1.Add("Source", "Skill.FormationDerive.Effect1");

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
            }
        }

        if (skillName == "ranged")
        {
            Ranged ranged = gameObject.GetComponent<Ranged>();
            Dictionary<string, int> sourceAndValue = ranged.sourceAndValue;

            if (sourceAndValue.Count == 1)
            {
                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "Effect2");
                parameter1.Add("SkillName", "ranged");
                parameter1.Add("SkillValue", GetSkillValue());
                parameter1.Add("Source", "Skill.FormationDerive.Effect1");

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
            }
        }
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是魔法、是不是刚获得魔法
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        if (monsterInBattle.gameObject != gameObject)
        {
            return false;
        }

        if (skillName != "melee" && skillName != "ranged")
        {
            return false;
        }

        if (!parameterNode.Parent.EffectChild.result.ContainsKey("AddNewSkill"))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 删除魔法的来源后，若只剩下buff类来源，则删除来自此技能的魔法
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.DeleteSkillSource$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        if (skillName == "melee")
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect3");
            parameter1.Add("SkillName", "melee");
            parameter1.Add("Source", "Skill.FormationDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
        }

        if (skillName == "ranged")
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect3");
            parameter1.Add("SkillName", "ranged");
            parameter1.Add("Source", "Skill.FormationDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
        }
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是魔法、来源是不是非buff、剩下的来源是不是都是buff
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];

        if (monsterInBattle.gameObject != gameObject)
        {
            return false;
        }

        if (skillName != "melee" && skillName != "ranged")
        {
            return false;
        }

        List<Dictionary<string, string>> list = Database.cardMonster.Query("SkillSourceBuff", "and source='" + source + "'");

        if (list.Count > 0)
        {
            return false;
        }

        if (skillName == "melee" && gameObject.TryGetComponent(out Melee melee))
        {
            Dictionary<string, int> sourceAndValue = melee.sourceAndValue;

            foreach (var item in sourceAndValue)
            {
                List<Dictionary<string, string>> list1 = Database.cardMonster.Query("SkillSourceBuff", "and source='" + item.Key + "'");

                if (list1.Count == 0)
                {
                    return false;
                }
            }
        }

        if (skillName == "ranged" && gameObject.TryGetComponent(out Ranged ranged))
        {
            Dictionary<string, int> sourceAndValue = ranged.sourceAndValue;

            foreach (var item in sourceAndValue)
            {
                List<Dictionary<string, string>> list1 = Database.cardMonster.Query("SkillSourceBuff", "and source='" + item.Key + "'");

                if (list1.Count == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 删除此技能前，删除来自此技能的魔法
    /// </summary>
    [TriggerEffect(@"^Before\.MonsterInBattle\.DeleteSkillSource$", "Compare4")]
    public IEnumerator Effect4(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string source = (string)parameter["Source"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, int> keyValuePairs = new(sourceAndValue);
        keyValuePairs.Remove(source);
        int newSkillValue = keyValuePairs.Count == 0 ? -1 : keyValuePairs.Values.Max();

        //移除此来源后，没有来源，则删除对基础攻击的增益，还有来源就变动差值
        if (newSkillValue == -1)
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect4");
            parameter1.Add("SkillName", "melee");
            parameter1.Add("Source", "Skill.FormationDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", this);
            parameter2.Add("EffectName", "Effect4");
            parameter2.Add("SkillName", "ranged");
            parameter2.Add("Source", "Skill.FormationDerive.Effect1");

            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
            parameterNode2.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
        }
        else
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect4");
            parameter1.Add("SkillName", "melee");
            parameter1.Add("SkillValue", newSkillValue - GetSkillValue());
            parameter1.Add("Source", "Skill.FormationDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", this);
            parameter2.Add("EffectName", "Effect4");
            parameter2.Add("SkillName", "ranged");
            parameter2.Add("SkillValue", newSkillValue - GetSkillValue());
            parameter2.Add("Source", "Skill.FormationDerive.Effect1");

            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
            parameterNode2.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
        }
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是此技能
    /// </summary>
    public bool Compare4(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];

        Dictionary<string, int> keyValuePairs = new(sourceAndValue);
        keyValuePairs.Remove(source);
        int newSkillValue = keyValuePairs.Count == 0 ? -1 : keyValuePairs.Values.Max();

        return monsterInBattle.gameObject == gameObject && skillName == "formation_derive" && newSkillValue != GetSkillValue();
    }

    /// <summary>
    /// 免疫恐惧
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^Replace\.GameAction\.SwapMonsterPosition$", "Compare5")]
    public IEnumerator Effect5(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
    }

    /// <summary>
    /// 判断是否是本怪兽，技能是恐惧
    /// </summary>
    public bool Compare5(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        GameObject monsterMove1 = (GameObject)parameter["MonsterMove1"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        return monsterMove1 == gameObject && skillInBattle is Swap;
    }

    /// <summary>
    /// 对方回合结束时，清除此技能
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^AfterRoundBattle$", "Compare6")]
    public IEnumerator Effect6(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect6");
        parameter2.Add("SkillName", "formation_derive");
        parameter2.Add("Source", "Skill.Formation.Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
    }

    /// <summary>
    /// 判断是否是对方回合
    /// </summary>
    public bool Compare6(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Enemy)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}