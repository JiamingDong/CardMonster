using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 霸权
/// 装备装备卡时，获得该装备两倍的基础攻击力和护甲
/// </summary>
public class StrengthenArmed : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        List<SkillInBattle> skillInBattles = monsterInBattle.skillList;

        for (int i = 0; i < skillInBattles.Count; i++)
        {
            SkillInBattle skillInBattle = skillInBattles[i];
            if (SkillUtils.IsBasicAttackEffect(skillInBattle) || skillInBattle is Armor)
            {
                Dictionary<string, int> sourceAndValue = skillInBattle.sourceAndValue;
                if (sourceAndValue.ContainsKey("Equipment"))
                {
                    var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillClassName='" + skillInBattle.GetType().Name + "'")[0];
                    var skillEnglishName = skillConfig["SkillEnglishName"];

                    int skillValue = sourceAndValue["Equipment"];

                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("LaunchedSkill", this);
                    parameter1.Add("EffectName", "Effect1");
                    parameter1.Add("SkillName", skillEnglishName);
                    parameter1.Add("SkillValue", skillValue);
                    parameter1.Add("Source", "Skill.StrengthenArmed.Effect1");

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                    yield return null;
                }
            }
        }
    }

    /// <summary>
    /// 判断是否被使用的是此卡、有没有基础技能或护甲
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //使用手牌的玩家
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //装备
        if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                return false;
            }
        }

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        List<SkillInBattle> skillInBattles = monsterInBattle.skillList;

        for (int i = 0; i < skillInBattles.Count; i++)
        {
            SkillInBattle skillInBattle = skillInBattles[i];
            if (SkillUtils.IsBasicAttackEffect(skillInBattle) || skillInBattle is Armor)
            {
                Dictionary<string, int> sourceAndValue = skillInBattle.sourceAndValue;
                if (sourceAndValue.ContainsKey("Equipment"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
