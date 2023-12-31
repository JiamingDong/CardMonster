using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// �˿����С�ħ��������ʱ�����Զ�������%d�Ρ�
/// </summary>
public class ComboMagic : SkillInBattle
{
    [TriggerEffect(@"^After\.Magic\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (gameObject.TryGetComponent<Magic>(out var magic))
        {
            string fullName = "Magic.Effect1";
            for (int i = 0; i < GetSkillValue(); i++)
            {
                ParameterNode parameterNode1 = new();
                parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
                parameterNode1.opportunity = "InRoundBattle";
                parameterNode1.result.Add("isAdditionalExecute", true);

                if (magic.CompareCondition(magic.Effect1, parameterNode1))
                {
                    yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(magic, fullName, parameterNode1, magic.Effect1));
                    yield return null;
                }
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ��Է��й���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;

        if (result.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        BattleProcess battleProcess = BattleProcess.GetInstance();

        GameObject go = skillInBattle.gameObject;
        if (go != gameObject)
        {
            return false;
        }

        if (!gameObject.TryGetComponent<Magic>(out _))
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            bool isEnemy = true;
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == gameObject)
                {
                    isEnemy = false;
                }
            }

            if (isEnemy)
            {
                if (playerData.monsterGameObjectArray[0] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
