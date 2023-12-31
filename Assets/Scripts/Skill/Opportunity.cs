using System.Collections;
using UnityEngine;

/// <summary>
/// ����
/// ����λ�ں��ŷ���<��ս>�󣬷���һ��<Զ��>����
/// </summary>
public class Opportunity : SkillInBattle
{
    [TriggerEffect(@"^After\.Melee\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.Parent.creator;
        GameObject go = skillInBattle.gameObject;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "Ranged.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            ParameterNode parameterNode1 = new();
            parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
            parameterNode1.opportunity = "InRoundBattle";

            if (go != null && go.TryGetComponent<Ranged>(out var ranged) && ranged.CompareCondition(ranged.Effect1, parameterNode1))
            {
                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(ranged, fullName, parameterNode1, ranged.Effect1));
                yield return null;
            }
        }
    }

    /// <summary>
    /// �жϵз��ں��ţ��Լ��ں��š���Զ��
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        GameObject go = skillInBattle.gameObject;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (!gameObject.TryGetComponent<Ranged>(out _))
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if ((playerData.monsterGameObjectArray[1] == gameObject || playerData.monsterGameObjectArray[2] == gameObject) && playerData.monsterGameObjectArray[1] != go && playerData.monsterGameObjectArray[2] != go)
                {
                    return true;
                }
            }
        }

        return false;
    }
}