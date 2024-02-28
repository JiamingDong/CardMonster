using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗阵型
/// 我方战斗阶段开始时，我方每有一只怪兽提升我方全体怪兽%d点近战和远程，我方所有怪兽免疫恐惧，直到对方回合结束。（多个仅最大值生效）
/// </summary>
public class Formation : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        int monsterAmount = 0;
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
                            monsterAmount = k + 1 > monsterAmount ? k + 1 : monsterAmount;

                            MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();

                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("LaunchedSkill", this);
                            parameter2.Add("EffectName", "Effect1");
                            parameter2.Add("SkillName", "formation_derive");
                            parameter2.Add("SkillValue", GetSkillValue() * monsterAmount);
                            parameter2.Add("Source", "Skill.Formation.Effect1");

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;
                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
                        }
                    }
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