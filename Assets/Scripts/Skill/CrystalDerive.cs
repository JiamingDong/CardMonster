using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 额外水晶（衍生）
/// 我方下一次因为献祭获得水晶时，额外获得X点
/// </summary>
public class CrystalDerive : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.ChangeCrystalAmount$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Debug.Log("触发手牌中的“额外水晶（衍生）”");

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> parameter = parameterNode.parameter;
        int crystalAmount = (int)parameter["CrystalAmount"];
        Player player = (Player)parameter["Player"];

        parameter["CrystalAmount"] = crystalAmount + GetSkillValue();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("Player", player);
        parameter1.Add("SkillName", "crystal_derive");
        parameter1.Add("Source", "Skill.Crystal.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DeletePlayerSkillSource, parameterNode1));
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        if (parameter.ContainsKey("LaunchedSkill") && parameter.ContainsKey("EffectName"))
        {
            object launchedSkill = parameter["LaunchedSkill"];
            string effectName = (string)parameter["EffectName"];

            if (launchedSkill is GameAction && effectName == "Sacrifice")
            {
                return true;
            }
        }

        return false;
    }
}