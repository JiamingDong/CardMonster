using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ͨ��������
/// ���غ��У��ҷ�ʹ�õ���һ������Ʒ���<��ļ%s>�����һ�����ͬ��λ�ö�����Ч1�Ρ�
/// </summary>
public class KnowEverythingDerive : SkillInBattle
{
    /// <summary>
    /// ��ļ����
    /// </summary>
    int launchMark1 = 0;
    /// <summary>
    /// ������Ч����
    /// </summary>
    int launchMark2 = 0;
    /// <summary>
    /// ��¼��������
    /// </summary>
    ParameterNode parameterNodeRecord;

    public override int AddValue(string source, int value)
    {
        launchMark1 += value;
        launchMark2++;

        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] += value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    [TriggerEffect(@"^Before\.GameAction\.ConsumeEnterBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, string> cardData = (Dictionary<string, string>)parameter["CardData"];

        string cardSkill = cardData["CardSkill"];
        Dictionary<string, int> dic = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardSkill);

        if (dic.ContainsKey("draft"))
        {
            dic["draft"] += launchMark1;
        }
        else
        {
            dic.Add("draft", launchMark1);
        }

        cardData["CardSkill"] = JsonConvert.SerializeObject(dic);

        parameterNodeRecord = parameterNode;

        launchMark1 = 0;

        yield break;
    }

    /// <summary>
    /// �ж��Ǽ���������Ʒ����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchMark1 < 1)
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.skillList.Count; j++)
                {
                    SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                    if (skillInBattle == this)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [TriggerEffect(@"^Before\.GameAction\.ConsumeLeave$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        int l = launchMark2;
        launchMark2 = 0;

        for (int j = 0; j < l; j++)
        {
            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                //ѯ������Ʒ
                GameObject consumeGameObject = battleProcess.systemPlayerData[i].consumeGameObject;
                if (consumeGameObject != null)
                {
                    //������������Ʒ�ϵļ���
                    //Debug.Log("������������Ʒ�ϵļ���");
                    ConsumeInBattle consumeInBattle = consumeGameObject.GetComponent<ConsumeInBattle>();
                    foreach (var item in consumeInBattle.skillList)
                    {
                        Destroy(item);
                    }
                    consumeInBattle.skillList = new();

                    yield return consumeInBattle.Generate(consumeInBattle.cardData);

                    //�����趨Ŀ��
                    ParameterNode parameterNode2 = parameterNodeRecord.Parent.Parent.superiorNode.Parent.AfterChild;

                    int battlePanelNumber = (int)parameterNode2.Parent.parameter["BattlePanelNumber"];
                    parameterNode2.Parent.EffectChild.nodeInMethodList[1].EffectChild.result["ConsumeTarget"] = battleProcess.systemPlayerData[(i + 1) % 2].monsterGameObjectArray[battlePanelNumber];

                    yield return battleProcess.StartCoroutine(consumeInBattle.LaunchSkill(parameterNode2));
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ǽ���������Ʒ�볡
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchMark2 < 1)
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.skillList.Count; j++)
                {
                    SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                    if (skillInBattle == this)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [TriggerEffect("^AfterRoundBattle$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.skillList.Count; j++)
            {
                SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                if (skillInBattle == this)
                {
                    systemPlayerData.skillList.Remove(skillInBattle);
                    Destroy(this);
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غ�
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.skillList.Count; j++)
            {
                SkillInBattle skillInBattle = systemPlayerData.skillList[j];

                if (skillInBattle == this)
                {
                    return true;
                }
            }
        }

        return false;
    }
}