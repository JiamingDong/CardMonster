using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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

    /// <summary>
    /// 结果
    /// </summary>
    public Dictionary<string, object> result = new();

    /// <summary>
    /// 父节点
    /// </summary>
    private ParameterNode parent;
    /// <summary>
    /// 替代时机子节点
    /// </summary>
    private ParameterNode replaceChild;
    /// <summary>
    /// 执行前时机子节点
    /// </summary>
    private ParameterNode beforeChild;
    /// <summary>
    /// 执行子节点
    /// </summary>
    private ParameterNode effectChild;
    /// <summary>
    /// 执行后时机子节点
    /// </summary>
    private ParameterNode afterChild;

    /// <summary>
    /// 方法执行过程中产生的节点的列表
    /// </summary>
    public List<ParameterNode> nodeInMethodList = new();
    /// <summary>
    /// 此节点所在列表的拥有者
    /// </summary>
    public ParameterNode superiorNode;

    public ParameterNode Parent { get => parent; }
    public void SetParent(ParameterNode parent, ParameterNodeChildType parameterNodeChildType)
    {
        this.parent = parent;
        switch (parameterNodeChildType)
        {
            case ParameterNodeChildType.ReplaceChild:
                parent.replaceChild = this;
                break;
            case ParameterNodeChildType.BeforeChild:
                parent.beforeChild = this;
                break;
            case ParameterNodeChildType.EffectChild:
                parent.effectChild = this;
                break;
            case ParameterNodeChildType.AfterChild:
                parent.afterChild = this;
                break;
        }
    }
    public ParameterNode ReplaceChild { get => replaceChild; }
    public ParameterNode BeforeChild { get => beforeChild; }
    public ParameterNode EffectChild { get => effectChild; }
    public ParameterNode AfterChild { get => afterChild; }

    public ParameterNode AddNodeInMethod()
    {
        ParameterNode parameterNode = new();
        nodeInMethodList.Add(parameterNode);
        parameterNode.superiorNode = this;
        return parameterNode;
    }

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
