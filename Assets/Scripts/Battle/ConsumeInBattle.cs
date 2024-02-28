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
    /// 原始卡牌数据
    /// </summary>
    public Dictionary<string, string> cardData;

    /// <summary>
    /// 技能配置json，同数据库样式
    /// </summary>
    public string skill;
    /// <summary>
    /// 精英技能配置json，同数据库样式
    /// </summary>
    public string eliteSkill;

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
        skinId = cardData["CardSkinID"];
        int cost = Convert.ToInt32(cardData["CardCost"]);
        skill = cardData["CardSkill"];
        eliteSkill = cardData["CardEliteSkill"];

        this.cardData = cardData;

        SetCost(cost);

        Dictionary<string, int> skillConfig = JsonConvert.DeserializeObject<Dictionary<string, int>>(skill);
        foreach (var keyValuePair in skillConfig)
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("SkillName", keyValuePair.Key);
            parameter.Add("SkillValue", keyValuePair.Value);
            parameter.Add("Source", "Consume");
            yield return StartCoroutine(DoAction(AddSkill, parameter));
        }

        if (!string.IsNullOrEmpty(eliteSkill))
        {
            Debug.Log(eliteSkill);

            BattleProcess battleProcess = BattleProcess.GetInstance();

            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData playerData = battleProcess.systemPlayerData[i];

                if (playerData.consumeGameObject == gameObject)
                {
                    for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
                    {
                        if (playerData.monsterGameObjectArray[j] != null)
                        {
                            MonsterInBattle monsterInBattle = playerData.monsterGameObjectArray[j].GetComponent<MonsterInBattle>();

                            Debug.Log(monsterInBattle.kind + "----" + kind);

                            set.Add(monsterInBattle.kind);
                        }
                    }
                }
            }

            Dictionary<string, string> kindConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(kind);
            Dictionary<string, Dictionary<string, object>> eliteSkillConfig = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(eliteSkill);
            foreach (var keyValuePair in eliteSkillConfig)
            {
                string skillName = keyValuePair.Value["name"].ToString();
                int skillValue = Convert.ToInt32(keyValuePair.Value["value"].ToString());

                string kind = keyValuePair.Key == "leftSkill" ? kindConfig["leftKind"] : kindConfig["rightKind"];

                if (set.Contains(kind))
                {
                    Dictionary<string, object> parameter = new();
                    parameter.Add("SkillName", skillName);
                    parameter.Add("SkillValue", skillValue);
                    parameter.Add("Source", "Consume");
                    yield return StartCoroutine(DoAction(AddSkill, parameter));
                }
            }

            foreach (var keyValuePair in skillList)
            {
                Debug.Log(keyValuePair.GetType().Name);
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

        BattleProcess battleProcess = BattleProcess.GetInstance();

        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
        var skillClassName = skillConfig["SkillClassName"];
        var skillType = skillConfig["TypeInBattle"];

        //先看看是否已有这个技能
        foreach (SkillInBattle skillInCard in skillList)
        {
            if (skillInCard.GetType().Name.Equals(skillClassName))
            {
                skillValue = skillInCard.AddValue(source, skillValue);
                if (skillValue <= 0 && skillType == "value")
                {
                    skillList.Remove(skillInCard);

                    //排序
                    skillList.Sort((a, b) => battleProcess.skillPriority[b.GetType().Name].CompareTo(battleProcess.skillPriority[a.GetType().Name]));
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

            //排序
            skillList.Sort((a, b) => battleProcess.skillPriority[b.GetType().Name].CompareTo(battleProcess.skillPriority[a.GetType().Name]));
        }
    }
}
