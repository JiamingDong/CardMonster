using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 根据时机触发效果
/// </summary>
public abstract class OpportunityEffect : MonoBehaviour
{
    /// <summary>
    /// 效果列表
    /// </summary>
    public List<Func<ParameterNode, IEnumerator>> effectList = new();

    void Start()
    {
        //Debug.Log("初始化----" + GetType().Name);
        MethodInfo[] methodInfos = GetType().GetMethods();
        foreach (MethodInfo methodInfo in methodInfos)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(methodInfo);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is TriggerEffectAttribute)
                {
                    effectList.Add((Func<ParameterNode, IEnumerator>)Delegate.CreateDelegate(typeof(Func<ParameterNode, IEnumerator>), this, methodInfo));
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 发动技能
    /// </summary>
    /// <param name="opportunity">时机</param>
    public IEnumerator ExecuteEligibleEffect(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        //Debug.Log(parameterNode.opportunity);
        foreach (Func<ParameterNode, IEnumerator> effect in effectList)
        {
            if (!CompareCondition(effect, parameterNode))
            {
                continue;
            }

            string typeName = GetType().Name;
            string effectName = effect.Method.Name;
            string fullName = typeName + "." + effectName;

            yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(this, fullName, parameterNode, effect));
            //yield return null;
        }
        //Debug.Log(GetType().Name+"------------"+parameterNode.opportunity);
        //yield return null;
    }

    public bool CompareCondition(Func<ParameterNode, IEnumerator> effect, ParameterNode parameterNode)
    {
        //当前时机
        string paramOpportunity = parameterNode.opportunity;

        //获取方法上的所有特性
        Attribute[] attrs = Attribute.GetCustomAttributes(effect.Method);

        //比较所有条件
        foreach (Attribute attr in attrs)
        {
            if (attr is TriggerEffectAttribute attribute)
            {
                string opportunity = attribute.GetOpportunity();
                string compareMethodName = attribute.GetComparer();

                //判断时机
                if (Regex.IsMatch(paramOpportunity, opportunity))
                {
                    //用比较方法判断
                    if (compareMethodName != null)
                    {
                        MethodInfo methodInfo = GetType().GetMethod(compareMethodName);

                        if (methodInfo != null)
                        {
                            bool compareResult = (bool)methodInfo.Invoke(this, new object[] { parameterNode });
                            if (compareResult)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

#pragma warning disable CS0108
    [Obsolete("为防止技能所在物体被销毁导致游戏过程被中断，所有技能统一使用BattleProcess.GetInstance().StartCoroutine", true)]
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return null;
    }
#pragma warning restore CS0108
}
