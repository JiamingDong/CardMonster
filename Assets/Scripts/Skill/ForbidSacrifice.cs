using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 无法献祭
/// 此卡无法献祭
/// </summary>
public class ForbidSacrifice : SkillInBattle
{
    [TriggerEffect(@"^Replace\.RuleEvent\.CardBeSacrificed$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;

        result.Add("BeReplaced", true);

        yield break;
    }

    /// <summary>
    /// 判断被献祭的是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        Dictionary<string, object> parameter = parameterNode.Parent.parameter;

        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

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