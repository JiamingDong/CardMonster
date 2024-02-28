using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// ����ʱ������Ч��
/// </summary>
public abstract class OpportunityEffect : MonoBehaviour
{
    /// <summary>
    /// Ч���б�
    /// </summary>
    public List<Func<ParameterNode, IEnumerator>> effectList = new();

    void Start()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MethodInfo[] methodInfos = GetType().GetMethods();
        foreach (MethodInfo methodInfo in methodInfos)
        {
            bool isEffect = false;

            Attribute[] attributes = Attribute.GetCustomAttributes(methodInfo);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is TriggerEffectAttribute triggerEffectAttribute)
                {
                    isEffect = true;

                    if (this is RuleEvent)
                    {
                        battleProcess.effectOpportunityRecord1.Add(triggerEffectAttribute.GetOpportunity());
                    }
                    else if (this is RuleEvent2)
                    {
                        //Debug.Log(triggerEffectAttribute.GetOpportunity());
                        battleProcess.effectOpportunityRecord2.Add(triggerEffectAttribute.GetOpportunity());
                    }
                    else if (this is SkillNotOnMonster)
                    {
                        battleProcess.effectOpportunityRecord4.Add(triggerEffectAttribute.GetOpportunity());
                    }
                    else
                    {
                        battleProcess.effectOpportunityRecord3.Add(triggerEffectAttribute.GetOpportunity());
                    }
                }
            }

            if (isEffect)
            {
                effectList.Add((Func<ParameterNode, IEnumerator>)Delegate.CreateDelegate(typeof(Func<ParameterNode, IEnumerator>), this, methodInfo));
            }
        }
    }

    /// <summary>
    /// �Ƴ�effectOpportunityRecord�еļ�¼
    /// </summary>
    void OnDestroy()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MethodInfo[] methodInfos = GetType().GetMethods();
        foreach (MethodInfo methodInfo in methodInfos)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(methodInfo);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is TriggerEffectAttribute triggerEffectAttribute)
                {
                    if (this is RuleEvent)
                    {
                        battleProcess?.effectOpportunityRecord1.Remove(triggerEffectAttribute.GetOpportunity());
                    }
                    else if (this is RuleEvent2)
                    {
                        battleProcess?.effectOpportunityRecord2.Remove(triggerEffectAttribute.GetOpportunity());
                    }
                    else if (this is SkillNotOnMonster)
                    {
                        battleProcess?.effectOpportunityRecord4.Remove(triggerEffectAttribute.GetOpportunity());
                    }
                    else
                    {
                        battleProcess?.effectOpportunityRecord3.Remove(triggerEffectAttribute.GetOpportunity());
                    }

                    break;
                }
            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="opportunity">ʱ��</param>
    public IEnumerator ExecuteEligibleEffect(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < effectList.Count; i++)
        {
            Func<ParameterNode, IEnumerator> effect = effectList[i];

            if (!CompareCondition(effect, parameterNode))
            {
                continue;
            }
            string typeName = GetType().Name;
            string effectName = effect.Method.Name;
            string fullName = typeName + "." + effectName;

            //Debug.Log("����" + fullName);

            yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(this, fullName, parameterNode, effect));
        }
    }

    public bool CompareCondition(Func<ParameterNode, IEnumerator> effect, ParameterNode parameterNode)
    {
        //��ǰʱ��
        string paramOpportunity = parameterNode.opportunity;

        //��ȡ�����ϵ���������
        Attribute[] attrs = Attribute.GetCustomAttributes(effect.Method);

        //�Ƚ���������
        foreach (Attribute attr in attrs)
        {
            if (attr is TriggerEffectAttribute attribute)
            {
                string opportunity = attribute.GetOpportunity();
                string compareMethodName = attribute.GetComparer();

                //�ж�ʱ��
                if (Regex.IsMatch(paramOpportunity, opportunity))
                {
                    //�ñȽϷ����ж�
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
    [Obsolete("Ϊ��ֹ�����������屻���ٵ�����Ϸ���̱��жϣ����м���ͳһʹ��BattleProcess.GetInstance().StartCoroutine", true)]
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return null;
    }
#pragma warning restore CS0108
}
