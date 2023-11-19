using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 根据时机触发效果
/// </summary>
public class OpportunityEffect : MonoBehaviour
{
    /// <summary>
    /// 效果列表
    /// </summary>
    public List<Func<ParameterNode, IEnumerator>> effectList = new();


    /// <summary>
    /// 发动技能
    /// </summary>
    /// <param name="opportunity">时机</param>
    public IEnumerator ExecuteEligibleEffect(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        foreach (Func<ParameterNode, IEnumerator> effect in effectList)
        {
            if (!CompareCondition(effect, parameterNode))
            {
                continue;
            }

            string typeName = GetType().Name;

            string effectName = effect.Method.Name;

            string fullName = typeName + "." + effectName;

            yield return StartCoroutine(battleProcess.ExecuteEffect(this, effect, parameterNode.parameter, fullName));
            yield return null;
        }
    }

    private bool CompareCondition(Func<ParameterNode, IEnumerator> effect, ParameterNode parameterNode)
    {
        //获取效果发动的所有条件
        string paramOpportunity = parameterNode.opportunity;

        //获取方法上的所有特性
        Attribute[] attrs = Attribute.GetCustomAttributes(effect.Method);

        //比较所有条件
        foreach (Attribute attr in attrs)
        {
            if (attr is TriggerEffectConditionAttribute attribute)
            {
                string opportunity = attribute.GetOpportunity();
                string compareMethodName = attribute.compareMethodName;

                //判断时机
                if (opportunity.Equals(paramOpportunity))
                {
                    //判断比较方法
                    if (compareMethodName != null)
                    {
                        MethodInfo methodInfo = GetType().GetMethod(compareMethodName);

                        if (methodInfo != null)
                        {
                            bool compareResult = (bool)methodInfo.Invoke(null, new object[] { parameterNode });
                            if (compareResult)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
}
