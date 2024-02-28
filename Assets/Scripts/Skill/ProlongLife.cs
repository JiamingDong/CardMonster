using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 延年寿
/// 使用时：若自身下一顺位有其他怪兽，则令其骑乘乘黄。（进入“骑乘”状态，获得乘黄除“延年寿”外的所有技能，若乘黄离场，清除该状态）
/// </summary>
public class ProlongLife : SkillInBattle
{
    int launchMark = 0;

    public override int AddValue(string source, int value)
    {
        launchMark = 1;

        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] += value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
        Dictionary<string, string> cardData = monsterInBattle.cardData;
        string cardSkill = cardData["CardSkill"];
        Dictionary<string, int> skillDic = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardSkill);
        skillDic.Remove("prolong_life");

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length - 1; j++)
            {
                GameObject go = systemPlayerData.monsterGameObjectArray[j];
                if (go == gameObject)
                {

                    GameObject target = systemPlayerData.monsterGameObjectArray[j + 1];
                    MonsterInBattle monsterInBattle2 = target.GetComponent<MonsterInBattle>();

                    Dictionary<string, object> parameter2 = new();
                    parameter2.Add("LaunchedSkill", this);
                    parameter2.Add("EffectName", "Effect1");
                    parameter2.Add("SkillName", "prolong_life_derive");
                    parameter2.Add("SkillValue", 0);
                    parameter2.Add("Source", "Skill.ProlongLife.Effect1");
                    parameter2.Add("SkillFromProlongLife", skillDic);

                    ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    parameterNode2.parameter = parameter2;

                    yield return battleProcess.StartCoroutine(monsterInBattle2.DoAction(monsterInBattle2.AddSkill, parameterNode2));
                }
            }
        }

        launchMark = 0;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、我方有没有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchMark < 1)
        {
            return false;
        }

        //怪兽
        if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        //装备
        else if (result.ContainsKey("MonsterBeEquipped"))
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

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length - 1; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject && systemPlayerData.monsterGameObjectArray[j + 1] != null)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
