using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弹幕
/// 与此卡相同阵营的怪兽，“随机攻击”额外攻击X次。
/// </summary>
public class Barrage : SkillInBattle
{
    [TriggerEffect(@"^After\.Chance\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.Parent.creator;
        GameObject go = skillInBattle.gameObject;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "Chance.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            ParameterNode parameterNode1 = new();
            parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
            parameterNode1.opportunity = "InRoundBattle";
            parameterNode1.result.Add("isAdditionalExecute", true);

            if (go != null && go.TryGetComponent<Chance>(out var chance) && chance.CompareCondition(chance.Effect1, parameterNode1))
            {
                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(chance, fullName, parameterNode1, chance.Effect1));
                yield return null;
            }
        }
    }

    /// <summary>
    /// 判断是否是己方怪兽并且阵营相同
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;

        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        GameObject go = skillInBattle.gameObject;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (result.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        Player? thisPlayer = null;
        Player? targetPlayer = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            bool isEnemy = true;
            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == gameObject)
                {
                    thisPlayer = playerData.perspectivePlayer;
                    isEnemy = false;
                }
                if (playerData.monsterGameObjectArray[j] == go)
                {
                    targetPlayer = playerData.perspectivePlayer;
                }
            }

            if (isEnemy && playerData.monsterGameObjectArray[0] == null)
            {
                return false;
            }
        }

        if (targetPlayer != thisPlayer)
        {
            return false;
        }

        MonsterInBattle thisMonster = gameObject.GetComponent<MonsterInBattle>();
        MonsterInBattle targetMonster = go.GetComponent<MonsterInBattle>();

        //Debug.Log(thisMonster.kind);
        //Debug.Log(targetMonster.kind);
        if (!thisMonster.kind.Equals(targetMonster.kind))
        {
            return false;
        }

        return true;
    }
}
