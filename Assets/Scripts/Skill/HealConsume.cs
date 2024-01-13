using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 治疗（消耗品）
/// 使用后，使目标己方怪兽恢复%d点生命值
/// </summary>
public class HealConsume : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //治疗
        Dictionary<string, object> treatParameter = new();
        //当前技能
        treatParameter.Add("LaunchedSkill", this);
        //效果名称
        treatParameter.Add("EffectName", "Effect1");
        //受到治疗的怪兽
        treatParameter.Add("EffectTarget", consumeTarget);
        //治疗数值
        treatParameter.Add("TreatValue", GetSkillValue());

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = treatParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、目标是不是我方怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //使用手牌的玩家
        Player player = (Player)parameter["Player"];
        //手牌目标玩家
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        //目标怪兽
        GameObject consumeTarget = (GameObject)parameter["ConsumeTarget"];

        //消耗品物体
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        if (player != targetPlayer)
        {
            return false;
        }

        if (consumeTarget == null)
        {
            return false;
        }

        return true;
    }
}