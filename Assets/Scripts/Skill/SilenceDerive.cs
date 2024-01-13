using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 被沉默
/// <魔法><治疗><群体治疗>失效。回合结束时，数值-1。
/// </summary>
public class SilenceDerive : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Magic\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Heal\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.HealAll\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        Dictionary<string, object> result = parameterNode.result;
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        return skillInBattle.gameObject == gameObject;
    }

    [TriggerEffect("^AfterRoundBattle$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter = new();
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect2");
        parameter.Add("SkillName", "silence_derive");
        parameter.Add("SkillValue", -1);
        parameter.Add("Source", "Skill.SilenceDerive.Effect2");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断是否是己方回合
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
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
