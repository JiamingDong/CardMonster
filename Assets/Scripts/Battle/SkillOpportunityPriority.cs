//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 技能的动作、时机和优先级的类
///// </summary>
//public class SkillOpportunityPriority: IComparable<SkillOpportunityPriority>
//{
//    /// <summary>
//    /// 所属技能名字
//    /// </summary>
//    private string name;
//    /// <summary>
//    /// 技能
//    /// </summary>
//    private Func<Dictionary<string, object>, IEnumerator> skill;
//    /// <summary>
//    /// 时机
//    /// </summary>
//    private string opportunity;
//    /// <summary>
//    /// 优先级
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
//    /// 技能
//    /// </summary>
//    public Func<Dictionary<string, object>, IEnumerator> Skill { get => skill; set => skill = value; }
//    /// <summary>
//    /// 时机
//    /// </summary>
//    public string Opportunity { get => opportunity; set => opportunity = value; }
//    /// <summary>
//    /// 优先级，越小越早
//    /// </summary>
//    public int Priority { get => priority; set => priority = value; }
//    /// <summary>
//    /// 所属技能名字
//    /// </summary>
//    public string Name { get => name; set => name = value; }

//    public int CompareTo(SkillOpportunityPriority other)
//    {
//        if (priority > other.priority) return 1;
//        else return -1;
//    }
//}
