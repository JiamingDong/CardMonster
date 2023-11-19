using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������ڵ�
/// </summary>
public class ParameterNode
{
    /// <summary>
    /// ������
    /// </summary>
    public object creator = new();

    /// <summary>
    /// ʱ��
    /// </summary>
    public string opportunity;

    /// <summary>
    /// ����
    /// </summary>
    public Dictionary<string, object> parameter = new();

    public override string ToString()
    {
        string parameterString = "{";
        foreach (var item in parameter)
        {
            parameterString += "\"" + item.Key + "\":\"" + item.Value.ToString() + "\",";
        }
        parameterString = parameterString[..^1] + "}";
        
        return "ParameterNode.ToString():opportunity=" + opportunity + ",parameter=" + parameterString;
    }
}
