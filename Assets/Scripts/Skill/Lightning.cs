using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 闪电
/// 使用时，对目标敌人造成%d点法术伤害
/// </summary>
public class Lightning : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //伤害参数
        Dictionary<string, object> damageParameter = new();
        //当前技能
        damageParameter.Add("LaunchedSkill", this);
        //效果名称
        damageParameter.Add("EffectName", "Effect1");
        //受到伤害的怪兽
        damageParameter.Add("EffectTarget", consumeTarget);
        //伤害数值
        damageParameter.Add("DamageValue", GetSkillValue());
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Magic);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
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
