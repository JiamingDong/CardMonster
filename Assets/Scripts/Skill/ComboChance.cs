using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 散射
/// 此卡进行“随机伤害”攻击时，可以额外多进行%d次。
/// </summary>
public class ComboChance : SkillInBattle
{
    [TriggerEffect(@"^After\.Chance\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "Chance.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            if (gameObject.TryGetComponent<Chance>(out var chance))
            {
                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
                parameterNode1.opportunity = "InRoundBattle";
                parameterNode1.result.Add("isAdditionalExecute", true);

                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(chance, fullName, parameterNode1, chance.Effect1));
            }
        }
    }

    /// <summary>
    /// 判断是否是本怪兽，对方有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;

        if (result.ContainsKey("isAdditionalExecute"))
        {
            //Debug.Log("散射9999999999999999999");
            return false;
        }

        BattleProcess battleProcess = BattleProcess.GetInstance();

        GameObject go = skillInBattle.gameObject;
        if (go != gameObject)
        {
            return false;
        }

        if (!gameObject.TryGetComponent<Chance>(out _))
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
