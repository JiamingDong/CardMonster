using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ѡ֮��
/// ���ܳ�Ϊ�з�����Ʒ��Ŀ�꣬<����Х><��Ĭ><����><��������><�־�><�ͷ�>�Դ˿���Ч
/// </summary>
public class PowerfulImmunity : SkillInBattle
{
    /// <summary>
    /// ���ܳ�Ϊ�з�����Ʒ��Ŀ��
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
    /// ��ʹ�õ�������Ʒ��Ŀ���Ǳ����ޣ��ǶԷ�ʹ�õ�
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        //ʹ���Ƶ����
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
    /// <��Ĭ><����><��������>�Դ˿���Ч
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
    /// �ж��Ƿ��Ǵ˿��������ǲ��ǡ�����Ĭ���������ţ��������������������ƣ�������������ԴΪ���������ơ��ġ�ħ��������ԴΪ�����š��ġ�Զ�̡��͡���ս��
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
    /// �ж�Ŀ���Ƿ��Ǵ˿��������ǲ�������Х
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
    /// �ж��Ƿ��Ǳ����ޣ������ǿ־�
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
    /// �ж��Ƿ��Ǳ����ޣ������ǳͷ�
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
