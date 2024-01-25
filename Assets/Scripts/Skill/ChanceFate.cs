using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 强运
/// 我方战斗阶段开始时，所有与自身同阵营盟友获得“强运（衍生）”。（<随机伤害>造成最大值伤害。对方回合结束时，清除【强运】效果）
/// </summary>
public class ChanceFate : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

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
                            MonsterInBattle monsterInBattle1 = go.GetComponent<MonsterInBattle>();

                            if (monsterInBattle.kind == monsterInBattle1.kind)
                            {
                                Dictionary<string, object> parameter2 = new();
                                parameter2.Add("LaunchedSkill", this);
                                parameter2.Add("EffectName", "Effect1");
                                parameter2.Add("SkillName", "chance_fate_derive");
                                parameter2.Add("SkillValue", 0);
                                parameter2.Add("Source", "Skill.ChanceFate.Effect1");

                                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                                parameterNode2.parameter = parameter2;
                                yield return battleProcess.StartCoroutine(monsterInBattle1.DoAction(monsterInBattle1.AddSkill, parameterNode2));
                            }
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