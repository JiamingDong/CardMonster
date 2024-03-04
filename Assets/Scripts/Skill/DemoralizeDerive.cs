using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ���ţ�������
/// �غϽ���ʱ����������š�Ч��
/// </summary>
public class DemoralizeDerive : SkillInBattle
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
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        if (gameObject.TryGetComponent(out Melee melee))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = melee.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.DemoralizeDerive.Effect1"))
            {
                currentValue = -keyValuePairs["Skill.DemoralizeDerive.Effect1"];
            }

            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect1");
            parameter1.Add("SkillName", "melee");
            parameter1.Add("SkillValue", currentValue - GetSkillValue());
            parameter1.Add("Source", "Skill.DemoralizeDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        if (gameObject.TryGetComponent(out Ranged ranged))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = ranged.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.DemoralizeDerive.Effect1"))
            {
                currentValue = -keyValuePairs["Skill.DemoralizeDerive.Effect1"];
            }

            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect1");
            parameter1.Add("SkillName", "ranged");
            parameter1.Add("SkillValue", currentValue - GetSkillValue());
            parameter1.Add("Source", "Skill.DemoralizeDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǵ˿��������ǲ��Ǵ˼��ܡ���ԴΪ�˼��ܵĽ�ս��ֵС�ڴ˼���ֵ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        if (monsterInBattle.gameObject != gameObject || skillName != "demoralize_derive")
        {
            return false;
        }

        if (gameObject.TryGetComponent(out Melee melee))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = melee.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.DemoralizeDerive.Effect1"))
            {
                currentValue = -keyValuePairs["Skill.DemoralizeDerive.Effect1"];
            }

            if (currentValue < GetSkillValue())
            {
                return true;
            }
        }

        if (gameObject.TryGetComponent(out Ranged ranged))
        {
            int currentValue = 0;

            Dictionary<string, int> keyValuePairs = ranged.sourceAndValue;
            if (keyValuePairs.ContainsKey("Skill.DemoralizeDerive.Effect1"))
            {
                currentValue = -keyValuePairs["Skill.DemoralizeDerive.Effect1"];
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
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect2");
        parameter1.Add("SkillName", skillName);
        parameter1.Add("SkillValue", -GetSkillValue());
        parameter1.Add("Source", "Skill.DemoralizeDerive.Effect1");

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

        if (monsterInBattle.gameObject != gameObject)
        {
            return false;
        }

        if (skillName != "melee" && skillName != "ranged")
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
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect3");
        parameter1.Add("SkillName", skillName);
        parameter1.Add("Source", "Skill.DemoralizeDerive.Effect1");

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

        if (monsterInBattle.gameObject != gameObject)
        {
            return false;
        }

        if (skillName != "melee" && skillName != "ranged")
        {
            return false;
        }

        List<Dictionary<string, string>> list = Database.cardMonster.Query("SkillSourceBuff", "and source='" + source + "'");

        if (list.Count > 0)
        {
            return false;
        }

        if (skillName == "melee" && gameObject.TryGetComponent(out Melee melee))
        {
            Dictionary<string, int> sourceAndValue = melee.sourceAndValue;

            foreach (var item in sourceAndValue)
            {
                List<Dictionary<string, string>> list1 = Database.cardMonster.Query("SkillSourceBuff", "and source='" + item.Key + "'");

                if (list1.Count == 0)
                {
                    return false;
                }
            }
        }

        if (skillName == "ranged" && gameObject.TryGetComponent(out Ranged ranged))
        {
            Dictionary<string, int> sourceAndValue = ranged.sourceAndValue;

            foreach (var item in sourceAndValue)
            {
                List<Dictionary<string, string>> list1 = Database.cardMonster.Query("SkillSourceBuff", "and source='" + item.Key + "'");

                if (list1.Count == 0)
                {
                    return false;
                }
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
            parameter1.Add("SkillName", "melee");
            parameter1.Add("Source", "Skill.DemoralizeDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", this);
            parameter2.Add("EffectName", "Effect4");
            parameter2.Add("SkillName", "ranged");
            parameter2.Add("Source", "Skill.DemoralizeDerive.Effect1");

            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
            parameterNode2.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
        }
        else
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect4");
            parameter1.Add("SkillName", "melee");
            parameter1.Add("SkillValue", GetSkillValue() - newSkillValue);
            parameter1.Add("Source", "Skill.DemoralizeDerive.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", this);
            parameter2.Add("EffectName", "Effect4");
            parameter2.Add("SkillName", "ranged");
            parameter2.Add("SkillValue", GetSkillValue() - newSkillValue);
            parameter2.Add("Source", "Skill.DemoralizeDerive.Effect1");

            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
            parameterNode2.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
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

        return monsterInBattle.gameObject == gameObject && skillName == "demoralize_derive" && newSkillValue != GetSkillValue();
    }

    /// <summary>
    /// �Է��غϽ���ʱ������˼���
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
        parameter2.Add("SkillName", "demoralize_derive");
        parameter2.Add("Source", "Skill.Demoralize.Effect1");

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
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}