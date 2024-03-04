using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 额外水晶
/// 被献祭时，额外获得%d点水晶
/// 手牌中的“额外水晶”通过RuleEvent.Crystal触发
/// </summary>
public class Crystal : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.Sacrifice$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, object> parameter2 = new();
                    parameter2.Add("LaunchedSkill", this);
                    parameter2.Add("EffectName", "Effect1");
                    parameter2.Add("Player", systemPlayerData.perspectivePlayer);
                    parameter2.Add("SkillName", "crystal_derive");
                    parameter2.Add("SkillValue", GetSkillValue());
                    parameter2.Add("Source", "Skill.Crystal.Effect1");

                    ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    parameterNode2.parameter = parameter2;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddPlayerSkill, parameterNode2));

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (objectBeSacrificedNumber >= 4 && objectBeSacrificedNumber <= 6)
        {
            int t = objectBeSacrificedNumber - 4;

            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

                if (systemPlayerData.perspectivePlayer == player)
                {
                    if (systemPlayerData.monsterGameObjectArray[t] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
