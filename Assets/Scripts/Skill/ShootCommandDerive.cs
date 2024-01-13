using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 设计指挥（衍生）
/// 远程不会被“护甲”闪避。对方回合结束时，清除【射击指挥】效果
/// </summary>
public class ShootCommandDerive : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Armor\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
    }

    /// <summary>
    /// 判断是否是本怪兽，技能是远程
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        return skillInBattle is Ranged && skillInBattle.gameObject == gameObject;
    }

    [TriggerEffect("^AfterRoundBattle$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect2");
        parameter1.Add("SkillName", "ranged");
        parameter1.Add("Source", "Skill.ShootCommand.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect2");
        parameter2.Add("SkillName", "shoot_command_derive");
        parameter2.Add("Source", "Skill.ShootCommand.Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
    }

    /// <summary>
    /// 判断是否是对方回合
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        //Debug.Log("设计指挥（衍生）");

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