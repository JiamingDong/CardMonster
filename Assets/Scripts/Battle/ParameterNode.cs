using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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

    /// <summary>
    /// ���
    /// </summary>
    public Dictionary<string, object> result = new();

    /// <summary>
    /// ���ڵ�
    /// </summary>
    private ParameterNode parent;
    /// <summary>
    /// ���ʱ���ӽڵ�
    /// </summary>
    private ParameterNode replaceChild;
    /// <summary>
    /// ִ��ǰʱ���ӽڵ�
    /// </summary>
    private ParameterNode beforeChild;
    /// <summary>
    /// ִ���ӽڵ�
    /// </summary>
    private ParameterNode effectChild;
    /// <summary>
    /// ִ�к�ʱ���ӽڵ�
    /// </summary>
    private ParameterNode afterChild;

    /// <summary>
    /// ����ִ�й����в����Ľڵ���б�
    /// </summary>
    public List<ParameterNode> nodeInMethodList = new();
    /// <summary>
    /// �˽ڵ������б��ӵ����
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
