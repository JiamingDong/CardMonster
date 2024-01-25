using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生命之力
/// 我方战斗阶段开始时，所有盟友的<治疗>值永久+X，不具有<治疗>的盟友永久获得<治疗X>（X为%d）
/// </summary>
public class ForceOfLife : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                GameObject monsterGameObject = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                if (monsterGameObject == gameObject)
                {
                    for (int k = 2; k > -1; k--)
                    {
                        GameObject go = battleProcess.systemPlayerData[i].monsterGameObjectArray[k];
                        if (go != null)
                        {
                            MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();

                            Dictionary<string, object> parameter = new();
                            parameter.Add("LaunchedSkill", this);
                            parameter.Add("EffectName", "Effect1");
                            parameter.Add("SkillName", "heal");
                            parameter.Add("SkillValue", GetSkillValue());
                            parameter.Add("Source", "Skill.ForceOfLife.Effect1");

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = parameter;
                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                        }
                    }

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是当前回合角色
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Ally)
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