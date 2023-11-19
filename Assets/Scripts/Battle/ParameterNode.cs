using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 参数树节点
/// </summary>
public class ParameterNode
{
    /// <summary>
    /// 创建者
    /// </summary>
    public object creator = new();

    /// <summary>
    /// 时机
    /// </summary>
    public string opportunity;

    /// <summary>
    /// 参数
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
