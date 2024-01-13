using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ħ������
/// <ħ��>�������е���ʱ���������ڵ������%d�㷨���˺�
/// </summary>
public class MagicAoe : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == monsterBeHurt)
                {
                    if (j == 2 || j == 1)
                    {
                        Dictionary<string, object> damageParameter = new();
                        damageParameter.Add("LaunchedSkill", this);
                        damageParameter.Add("EffectName", "Effect1");
                        damageParameter.Add("EffectTarget", battleProcess.systemPlayerData[i].monsterGameObjectArray[0]);
                        damageParameter.Add("DamageValue", GetSkillValue());
                        damageParameter.Add("DamageType", DamageType.Magic);

                        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                        parameterNode1.parameter = damageParameter;

                        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
                        //yield return null;
                    }
                    else
                    {
                        if (battleProcess.systemPlayerData[i].monsterGameObjectArray[2] != null)
                        {
                            Dictionary<string, object> damageParameter = new();
                            damageParameter.Add("LaunchedSkill", this);
                            damageParameter.Add("EffectName", "Effect1");
                            damageParameter.Add("EffectTarget", battleProcess.systemPlayerData[i].monsterGameObjectArray[2]);
                            damageParameter.Add("DamageValue", GetSkillValue());
                            damageParameter.Add("DamageType", DamageType.Magic);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = damageParameter;

                            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
                            //yield return null;
                        }

                        if (battleProcess.systemPlayerData[i].monsterGameObjectArray[1] != null)
                        {
                            Dictionary<string, object> damageParameter = new();
                            damageParameter.Add("LaunchedSkill", this);
                            damageParameter.Add("EffectName", "Effect1");
                            damageParameter.Add("EffectTarget", battleProcess.systemPlayerData[i].monsterGameObjectArray[1]);
                            damageParameter.Add("DamageValue", GetSkillValue());
                            damageParameter.Add("DamageType", DamageType.Magic);

                            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                            parameterNode1.parameter = damageParameter;

                            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
                            //yield return null;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�����˺��ļ�����ħ��
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            bool isEnemy = true;
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    isEnemy = false;
                }
            }

            if (isEnemy && systemPlayerData.monsterGameObjectArray[1] == null)
            {
                return false;
            }
        }

        return skillInBattle is Magic && skillInBattle.gameObject == gameObject;
    }
}
