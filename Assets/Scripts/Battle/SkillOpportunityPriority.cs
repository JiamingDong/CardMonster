//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// ���ܵĶ�����ʱ�������ȼ�����
///// </summary>
//public class SkillOpportunityPriority: IComparable<SkillOpportunityPriority>
//{
//    /// <summary>
//    /// ������������
//    /// </summary>
//    private string name;
//    /// <summary>
//    /// ����
//    /// </summary>
//    private Func<Dictionary<string, object>, IEnumerator> skill;
//    /// <summary>
//    /// ʱ��
//    /// </summary>
//    private string opportunity;
//    /// <summary>
//    /// ���ȼ�
//    /// </summary>
//    private int priority;

//    public SkillOpportunityPriority(string name, Func<Dictionary<string, object>, IEnumerator> skill, string opportunity, int priority)
//    {
//        this.name = name;
//        this.skill = skill;
//        this.opportunity = opportunity;
//        this.priority = priority;
//    }

//    /// <summary>
//    /// ����
//    /// </summary>
//    public Func<Dictionary<string, object>, IEnumerator> Skill { get => skill; set => skill = value; }
//    /// <summary>
//    /// ʱ��
//    /// </summary>
//    public string Opportunity { get => opportunity; set => opportunity = value; }
//    /// <summary>
//    /// ���ȼ���ԽСԽ��
//    /// </summary>
//    public int Priority { get => priority; set => priority = value; }
//    /// <summary>
//    /// ������������
//    /// </summary>
//    public string Name { get => name; set => name = value; }

//    public int CompareTo(SkillOpportunityPriority other)
//    {
//        if (priority > other.priority) return 1;
//        else return -1;
//    }
//}
