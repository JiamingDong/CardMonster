using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 免疫
/// 不能成为敌方消耗品目标，<灵魂尖啸><沉默><恫吓><法术抑制>对此卡无效
/// </summary>
public class Immunity : SkillInBattle
{
    [TriggerEffect(@"^Replace\.RuleEvent\.CardBeUsed$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// 被使用的是消耗品，目标是本怪兽，是对方使用的
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        //使用牌的玩家
        Player player = (Player)parameter["Player"];
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        int battlePanelNumber = (int)parameter["BattlePanelNumber"];
        Dictionary<string, object> cardDataInBattle = (Dictionary<string, object>)parameter["CardDataInBattle"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        string cardType = (string)cardDataInBattle["CardType"];

        if (!cardType.Equals("consume"))
        {
            return false;
        }

        bool isEnemy = false;
        bool isThis = false;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        isEnemy = true;
                    }
                }
            }

            if (systemPlayerData.perspectivePlayer == targetPlayer && systemPlayerData.monsterGameObjectArray[battlePanelNumber] == gameObject)
            {
                isThis = true;
            }
        }

        return isEnemy && isThis;
    }

    [TriggerEffect(@"^Replace\.MonsterInBattle\.AddSkill$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// 判断目标是否是此卡、技能是不是“被沉默”、“恫吓（衍生）”、“法术抑制（衍生）”、来源为“法术抑制”的“魔法”或来源为“恫吓”的“远程”和“近战”
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (monsterInBattle.gameObject == gameObject)
        {
            if (skillName.Equals("SilenceDerive") || skillName.Equals("DemoralizeDerive") || skillName.Equals("AntimagicDerive"))
            {
                return true;
            }

            if (skillName.Equals("Magic") && source.Equals("Skill.Antimagic"))
            {
                return true;
            }

            if ((skillName.Equals("Melee") || skillName.Equals("Ranged")) && source.Equals("Skill.Demoralize"))
            {
                return true;
            }
        }

        return false;
    }

    [TriggerEffect(@"^Replace\.GameAction\.MonsterInBattleToDeck$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// 判断目标是否是此卡、技能是不是灵魂尖啸
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];

        return effectTarget == gameObject && launchedSkill is SoulShriek;
    }
}
