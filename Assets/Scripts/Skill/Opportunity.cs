using System.Collections;
using UnityEngine;

/// <summary>
/// 机会
/// 敌人位于后排发动<近战>后，发动一次<远程>攻击
/// </summary>
public class Opportunity : SkillInBattle
{
    [TriggerEffect(@"^After\.Melee\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        Ranged ranged = gameObject.GetComponent<Ranged>();

        string fullName = "Ranged.Effect1";
        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
        parameterNode1.opportunity = "InRoundBattle";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(ranged, fullName, parameterNode1, ranged.Effect1));
    }

    /// <summary>
    /// 判断敌方在后排，自己在后排、有远程
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

        bool f1 = false;
        bool f2 = false;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            bool isAlly = false;
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == gameObject)
                {
                    isAlly = true;
                }
            }

            if (isAlly && (playerData.monsterGameObjectArray[1] == gameObject || playerData.monsterGameObjectArray[2] == gameObject))
            {
                f1 = true;
            }

            if (!isAlly && (playerData.monsterGameObjectArray[1] == go || playerData.monsterGameObjectArray[2] == go))
            {
                f2 = true;
            }
        }

        return f1 && f2;
    }
}