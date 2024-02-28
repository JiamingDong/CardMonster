using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ���ָ�ӣ�������
/// Զ�̲��ᱻ�����ס����ܡ��Է��غϽ���ʱ����������ָ�ӡ�Ч��
/// </summary>
public class ShootCommandDerive : SkillInBattle
{
    public override int AddValue(string source, int value)
    {
        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] = sourceAndValue[source] > value ? sourceAndValue[source] : value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    public override int GetSkillValue()
    {
        return sourceAndValue.Count == 0 ? -1 : sourceAndValue.Values.Max();
    }

    /// <summary>
    /// ��ô˼���ʱ������ԴΪ�˼��ܵĽ�ս��ֵС�ڴ˼���ֵ����õ��ڲ�ֵ�Ľ�ս
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int skillValue = (int)parameter["SkillValue"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Ranged ranged = gameObject.GetComponent<Ranged>();

        int currentValue = 0;

        Dictionary<string, int> keyValuePairs = ranged.sourceAndValue;
        if (keyValuePairs.ContainsKey("Skill.ShootCommandDerive.Effect1"))
        {
            currentValue = keyValuePairs["Skill.ShootCommandDerive.Effect1"];
        }

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillName", "ranged");
        parameter1.Add("SkillValue", GetSkillValue() - currentValue);
        parameter1.Add("Source", "Skill.ShootCommandDerive.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
    }

    /// <summary>
    /// �ж��Ƿ��Ǵ˿��������ǲ��Ǵ˼��ܡ���ԴΪ�˼��ܵĽ�ս��ֵС�ڴ˼���ֵ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        if (monsterInBattle.gameObject != gameObject || skillName != "shoot_command_derive")
        {
            return false;
        }

        if (gameObject.TryGetComponent(out Ranged ranged))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = ranged.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.ShootCommandDerive.Effect1"))
            {
                currentValue = keyValuePairs["Skill.ShootCommandDerive.Effect1"];
            }

            if (currentValue < GetSkillValue())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// ԭ��û��ħ�����ջ����ħ�������õ��ڼ�����ֵ��ħ��
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect2");
        parameter1.Add("SkillName", "ranged");
        parameter1.Add("SkillValue", GetSkillValue());
        parameter1.Add("Source", "Skill.ShootCommandDerive.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
    }

    /// <summary>
    /// �ж��Ƿ��Ǵ˿��������ǲ���ħ�����ǲ��Ǹջ��ħ��
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        if (monsterInBattle.gameObject != gameObject || skillName != "ranged")
        {
            return false;
        }

        if (!parameterNode.Parent.EffectChild.result.ContainsKey("AddNewSkill"))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ɾ��ħ������Դ����ֻʣ��buff����Դ����ɾ�����Դ˼��ܵ�ħ��
    /// </summary>
    [TriggerEffect(@"^After\.MonsterInBattle\.DeleteSkillSource$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect3");
        parameter1.Add("SkillName", "ranged");
        parameter1.Add("Source", "Skill.ShootCommandDerive.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
    }

    /// <summary>
    /// �ж��Ƿ��Ǵ˿��������ǲ���ħ������Դ�ǲ��Ƿ�buff��ʣ�µ���Դ�ǲ��Ƕ���buff
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];

        if (monsterInBattle.gameObject != gameObject || skillName != "ranged")
        {
            return false;
        }

        List<Dictionary<string, string>> list = Database.cardMonster.Query("SkillSourceBuff", "and source='" + source + "'");

        if (list.Count > 0)
        {
            return false;
        }

        Debug.Log(monsterInBattle.cardName);

        Ranged ranged = gameObject.GetComponent<Ranged>();
        Dictionary<string, int> sourceAndValue = ranged.sourceAndValue;

        foreach (var item in sourceAndValue)
        {
            List<Dictionary<string, string>> list1 = Database.cardMonster.Query("SkillSourceBuff", "and source='" + item.Key + "'");

            if (list1.Count == 0)
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// ɾ���˼���ǰ��ɾ�����Դ˼��ܵ�ħ��
    /// </summary>
    [TriggerEffect(@"^Before\.MonsterInBattle\.DeleteSkillSource$", "Compare4")]
    public IEnumerator Effect4(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string source = (string)parameter["Source"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, int> keyValuePairs = new(sourceAndValue);
        keyValuePairs.Remove(source);
        int newSkillValue = keyValuePairs.Count == 0 ? -1 : keyValuePairs.Values.Max();

        //�Ƴ�����Դ��û����Դ����ɾ���Ի������������棬������Դ�ͱ䶯��ֵ
        if (newSkillValue == -1)
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect4");
            parameter1.Add("SkillName", "ranged");
            parameter1.Add("Source", "Skill.ShootCommandDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
        }
        else
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect4");
            parameter1.Add("SkillName", "ranged");
            parameter1.Add("SkillValue", newSkillValue - GetSkillValue());
            parameter1.Add("Source", "Skill.ShootCommandDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǵ˿��������ǲ��Ǵ˼���
    /// </summary>
    public bool Compare4(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];

        Dictionary<string, int> keyValuePairs = new(sourceAndValue);
        keyValuePairs.Remove(source);
        int newSkillValue = keyValuePairs.Count == 0 ? -1 : keyValuePairs.Values.Max();

        return monsterInBattle.gameObject == gameObject && skillName == "shoot_command_derive" && newSkillValue != GetSkillValue();
    }

    /// <summary>
    /// Զ�̲��ᱻ��������
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^Replace\.Armor\.Effect1$", "Compare5")]
    public IEnumerator Effect5(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�������Զ��
    /// </summary>
    public bool Compare5(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        return skillInBattle is Ranged && skillInBattle.gameObject == gameObject;
    }

    /// <summary>
    /// �Է��غϽ�����ʧȥ�˼���
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^AfterRoundBattle$", "Compare6")]
    public IEnumerator Effect6(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect6");
        parameter2.Add("SkillName", "shoot_command_derive");
        parameter2.Add("Source", "Skill.ShootCommand.Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
    }

    /// <summary>
    /// �ж��Ƿ��ǶԷ��غ�
    /// </summary>
    public bool Compare6(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Enemy)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}