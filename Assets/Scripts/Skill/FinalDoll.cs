using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 终末之偶
/// <近战><远程><魔法><随机伤害>+X，X为5减去我方怪兽牌库剩余数量（需要具有对应技能）
/// </summary>
public class FinalDoll : SkillInBattle
{
    /// <summary>
    /// 获得技能后获得5-X的属性
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        switch (skillName)
        {
            case "magic":
                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "Effect1");
                parameter1.Add("SkillName", "magic");
                parameter1.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));

                if (cardAmount < GetSkillValue())
                {
                    if (gameObject.TryGetComponent(out Magic _))
                    {
                        Dictionary<string, object> parameter5 = new();
                        parameter5.Add("LaunchedSkill", this);
                        parameter5.Add("EffectName", "Effect1");
                        parameter5.Add("SkillName", "magic");
                        parameter5.Add("SkillValue", GetSkillValue() - cardAmount);
                        parameter5.Add("Source", "Skill.FinalDoll.Effect1");

                        ParameterNode parameterNode5 = parameterNode.AddNodeInMethod();
                        parameterNode5.parameter = parameter5;
                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode5));
                    }
                }
                break;

            case "melee":
                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect1");
                parameter2.Add("SkillName", "melee");
                parameter2.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));

                if (cardAmount < GetSkillValue())
                {
                    if (gameObject.TryGetComponent(out Melee _))
                    {
                        Dictionary<string, object> parameter6 = new();
                        parameter6.Add("LaunchedSkill", this);
                        parameter6.Add("EffectName", "Effect1");
                        parameter6.Add("SkillName", "melee");
                        parameter6.Add("SkillValue", GetSkillValue() - cardAmount);
                        parameter6.Add("Source", "Skill.FinalDoll.Effect1");

                        ParameterNode parameterNode6 = parameterNode.AddNodeInMethod();
                        parameterNode6.parameter = parameter6;
                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode6));
                    }
                }
                break;

            case "ranged":
                Dictionary<string, object> parameter3 = new();
                parameter3.Add("LaunchedSkill", this);
                parameter3.Add("EffectName", "Effect1");
                parameter3.Add("SkillName", "ranged");
                parameter3.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode3 = parameterNode.AddNodeInMethod();
                parameterNode3.parameter = parameter3;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode3));

                if (cardAmount < GetSkillValue())
                {
                    if (gameObject.TryGetComponent(out Ranged _))
                    {
                        Dictionary<string, object> parameter7 = new();
                        parameter7.Add("LaunchedSkill", this);
                        parameter7.Add("EffectName", "Effect1");
                        parameter7.Add("SkillName", "ranged");
                        parameter7.Add("SkillValue", GetSkillValue() - cardAmount);
                        parameter7.Add("Source", "Skill.FinalDoll.Effect1");

                        ParameterNode parameterNode7 = parameterNode.AddNodeInMethod();
                        parameterNode7.parameter = parameter7;
                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode7));
                    }
                }
                break;

            case "chance":
                Dictionary<string, object> parameter4 = new();
                parameter4.Add("LaunchedSkill", this);
                parameter4.Add("EffectName", "Effect1");
                parameter4.Add("SkillName", "chance");
                parameter4.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
                parameterNode4.parameter = parameter4;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode4));

                if (cardAmount < GetSkillValue())
                {
                    if (gameObject.TryGetComponent(out Chance _))
                    {
                        Dictionary<string, object> parameter8 = new();
                        parameter8.Add("LaunchedSkill", this);
                        parameter8.Add("EffectName", "Effect1");
                        parameter8.Add("SkillName", "chance");
                        parameter8.Add("SkillValue", GetSkillValue() - cardAmount);
                        parameter8.Add("Source", "Skill.FinalDoll.Effect1");

                        ParameterNode parameterNode8 = parameterNode.AddNodeInMethod();
                        parameterNode8.parameter = parameter8;
                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode8));
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 判断是否是此卡，且是基础攻击技能，牌库牌数
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (parameter.ContainsKey("LaunchedSkill"))
        {
            SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

            if (skillInBattle == this)
            {
                return false;
            }
        }

        if (monsterInBattle.gameObject != gameObject)
        {
            return false;
        }

        if (skillName != "magic" && skillName != "melee" && skillName != "ranged" && skillName != "chance")
        {
            return false;
        }

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        if (cardAmount >= GetSkillValue())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 牌离开牌库属性+1
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^After\.GameAction\.CardLeaveLibrary$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        if (cardAmount < GetSkillValue())
        {
            MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

            if (gameObject.TryGetComponent(out Magic magic))
            {
                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect1");
                parameter2.Add("SkillName", "magic");
                parameter2.Add("SkillValue", GetSkillValue() - cardAmount);
                parameter2.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;
                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
            }

            if (gameObject.TryGetComponent(out Melee melee))
            {
                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect1");
                parameter2.Add("SkillName", "melee");
                parameter2.Add("SkillValue", GetSkillValue() - cardAmount);
                parameter2.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;
                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
            }

            if (gameObject.TryGetComponent(out Ranged ranged))
            {
                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect1");
                parameter2.Add("SkillName", "ranged");
                parameter2.Add("SkillValue", GetSkillValue() - cardAmount);
                parameter2.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;
                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
            }

            if (gameObject.TryGetComponent(out Chance chance))
            {
                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect1");
                parameter2.Add("SkillName", "chance");
                parameter2.Add("SkillValue", GetSkillValue() - cardAmount);
                parameter2.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;
                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
            }
        }

    }

    /// <summary>
    /// 判断是否被使用的是此卡
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string type = (string)parameter["Type"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (type != "monster")
        {
            return false;
        }

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        cardAmount = systemPlayerData.monsterDeck.Count;
                        break;
                    }
                }
            }
        }

        if (cardAmount >= GetSkillValue())
        {
            return false;
        }

        return true;
    }
}