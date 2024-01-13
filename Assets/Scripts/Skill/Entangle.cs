using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 束缚
/// 我方战斗阶段开始时，50%%几率使正对面敌人获得“束缚”
/// </summary>
public class Entangle : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //所在怪兽的位置
        int position = -1;
        //对方玩家
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    position = j;
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 1, 0, 2 }, new int[] { 2, 0, 1 } };


        GameObject effectTarget = null;
        for (int i = 0; i < 3; i++)
        {
            effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];

            if (effectTarget != null)
            {
                break;
            }
        }

        MonsterInBattle monsterInBattle = effectTarget.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter = new();
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect1");
        parameter.Add("SkillName", "entangle_derive");
        parameter.Add("SkillValue", 0);
        parameter.Add("Source", "Skill.Entangle.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;
        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
    }

    /// <summary>
    /// 判断是否是己方回合，对方场上有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        int r = RandomUtils.GetRandomNumber(1, 2);

        if (r != 1)
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