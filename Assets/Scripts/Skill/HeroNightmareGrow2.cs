using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultEntangle = Entangle;

/// <summary>
/// ج������
/// <����>������������25%%��
/// �������޻�������󣬸�Ϊǿ������
/// ���ã��������ּܹ��Ժ�����õ�
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
    /// �жϹ����Ǽ������ޣ������ǡ�������
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
    /// ǿ������
    /// </summary>
    public class Entangle : DefaultEntangle
    {
        /// <summary>
        /// �ж��Ƿ��Ǽ����غϣ��Է������й���
        /// </summary>
        public override bool Compare1(ParameterNode parameterNode)
        {
            Debug.Log("���롾ǿ���������ж�");

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
