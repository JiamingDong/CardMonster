using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����еļ���
/// </summary>
public class SkillNotOnMonster : OpportunityEffect
{
    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static SkillNotOnMonster instance;

    /// <summary>
    /// ��ñ���ʵ��
    /// </summary>
    /// <returns></returns>
    public static SkillNotOnMonster GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Find("BattleProcessSystem").GetComponent<SkillNotOnMonster>();
        }
        return instance;
    }

    /// <summary>
    /// ���������еġ�����ˮ����
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect(@"^Before\.GameAction\.ChangeCrystalAmount$", "Compare1")]
    public IEnumerator Crystal(ParameterNode parameterNode)
    {
        Debug.Log("���������еġ�����ˮ����");

        Dictionary<string, object> parameter = parameterNode.parameter;
        int crystalAmount = (int)parameter["CrystalAmount"];

        Dictionary<string, object> result = parameterNode.Parent.Parent.superiorNode.result;
        int crystalSkillValue = (int)result["CrystalSkillValue"];

        //Debug.Log("crystalSkillValue=" + crystalSkillValue);

        parameter["CrystalAmount"] = crystalAmount + crystalSkillValue;

        yield break;
    }

    /// <summary>
    /// �жϱ��׼�������ӵ�С�����ˮ����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        if (parameter.ContainsKey("LaunchedSkill") && parameter.ContainsKey("EffectName"))
        {
            object launchedSkill = parameter["LaunchedSkill"];
            string effectName = (string)parameter["EffectName"];

            if (launchedSkill is GameAction && effectName == "Sacrifice")
            {
                Dictionary<string, object> result = parameterNode.Parent.superiorNode.result;

                if (result.ContainsKey("CrystalSkillValue"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// ���������еġ��޷�ʹ�á�
    /// </summary>
    [TriggerEffect(@"^Replace\.RuleEvent\.CardBeUsed$", "Compare2")]
    public IEnumerator ForbidUse(ParameterNode parameterNode)
    {
        Debug.Log("���������еġ��޷�ʹ�á�");

        Dictionary<string, object> result = parameterNode.Parent.result;

        result.Add("BeReplaced", true);

        yield break;
    }

    /// <summary>
    /// �ж��Ƿ������ƣ���ӵ�С��޷�ʹ�á�
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        Dictionary<string, object> parameter = parameterNode.parameter;
        int handPanelNumber = (int)parameter["HandPanelNumber"];
        Player player = (Player)parameter["Player"];

        if (handPanelNumber >= 0 && handPanelNumber <= 3)
        {
            foreach (PlayerData playerData in battleProcess.systemPlayerData)
            {
                if (playerData.perspectivePlayer == player)
                {
                    Dictionary<string, string> handCard = null;
                    switch (handPanelNumber)
                    {
                        case 0:
                            handCard = playerData.handMonster[0];
                            break;
                        case 1:
                            handCard = playerData.handMonster[1];
                            break;
                        case 2:
                            handCard = playerData.handItem[0];
                            break;
                        case 3:
                            handCard = playerData.handItem[1];
                            break;
                    }

                    if (handCard != null)
                    {
                        string cardP = handCard["CardSkill"];
                        if (!string.IsNullOrEmpty(cardP))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("forbid_use"))
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// ���������еġ��޷��׼���
    /// </summary>
    [TriggerEffect(@"^Replace\.RuleEvent\.CardBeSacrificed$", "Compare3")]
    public IEnumerator ForbidSacrifice(ParameterNode parameterNode)
    {
        Debug.Log("���������еġ��޷��׼���");

        Dictionary<string, object> result = parameterNode.Parent.result;

        result.Add("BeReplaced", true);

        yield break;
    }

    /// <summary>
    /// �ж��Ƿ������ƣ���ӵ�С��޷��׼���
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        if (objectBeSacrificedNumber >= 0 && objectBeSacrificedNumber <= 3)
        {
            foreach (PlayerData playerData in battleProcess.systemPlayerData)
            {
                if (playerData.perspectivePlayer == player)
                {
                    Dictionary<string, string> handCard = null;
                    switch (objectBeSacrificedNumber)
                    {
                        case 0:
                            handCard = playerData.handMonster[0];
                            break;
                        case 1:
                            handCard = playerData.handMonster[1];
                            break;
                        case 2:
                            handCard = playerData.handItem[0];
                            break;
                        case 3:
                            handCard = playerData.handItem[1];
                            break;
                    }

                    if (handCard != null)
                    {
                        string cardP = handCard["CardSkill"];
                        if (!string.IsNullOrEmpty(cardP))
                        {
                            Dictionary<string, object> pd = JsonConvert.DeserializeObject<Dictionary<string, object>>(cardP);
                            if (pd.ContainsKey("forbid_sacrifice"))
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        return false;
    }
}
