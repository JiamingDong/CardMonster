using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手牌中的技能
/// </summary>
public class SkillNotOnMonster : OpportunityEffect
{
    /// <summary>
    /// 本类实例
    /// </summary>
    private static SkillNotOnMonster instance;

    /// <summary>
    /// 获得本类实例
    /// </summary>
    /// <returns></returns>
    public static SkillNotOnMonster GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Find("BattleProcessSystem").GetComponent<SkillNotOnMonster>();
        }
        return instance;
    }

    /// <summary>
    /// 触发手牌中的“额外水晶”
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^Before\.GameAction\.Sacrifice$", "Compare1")]
    public IEnumerator Crystal(ParameterNode parameterNode)
    {
        Debug.Log("触发手牌中的“额外水晶”");

        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        foreach (PlayerData playerData in battleProcess.systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                Dictionary<string, string> handCard = null;
                switch (objectBeSacrificedNumber)
                {
                    case 0:
                        handCard = playerData.handMonster[0];
                        break;
                    case 1:
                        handCard = playerData.handMonster[1];
                        break;
                    case 2:
                        handCard = playerData.handItem[0];
                        break;
                    case 3:
                        handCard = playerData.handItem[1];
                        break;
                }

                string cardP = handCard["CardSkill"];
                Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);

                int crystalAmount = Convert.ToInt32(pd["crystal"]);

                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect1");
                parameter2.Add("Player", player);
                parameter2.Add("SkillName", "crystal_derive");
                parameter2.Add("SkillValue", crystalAmount);
                parameter2.Add("Source", "Skill.Crystal.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddPlayerSkill, parameterNode2));

                yield break;
            }
        }
    }

    /// <summary>
    /// 判断被献祭的物体拥有“额外水晶”
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber <= 3)
        {
            foreach (PlayerData playerData in battleProcess.systemPlayerData)
            {
                if (playerData.perspectivePlayer == player)
                {
                    Dictionary<string, string> handCard = null;
                    switch (objectBeSacrificedNumber)
                    {
                        case 0:
                            handCard = playerData.handMonster[0];
                            break;
                        case 1:
                            handCard = playerData.handMonster[1];
                            break;
                        case 2:
                            handCard = playerData.handItem[0];
                            break;
                        case 3:
                            handCard = playerData.handItem[1];
                            break;
                    }

                    if (handCard != null)
                    {
                        string cardP = handCard["CardSkill"];
                        if (cardP != null && !cardP.Equals(""))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("crystal") && Convert.ToInt32(pd["crystal"]) > 0)
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 触发手牌中的“无法使用”
    /// </summary>
    [TriggerEffect(@"^Replace\.RuleEvent\.CardBeUsed$", "Compare2")]
    public IEnumerator ForbidUse(ParameterNode parameterNode)
    {
        Debug.Log("触发手牌中的“无法使用”");

        Dictionary<string, object> result = parameterNode.Parent.result;

        result.Add("BeReplaced", true);

        yield break;
    }

    /// <summary>
    /// 判断是否是手牌，且拥有“无法使用”
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        Dictionary<string, object> parameter = parameterNode.parameter;
        int handPanelNumber = (int)parameter["HandPanelNumber"];
        Player player = (Player)parameter["Player"];

        if (handPanelNumber >= 0 && handPanelNumber <= 3)
        {
            foreach (PlayerData playerData in battleProcess.systemPlayerData)
            {
                if (playerData.perspectivePlayer == player)
                {
                    Dictionary<string, string> handCard = null;
                    switch (handPanelNumber)
                    {
                        case 0:
                            handCard = playerData.handMonster[0];
                            break;
                        case 1:
                            handCard = playerData.handMonster[1];
                            break;
                        case 2:
                            handCard = playerData.handItem[0];
                            break;
                        case 3:
                            handCard = playerData.handItem[1];
                            break;
                    }

                    if (handCard != null)
                    {
                        string cardP = handCard["CardSkill"];
                        if (!string.IsNullOrEmpty(cardP))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("forbid_use"))
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 触发手牌中的“无法献祭”
    /// </summary>
    [TriggerEffect(@"^Replace\.RuleEvent\.CardBeSacrificed$", "Compare3")]
    public IEnumerator ForbidSacrifice(ParameterNode parameterNode)
    {
        Debug.Log("触发手牌中的“无法献祭”");

        Dictionary<string, object> result = parameterNode.Parent.result;

        result.Add("BeReplaced", true);

        yield break;
    }

    /// <summary>
    /// 判断是否是手牌，且拥有“无法献祭”
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber <= 3)
        {
            foreach (PlayerData playerData in battleProcess.systemPlayerData)
            {
                if (playerData.perspectivePlayer == player)
                {
                    Dictionary<string, string> handCard = null;
                    switch (objectBeSacrificedNumber)
                    {
                        case 0:
                            handCard = playerData.handMonster[0];
                            break;
                        case 1:
                            handCard = playerData.handMonster[1];
                            break;
                        case 2:
                            handCard = playerData.handItem[0];
                            break;
                        case 3:
                            handCard = playerData.handItem[1];
                            break;
                    }

                    if (handCard != null)
                    {
                        string cardP = handCard["CardSkill"];
                        if (!string.IsNullOrEmpty(cardP))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("forbid_sacrifice"))
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 触发手牌中的“恶化”
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^Before\.GameAction\.Sacrifice$", "Compare4")]
    public IEnumerator Degradation(ParameterNode parameterNode)
    {
        Debug.Log("触发手牌中的“恶化”");

        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        foreach (PlayerData playerData in battleProcess.systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                Dictionary<string, string> handCard = null;
                switch (objectBeSacrificedNumber)
                {
                    case 0:
                        handCard = playerData.handMonster[0];
                        break;
                    case 1:
                        handCard = playerData.handMonster[1];
                        break;
                    case 2:
                        handCard = playerData.handItem[0];
                        break;
                    case 3:
                        handCard = playerData.handItem[1];
                        break;
                }

                string cardP = handCard["CardSkill"];
                Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);

                int skillValue = Convert.ToInt32(pd["degradation"]);

                for (int k = 0; k < playerData.monsterGameObjectArray.Length; k++)
                {
                    if (playerData.monsterGameObjectArray[k] != null)
                    {
                        MonsterInBattle monsterInBattle = playerData.monsterGameObjectArray[k].GetComponent<MonsterInBattle>();

                        Dictionary<string, object> parameter2 = new();
                        parameter2.Add("LaunchedSkill", this);
                        parameter2.Add("EffectName", "Effect1");
                        parameter2.Add("SkillName", "degradation_derive");
                        parameter2.Add("SkillValue", skillValue);
                        parameter2.Add("Source", "Skill.Degradation.Effect1");

                        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                        parameterNode2.parameter = parameter2;

                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 判断被献祭的物体拥有“恶化”
    /// </summary>
    public bool Compare4(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber <= 3)
        {
            foreach (PlayerData playerData in battleProcess.systemPlayerData)
            {
                if (playerData.perspectivePlayer == player)
                {
                    Dictionary<string, string> handCard = null;
                    switch (objectBeSacrificedNumber)
                    {
                        case 0:
                            handCard = playerData.handMonster[0];
                            break;
                        case 1:
                            handCard = playerData.handMonster[1];
                            break;
                        case 2:
                            handCard = playerData.handItem[0];
                            break;
                        case 3:
                            handCard = playerData.handItem[1];
                            break;
                    }

                    if (handCard != null)
                    {
                        string cardP = handCard["CardSkill"];
                        if (cardP != null && !cardP.Equals(""))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("degradation") && Convert.ToInt32(pd["degradation"]) > 0)
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        return false;
    }
}
