using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ⱥ����Ϯ
/// ʹ��ʱ�������е������%d����ʵ�˺�
/// </summary>
/// 
public class DamageAll : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$",  "Compare1")]
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
                        //�˺�����
                        Dictionary<string, object> damageParameter = new();
                        //��ǰ����
                        damageParameter.Add("LaunchedSkill", this);
                        //Ч������
                        damageParameter.Add("EffectName", "Effect1");
                        //�ܵ��˺��Ĺ���
                        damageParameter.Add("EffectTarget", systemPlayerData.monsterGameObjectArray[j]);
                        //�˺���ֵ
                        damageParameter.Add("DamageValue", GetSkillValue());
                        //�˺�����
                        damageParameter.Add("DamageType", DamageType.Real);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = damageParameter;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
                        //yield return null;
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
        //ʹ�����Ƶ����
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //����Ʒ����
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                //Debug.Log("Ⱥ����Ϯ1");
                return false;
            }
        }
        //����
        else if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                //Debug.Log("Ⱥ����Ⱦ�ж�2");
                return false;
            }
        }
        //װ��
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                //Debug.Log("Ⱥ����Ⱦ�ж�3");
                return false;
            }
        }
        else
        {
            //Debug.Log("Ⱥ����Ϯ2");
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            if (systemPlayerData.perspectivePlayer != player && systemPlayerData.monsterGameObjectArray[0] == null)
            {
                //Debug.Log("Ⱥ����Ϯ3");
                return false;
            }
        }

        return true;
    }
}