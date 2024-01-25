using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 治疗护盾（衍生）
/// 对方回合结束时，清除【治疗护盾】效果
/// </summary>
public class HealShieldDerive : SkillInBattle
{
    [TriggerEffect("^AfterRoundBattle$", "Compare1")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillName", "shield");
        parameter1.Add("Source", "Skill.HealShield.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect1");
        parameter2.Add("SkillName", "magic_shield");
        parameter2.Add("Source", "Skill.HealShield.Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));

        Dictionary<string, object> parameter3 = new();
        parameter3.Add("LaunchedSkill", this);
        parameter3.Add("EffectName", "Effect1");
        parameter3.Add("SkillName", "power_shield");
        parameter3.Add("Source", "Skill.HealShield.Effect1");

        ParameterNode parameterNode3 = parameterNode.AddNodeInMethod();
        parameterNode3.parameter = parameter3;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode3));

        Dictionary<string, object> parameter4 = new();
        parameter4.Add("LaunchedSkill", this);
        parameter4.Add("EffectName", "Effect1");
        parameter4.Add("SkillName", "heal_shield_derive");
        parameter4.Add("Source", "Skill.HealShield.Effect1");

        ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
        parameterNode4.parameter = parameter4;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode4));
        //yield return null;
    }

    /// <summary>
    /// 判断是否是对方回合
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
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