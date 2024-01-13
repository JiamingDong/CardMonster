using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// ����<����><����><����><���淴��><����>
/// </summary>
public class Cautious : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Counter\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.CrossCounter\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Thorns\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Opportunity\.Effect1$", "Compare2")]
    [TriggerEffect(@"^Replace\.Entangle\.Effect1$", "Compare3")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
    }

    /// <summary>
    /// �жϴ���<����><����><���淴��>�ļ������Ա�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        foreach (var item in parameter)
        {
            Debug.Log(item.Key + "----" + item.Value);
        }
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        return skillInBattle.gameObject == gameObject;
    }

    /// <summary>
    /// �жϴ���<����>�ļ������Ա�����
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        var creator = parameterNode.Parent.creator;
        Debug.Log("�����ж�");
        Debug.Log(creator.GetType());

        return creator is Melee melee && melee.gameObject == gameObject;
    }

    /// <summary>
    /// �ж�<����>��������Ǳ�����
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        object creator = parameterNode.creator;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (creator is Entangle entangle)
        {
            GameObject go = entangle.gameObject;

            int thisPosition = -1;
            int goPosition = -1;
            bool f = false;
            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

                bool isAlly = true;
                bool isEnemy = true;
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        thisPosition = j;
                        isEnemy = false;
                    }

                    if (systemPlayerData.monsterGameObjectArray[j] == go)
                    {
                        goPosition = j;
                        isAlly = false;
                    }
                }

                if (isAlly && !isEnemy)
                {
                    f = true;
                }
            }

            if (f && thisPosition == goPosition)
            {
                return true;
            }
        }

        return false;
    }
}
