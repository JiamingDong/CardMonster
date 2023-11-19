using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ����ʱ������Ч��
/// </summary>
public class OpportunityEffect : MonoBehaviour
{
    /// <summary>
    /// Ч���б�
    /// </summary>
    public List<Func<ParameterNode, IEnumerator>> effectList = new();


    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="opportunity">ʱ��</param>
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
        //��ȡЧ����������������
        string paramOpportunity = parameterNode.opportunity;

        //��ȡ�����ϵ���������
        Attribute[] attrs = Attribute.GetCustomAttributes(effect.Method);

        //�Ƚ���������
        foreach (Attribute attr in attrs)
        {
            if (attr is TriggerEffectConditionAttribute attribute)
            {
                string opportunity = attribute.GetOpportunity();
                string compareMethodName = attribute.compareMethodName;

                //�ж�ʱ��
                if (opportunity.Equals(paramOpportunity))
                {
                    //�жϱȽϷ���
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
