using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 进入战场，处于结算中的消耗品
/// </summary>
public class ConsumeInBattle : GameObjectInBattle
{
    public string id;
    public string cardName;
    public string skinId;
    public string kind;
    public string type;
    private int cost;
    public string race;

    /// <summary>
    /// 技能配置json，同数据库样式
    /// </summary>
    public string skill;
    /// <summary>
    /// 精英技能配置json，同数据库样式
    /// </summary>
    public string eliteSkill;

    /// <summary>
    /// 所有技能和状态
    /// </summary>
    public Dictionary<string, SkillInBattle> eliteSkillDictionary = new();

    public void SetCost(int cost)
    {
        this.cost = cost;
    }
    public int GetCost()
    {
        return cost;
    }

    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        string effectName = effect.Method.Name;
        string fullName = "ConsumeInBattle." + effectName;

        ParameterNode parameterNode = new();
        parameterNode.SetParent(new(), ParameterNodeChildType.EffectChild);
        parameterNode.parameter = parameter;

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, fullName, parameterNode, effect));
        yield return null;
    }

    /// <summary>
    /// 生成的时候调用
    /// </summary>
    public IEnumerator Generate(Dictionary<string, string> cardData)
    {
        id = cardData["CardID"];
        cardName = cardData["CardName"];
        type = cardData["CardType"];
        kind = cardData["CardKind"];
        race = cardData["CardRace"];
        //maxHp = (int)cardData["CardHP"];
        skinId = cardData["CardSkinID"];
        int cost = Convert.ToInt32(cardData["CardCost"]);
        //skillConfig = (Dictionary<string, int>)cardData["CardSkill"];
        skill = cardData["CardSkill"];
        //eliteSkillConfig = (Dictionary<string, Dictionary<string, object>>)cardData["CardEliteSkill"];
        eliteSkill = cardData["CardEliteSkill"];

        SetCost(cost);

        Dictionary<string, int> skillConfig = JsonConvert.DeserializeObject<Dictionary<string, int>>(skill);
        foreach (var keyValuePair in skillConfig)
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("SkillName", keyValuePair.Key);
            parameter.Add("SkillValue", keyValuePair.Value);
            parameter.Add("Source", "Consume");
            yield return StartCoroutine(DoAction(AddSkill, parameter));
            //AddSkill(keyValuePair.Key, keyValuePair.Value, "Monster");
        }

        if (eliteSkill != null && eliteSkill != "")
        {
            Debug.Log(eliteSkill);
            Dictionary<string, string> kindConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(kind);
            Dictionary<string, Dictionary<string, object>> eliteSkillConfig = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(eliteSkill);
            foreach (var keyValuePair in eliteSkillConfig)
            {
                string skillName = keyValuePair.Value["name"].ToString();
                int skillValue = Convert.ToInt32(keyValuePair.Value["value"].ToString());

                string kind = keyValuePair.Key == "leftSkill" ? kindConfig["leftKind"] : kindConfig["rightKind"];

                var allSkillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
                var skillClassName = allSkillConfig["SkillClassName"];
                Type type = Type.GetType(skillClassName);

                if (type != null)
                {
                    SkillInBattle skill = (SkillInBattle)gameObject.AddComponent(type);

                    skill.AddValue("Consume", skillValue);

                    eliteSkillDictionary.Add(kind, skill);
                }
            }

            foreach (var keyValuePair in eliteSkillDictionary)
            {
                Debug.Log(keyValuePair.Key);
                Debug.Log(keyValuePair.Value.GetType().Name);
            }
        }
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    public IEnumerator AddSkill(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        int skillValue = (int)parameter["SkillValue"];
        string source = (string)parameter["Source"];

        Debug.Log($"{cardName}添加技能{skillName}数值{skillValue}来源{source}");

        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
        var skillClassName = skillConfig["SkillClassName"];

        var skillTypeConfig = Database.cardMonster.Query("SkillTypeConfig", "and SkillName='" + skillName + "'")[0];
        var skillType = skillTypeConfig["SkillType"];

        //先看看是否已有这个技能
        foreach (SkillInBattle skillInCard in skillList)
        {
            if (skillInCard.GetType().Name.Equals(skillClassName))
            {
                skillValue = skillInCard.AddValue(source, skillValue);
                if (skillValue <= 0 && skillType == "value")
                {
                    skillList.Remove(skillInCard);
                }
                yield break;
            }
        }

        Type type = Type.GetType(skillClassName);

        if (type != null)
        {
            SkillInBattle skill = (SkillInBattle)gameObject.AddComponent(type);

            skill.AddValue(source, skillValue);
            skillList.Add(skill);
        }
    }

    /// <summary>
    /// 发动技能
    /// </summary>
    public new IEnumerator LaunchSkill(ParameterNode parameterNode)
    {
        Debug.Log("消耗品--发动技能");
        if (eliteSkillDictionary.Count > 0)
        {
            BattleProcess battleProcess = BattleProcess.GetInstance();

            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData playerData = battleProcess.systemPlayerData[i];

                if (playerData.consumeGameObject == gameObject)
                {
                    foreach (KeyValuePair<string, SkillInBattle> keyValuePair in eliteSkillDictionary)
                    {
                        string kind = keyValuePair.Key;
                        SkillInBattle skill = keyValuePair.Value;

                        for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
                        {
                            if (playerData.monsterGameObjectArray[j] != null)
                            {
                                MonsterInBattle monsterInBattle = playerData.monsterGameObjectArray[j].GetComponent<MonsterInBattle>();

                                Debug.Log(monsterInBattle.kind + "----" + kind);
                                if (monsterInBattle.kind == kind)
                                {
                                    yield return StartCoroutine(skill.ExecuteEligibleEffect(parameterNode));
                                    yield return null;

                                    break;
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }

        for (int i = 0; i < skillList.Count; i++)
        {
            Debug.Log("消耗品--发动技能2");
            SkillInBattle skill = skillList[i];
            yield return StartCoroutine(skill.ExecuteEligibleEffect(parameterNode));
            yield return null;
        }
    }
}
