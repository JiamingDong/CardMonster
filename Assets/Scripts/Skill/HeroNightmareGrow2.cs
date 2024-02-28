using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultEntangle = Entangle;

/// <summary>
/// 噩梦滋养
/// <束缚>触发概率上升25%%。
/// 己方怪兽获得束缚后，改为强力束缚
/// 弃用，但是这种架构以后可能用到
/// </summary>
public class HeroNightmareGrow2 : SkillInBattle
{
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;

        List<SkillInBattle> skillList = monsterInBattle.skillList;

        for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i] is DefaultEntangle)
            {
                DefaultEntangle defaultEntangle = monsterInBattle.gameObject.GetComponent<DefaultEntangle>();

                HeroNightmareGrowNamespace.Entangle entangle = monsterInBattle.gameObject.AddComponent<HeroNightmareGrowNamespace.Entangle>();

                entangle.sourceAndValue = new(defaultEntangle.sourceAndValue);

                Destroy(defaultEntangle);

                skillList[i] = entangle;

                yield break;
            }
        }
    }

    /// <summary>
    /// 判断怪兽是己方怪兽，技能是“束缚”
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (!skillName.Equals("entangle"))
        {
            return false;
        }

        List<SkillInBattle> skillList = monsterInBattle.skillList;
        bool hasDefaultEntangle = false;
        for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i] is DefaultEntangle)
            {
                hasDefaultEntangle = true;
                break;
            }
        }

        if (!hasDefaultEntangle)
        {
            return false;
        }

        Player? thisPlayer = null;
        Player? targetPlayer = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            if (playerData.heroSkillGameObject == gameObject)
            {
                thisPlayer = playerData.perspectivePlayer;
            }

            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == monsterInBattle)
                {
                    targetPlayer = playerData.perspectivePlayer;
                }
            }
        }

        if (thisPlayer != targetPlayer)
        {
            return false;
        }

        return true;
    }
}

namespace HeroNightmareGrowNamespace
{
    /// <summary>
    /// 强化束缚
    /// </summary>
    public class Entangle : DefaultEntangle
    {
        /// <summary>
        /// 判断是否是己方回合，对方场上有怪兽
        /// </summary>
        public override bool Compare1(ParameterNode parameterNode)
        {
            Debug.Log("进入【强化束缚】判定");

            BattleProcess battleProcess = BattleProcess.GetInstance();

            int r = RandomUtils.GetRandomNumber(1, 4);

            if (r > 3)
            {
                return false;
            }

            bool isAlly = false;
            bool enemyHasMonster = false;

            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

                if (systemPlayerData.perspectivePlayer == Player.Ally)
                {
                    for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                    {
                        if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                        {
                            isAlly = true;
                        }
                    }
                }

                if (systemPlayerData.perspectivePlayer == Player.Enemy && systemPlayerData.monsterGameObjectArray[0] != null)
                {
                    enemyHasMonster = true;
                }
            }

            return isAlly && enemyHasMonster;
        }
    }
}
