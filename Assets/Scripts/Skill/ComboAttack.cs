using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 乱舞
/// 额外发动%d次基础攻击（基础攻击，<近战><远程><魔法><随机伤害>）
/// </summary>
public class ComboAttack : SkillInBattle
{
    [TriggerEffect(@"^After\.Magic\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "Magic.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            if (gameObject.TryGetComponent<Magic>(out var magic))
            {
                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
                parameterNode1.opportunity = "InRoundBattle";
                parameterNode1.result.Add("isAdditionalExecute", true);

                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(magic, fullName, parameterNode1, magic.Effect1));
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

    [TriggerEffect(@"^After\.Chance\.Effect1$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "Chance.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            if (gameObject.TryGetComponent<Chance>(out var chance))
            {
                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode.SetParent(new(), ParameterNodeChildType.EffectChild);
                parameterNode1.opportunity = "InRoundBattle";
                parameterNode1.result.Add("isAdditionalExecute", true);

                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(chance, fullName, parameterNode1, chance.Effect1));
            }
        }
    }

    /// <summary>
    /// 判断是否是本怪兽，对方有怪兽
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
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

    [TriggerEffect(@"^After\.Ranged\.Effect1$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "Ranged.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            if (gameObject.TryGetComponent<Ranged>(out var ranged))
            {
                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
                parameterNode1.opportunity = "InRoundBattle";
                parameterNode1.result.Add("isAdditionalExecute", true);

                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(ranged, fullName, parameterNode1, ranged.Effect1));
            }
        }
    }

    /// <summary>
    /// 判断是否是本怪兽，对方有怪兽
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
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

        if (!gameObject.TryGetComponent<Ranged>(out _))
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

    [TriggerEffect(@"^After\.Melee\.Effect1$", "Compare4")]
    public IEnumerator Effect4(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "Melee.Effect1";
        for (int i = 0; i < GetSkillValue(); i++)
        {
            if (gameObject.TryGetComponent<Melee>(out var melee))
            {
                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
                parameterNode1.opportunity = "InRoundBattle";
                parameterNode1.result.Add("isAdditionalExecute", true);

                yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(melee, fullName, parameterNode1, melee.Effect1));
            }
        }
    }

    /// <summary>
    /// 判断是否是本怪兽，对方有怪兽
    /// </summary>
    public bool Compare4(ParameterNode parameterNode)
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

        if (!gameObject.TryGetComponent<Melee>(out _))
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
