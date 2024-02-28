using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ��ĩ֮ż
/// <��ս><Զ��><ħ��><����˺�>+(5-X)��XΪ�ҷ������ƿ�ʣ�������������ֵ��Ч
/// </summary>
public class FinalDoll : SkillInBattle
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

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        int validValue = GetSkillValue() - cardAmount;

        foreach (var basicAttackEffect in SkillUtils.basicAttackEffectSet)
        {
            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + basicAttackEffect + "'")[0];
            var skillClassName = skillConfig["SkillClassName"];

            object[] parameters = { null };

            var mi = typeof(GameObject).GetMethods().Where(method => method.Name == "TryGetComponent");
            MethodInfo methodInfo = null;
            foreach (var item in mi)
            {
                if (item.IsGenericMethod)
                {
                    methodInfo = item.MakeGenericMethod(Type.GetType(skillClassName));
                    break;
                }
            }

            bool hasSkill = (bool)methodInfo.Invoke(gameObject, parameters);
            if (hasSkill)
            {
                SkillInBattle skillInBattle = (SkillInBattle)parameters[0];

                int currentValue = 0;

                Dictionary<string, int> keyValuePairs = skillInBattle.sourceAndValue;
                if (keyValuePairs.ContainsKey("Skill.FinalDoll.Effect1"))
                {
                    currentValue = keyValuePairs["Skill.FinalDoll.Effect1"];
                }

                int c = validValue - currentValue;

                if (c > 0)
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("LaunchedSkill", this);
                    parameter1.Add("EffectName", "Effect1");
                    parameter1.Add("SkillName", basicAttackEffect);
                    parameter1.Add("SkillValue", c);
                    parameter1.Add("Source", "Skill.FinalDoll.Effect1");

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                }
            }
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

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (monsterInBattle.gameObject != gameObject || skillName != "final_doll")
        {
            return false;
        }

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        if (cardAmount >= GetSkillValue())
        {
            return false;
        }

        int validValue = GetSkillValue() - cardAmount;

        foreach (var basicAttackEffect in SkillUtils.basicAttackEffectSet)
        {
            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + basicAttackEffect + "'")[0];
            var skillClassName = skillConfig["SkillClassName"];

            object[] parameters = { null };

            var mi = typeof(GameObject).GetMethods().Where(method => method.Name == "TryGetComponent");
            MethodInfo methodInfo = null;
            foreach (var item in mi)
            {
                if (item.IsGenericMethod)
                {
                    methodInfo = item.MakeGenericMethod(Type.GetType(skillClassName));
                    break;
                }
            }

            bool hasSkill = (bool)methodInfo.Invoke(gameObject, parameters);
            if (hasSkill)
            {
                SkillInBattle skillInBattle = (SkillInBattle)parameters[0];

                int currentValue = 0;

                Dictionary<string, int> keyValuePairs = skillInBattle.sourceAndValue;
                if (keyValuePairs.ContainsKey("Skill.FinalDoll.Effect1"))
                {
                    currentValue = keyValuePairs["Skill.FinalDoll.Effect1"];
                }

                if (currentValue < validValue)
                {
                    //Debug.Log("��ô˼���ʱ������ԴΪ-----------------66666666666666666");
                    return true;
                }
            }
        }

        //Debug.Log("��ô˼���ʱ������ԴΪ-----------------3");
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

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        int validValue = GetSkillValue() - cardAmount;

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect2");
        parameter1.Add("SkillName", skillName);
        parameter1.Add("SkillValue", validValue);
        parameter1.Add("Source", "Skill.FinalDoll.Effect1");

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

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (monsterInBattle.gameObject != gameObject || !SkillUtils.basicAttackEffectSet.Contains(skillName))
        {
            return false;
        }

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        if (cardAmount >= GetSkillValue())
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
        parameter1.Add("Source", "Skill.FinalDoll.Effect1");

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

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (monsterInBattle.gameObject != gameObject || !SkillUtils.basicAttackEffectSet.Contains(skillName))
        {
            return false;
        }

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    cardAmount = systemPlayerData.monsterDeck.Count;
                    break;
                }
            }
        }

        if (cardAmount >= GetSkillValue())
        {
            return false;
        }

        List<Dictionary<string, string>> list = Database.cardMonster.Query("SkillSourceBuff", "and source='" + source + "'");

        if (list.Count > 0)
        {
            return false;
        }

        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
        var skillClassName = skillConfig["SkillClassName"];

        SkillInBattle skillInBattle = (SkillInBattle)gameObject.GetComponent(Type.GetType(skillClassName));
        Dictionary<string, int> sourceAndValue = skillInBattle.sourceAndValue;

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

        foreach (var basicAttackEffect in SkillUtils.basicAttackEffectSet)
        {
            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + basicAttackEffect + "'")[0];
            var skillClassName = skillConfig["SkillClassName"];

            object[] parameters = { null };

            var mi = typeof(GameObject).GetMethods().Where(method => method.Name == "TryGetComponent");
            MethodInfo methodInfo = null;
            foreach (var item in mi)
            {
                if (item.IsGenericMethod)
                {
                    methodInfo = item.MakeGenericMethod(Type.GetType(skillClassName));
                    break;
                }
            }

            bool hasSkill = (bool)methodInfo.Invoke(gameObject, parameters);
            if (hasSkill)
            {
                //�Ƴ�����Դ��û����Դ����ɾ���Ի������������棬������Դ�ͱ䶯��ֵ
                if (newSkillValue == -1)
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("LaunchedSkill", this);
                    parameter1.Add("EffectName", "Effect4");
                    parameter1.Add("SkillName", basicAttackEffect);
                    parameter1.Add("Source", "Skill.FinalDoll.Effect1");

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
                }
                else
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("LaunchedSkill", this);
                    parameter1.Add("EffectName", "Effect4");
                    parameter1.Add("SkillName", basicAttackEffect);
                    parameter1.Add("SkillValue", newSkillValue - GetSkillValue());
                    parameter1.Add("Source", "Skill.FinalDoll.Effect1");

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                }
            }
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

        return monsterInBattle.gameObject == gameObject && skillName == "final_doll" && newSkillValue != GetSkillValue();
    }

    /// <summary>
    /// ���뿪�ƿ⣬����+1
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^After\.GameAction\.CardLeaveLibrary$", "Compare5")]
    public IEnumerator Effect5(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        foreach (var basicAttackEffect in SkillUtils.basicAttackEffectSet)
        {
            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + basicAttackEffect + "'")[0];
            var skillClassName = skillConfig["SkillClassName"];

            object[] parameters = { null };

            var mi = typeof(GameObject).GetMethods().Where(method => method.Name == "TryGetComponent");
            MethodInfo methodInfo = null;
            foreach (var item in mi)
            {
                if (item.IsGenericMethod)
                {
                    methodInfo = item.MakeGenericMethod(Type.GetType(skillClassName));
                    break;
                }
            }

            bool hasSkill = (bool)methodInfo.Invoke(gameObject, parameters);

            if (hasSkill)
            {
                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect5");
                parameter2.Add("SkillName", basicAttackEffect);
                parameter2.Add("SkillValue", 1);
                parameter2.Add("Source", "Skill.FinalDoll.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;
                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
            }
        }
    }

    /// <summary>
    /// ���޿��뿪�ƿ�
    /// </summary>
    public bool Compare5(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string type = (string)parameter["Type"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (type != "monster")
        {
            return false;
        }

        int cardAmount = 0;
        bool isAlly = false;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        isAlly = true;
                        cardAmount = systemPlayerData.monsterDeck.Count;
                        break;
                    }
                }
            }
        }

        if (!isAlly)
        {
            return false;
        }

        if (cardAmount >= GetSkillValue())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// �ƽ����ƿ⣬����ƿ��й�������С�ڼ���ֵ������-1��������ڣ����Ƴ����Դ˼��ܵ���Դ
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^After\.GameAction\.AddCardToDeck$", "Compare6")]
    public IEnumerator Effect6(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        int cardAmount = 0;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        cardAmount = systemPlayerData.monsterDeck.Count;
                        break;
                    }
                }
            }
        }

        int c = GetSkillValue() - cardAmount;

        foreach (var basicAttackEffect in SkillUtils.basicAttackEffectSet)
        {
            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + basicAttackEffect + "'")[0];
            var skillClassName = skillConfig["SkillClassName"];

            object[] parameters = { null };

            var mi = typeof(GameObject).GetMethods().Where(method => method.Name == "TryGetComponent");
            MethodInfo methodInfo = null;
            foreach (var item in mi)
            {
                if (item.IsGenericMethod)
                {
                    methodInfo = item.MakeGenericMethod(Type.GetType(skillClassName));
                    break;
                }
            }

            bool hasSkill = (bool)methodInfo.Invoke(gameObject, parameters);
            if (hasSkill)
            {
                if (c > 0)
                {
                    Dictionary<string, object> parameter2 = new();
                    parameter2.Add("LaunchedSkill", this);
                    parameter2.Add("EffectName", "Effect6");
                    parameter2.Add("SkillName", basicAttackEffect);
                    parameter2.Add("SkillValue", -1);
                    parameter2.Add("Source", "Skill.FinalDoll.Effect1");

                    ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    parameterNode2.parameter = parameter2;
                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
                }
                else
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("LaunchedSkill", this);
                    parameter1.Add("EffectName", "Effect6");
                    parameter1.Add("SkillName", basicAttackEffect);
                    parameter1.Add("Source", "Skill.FinalDoll.Effect1");

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));
                }
            }
        }
    }

    /// <summary>
    /// �ƽ����ƿ�
    /// </summary>
    public bool Compare6(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        string type = cardData["CardType"];

        if (type != "monster")
        {
            return false;
        }

        int cardAmount = 0;
        bool isAlly = false;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        isAlly = true;
                        cardAmount = systemPlayerData.monsterDeck.Count;
                        break;
                    }
                }
            }
        }

        if (!isAlly)
        {
            return false;
        }

        if (cardAmount > GetSkillValue())
        {
            return false;
        }

        return true;
    }
}