using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 天选之人
/// 不能成为敌方消耗品的目标，<灵魂尖啸><沉默><恫吓><法术抑制><恐惧><惩罚>对此卡无效
/// </summary>
public class PowerfulImmunity : SkillInBattle
{
    /// <summary>
    /// 不能成为敌方消耗品的目标
    /// </summary>
    [TriggerEffect(@"^Replace\.RuleEvent\.CardBeUsed$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
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
        int handPanelNumber = (int)parameter["HandPanelNumber"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        Dictionary<string, string> cardData = new();
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == player)
            {
                cardData = handPanelNumber < 2 ? playerData.handMonster[handPanelNumber] : playerData.handItem[handPanelNumber - 2];
            }
        }

        string cardType = cardData["CardType"];

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

    /// <summary>
    /// <沉默><恫吓><法术抑制>对此卡无效
    /// </summary>
    [TriggerEffect(@"^Replace\.MonsterInBattle\.AddSkill$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断是否是此卡、技能是不是“被沉默”、“恫吓（衍生）”、“法术抑制（衍生）”、来源为“法术抑制”的“魔法”或来源为“恫吓”的“远程”和“近战”
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
            if (skillName.Equals("silence_derive") || skillName.Equals("demoralize_derive") || skillName.Equals("antimagic_derive"))
            {
                return true;
            }

            if (skillName.Equals("magic") && source.Equals("Skill.Antimagic"))
            {
                return true;
            }

            if ((skillName.Equals("melee") || skillName.Equals("ranged")) && source.Equals("Skill.Demoralize"))
            {
                return true;
            }
        }

        return false;
    }


    [TriggerEffect(@"^Replace\.GameAction\.MonsterInBattleToDeck$", "Compare3")]
    [TriggerEffect(@"^Replace\.GameAction\.SwapMonsterPosition$", "Compare4")]
    [TriggerEffect(@"^Replace\.GameAction\.ChangeMonsterCost$", "Compare5")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断目标是否是此卡、技能是不是灵魂尖啸
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        return effectTarget == gameObject && launchedSkill is SoulShriek;
    }

    /// <summary>
    /// 判断是否是本怪兽，技能是恐惧
    /// </summary>
    public bool Compare4(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        GameObject monsterMove1 = (GameObject)parameter["MonsterMove1"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        return monsterMove1 == gameObject && skillInBattle is Swap;
    }

    /// <summary>
    /// 判断是否是本怪兽，技能是惩罚
    /// </summary>
    public bool Compare5(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        GameObject effectTarget = (GameObject)parameter["EffectTarget"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        return effectTarget == gameObject && skillInBattle is Punishment;
    }
}
