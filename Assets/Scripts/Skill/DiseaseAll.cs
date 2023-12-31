using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ⱥ����Ⱦ
/// ʹ��ʱ�����е��˻��%d�㡰������Ч����������ʿ���������ָ�ӡ���ǿ�ˡ������š���ħ�����䡱ʧЧ���ɱ������ơ���Ⱥ�����ơ������
/// </summary>
public class DiseaseAll : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player)
            {
                for (int j = 2; j >= 0; j--)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] != null)
                    {
                        MonsterInBattle monsterInBattle = systemPlayerData.monsterGameObjectArray[j].GetComponent<MonsterInBattle>();

                        Dictionary<string, object> parameter1 = new();
                        parameter1.Add("LaunchedSkill", this);
                        parameter1.Add("EffectName", "Effect1");
                        parameter1.Add("SkillName", "disease_derive");
                        parameter1.Add("SkillValue", GetSkillValue());
                        parameter1.Add("Source", "Skill.DiseaseAll.Effect1");

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = parameter1;

                        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
                    }
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ�ʹ�õ��Ǵ˿����Է���û�й���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //����Ʒ����
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        //����
        else if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        //װ��
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                return false;
            }
        }

        return true;
    }
}
