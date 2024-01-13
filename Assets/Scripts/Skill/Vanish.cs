using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消灭
/// 使用时，消灭目标敌人
/// </summary>
public class Vanish : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> destroyParameter = new();
        destroyParameter.Add("LaunchedSkill", this);
        destroyParameter.Add("EffectName", "Effect1");
        destroyParameter.Add("EffectTarget", consumeTarget);
        destroyParameter.Add("Destroyer", gameObject);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = destroyParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyMonster, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、目标是不是敌方怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];
        Player targetPlayer = (Player)parameter["TargetPlayer"];

        //消耗品物体
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                //Debug.Log("闪电1");
                return false;
            }
        }
        else
        {
            //Debug.Log("闪电2");
            return false;
        }

        if (player == targetPlayer)
        {
            //Debug.Log("闪电3");
            return false;
        }

        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];
        if (consumeTarget == null)
        {
            //Debug.Log("闪电4");
            return false;
        }

        return true;
    }
}