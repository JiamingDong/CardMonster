using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 恶化（衍生）
/// 造成的真实伤害+1，持续到敌方回合结束
/// </summary>
public class DegradationDerive : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        parameter["DamageValue"] = damageValue + GetSkillValue();

        yield break;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];
        DamageType damageType = (DamageType)parameter["DamageType"];

        return launchedSkill.gameObject == gameObject && damageType == DamageType.Real;
    }

    /// <summary>
    /// 对方回合结束后，清除此技能
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^AfterRoundBattle$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillName", "degradation_derive");
        parameter1.Add("Source", "Skill.Degradation.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
    }

    /// <summary>
    /// 判断是否是对方回合
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
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
