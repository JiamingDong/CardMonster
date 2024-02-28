using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 治疗护盾（衍生）
/// 对方回合结束时，清除【治疗护盾】效果
/// </summary>
public class HealShieldDerive : SkillInBattle
{
    int skillValue = 0;
    string shieldKind = "";

    public override int AddValue(string source, int value)
    {
        if (sourceAndValue.Count == 0)
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    /// <summary>
    /// 第一次获得此技能时，获得护盾
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        int r = RandomUtils.GetRandomNumber(1, 3);
        switch (r)
        {
            case 1:
                shieldKind = "shield";
                break;
            case 2:
                shieldKind = "magic_shield";
                break;
            case 3:
                shieldKind = "power_shield";
                break;
        }

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillValue", GetSkillValue());
        parameter1.Add("Source", "Skill.HealShieldDerive.Effect1");
        parameter1.Add("SkillName", shieldKind);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

        skillValue = GetSkillValue();
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是此技能、是否已经获取过这个技能
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        if (monsterInBattle.gameObject != gameObject || skillName != "heal_shield_derive")
        {
            return false;
        }

        if (skillValue > 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 删除此技能前，删除来自此技能的护盾
    /// </summary>
    [TriggerEffect(@"^Before\.MonsterInBattle\.DeleteSkillSource$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect2");
        parameter1.Add("SkillName", shieldKind);
        parameter1.Add("Source", "Skill.HealShieldDerive.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是此技能
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];

        Dictionary<string, int> keyValuePairs = new(sourceAndValue);
        keyValuePairs.Remove(source);

        return monsterInBattle.gameObject == gameObject && skillName == "heal_shield_derive" && keyValuePairs.Count == 0;
    }

    [TriggerEffect("^AfterRoundBattle$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter4 = new();
        parameter4.Add("LaunchedSkill", this);
        parameter4.Add("EffectName", "Effect3");
        parameter4.Add("SkillName", "heal_shield_derive");
        parameter4.Add("Source", "Skill.HealShield.Effect1");

        ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
        parameterNode4.parameter = parameter4;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode4));
    }

    /// <summary>
    /// 判断是否是对方回合
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Enemy)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}