using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法
/// 在回合战斗中，若所在怪兽位置为1号位，对对方1个怪兽造成等于技能数值的物理伤害，优先级为1、2、3，
/// 若为2号位，优先级为2、1、3，若为3号位，优先级为3、1、2。
/// </summary>
public class Magic : SkillInBattle
{
    public Magic(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
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

        //选取技能目标
        int[][] skillTargetPriority = new int[][] { new int[] { 0, 1, 2 }, new int[] { 1, 0, 2 }, new int[] { 2, 0, 1 } };
        GameObject effectTarget = null;
        for (int i = 0; i < 3; i++)
        {
            effectTarget = oppositePlayerMessage.monsterGameObjectArray[skillTargetPriority[position][i]];
            if (effectTarget != null)
            {
                ParameterNode parameterNode1 = new();
                parameterNode1.opportunity = "Magic.Effect1.ChoiceTarget";

                //Dictionary<string, object> targetCondition = new();
                //targetCondition.Add("LaunchedSkill", this);

                Dictionary<string, object> targetParameter = new();
                targetParameter.Add("LaunchedSkill", this);
                targetParameter.Add("SkillTarget", effectTarget);

                parameterNode1.parameter = targetParameter;
                //parameterNode1.condition = targetCondition;

                yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
            }
        }

        yield return null;

        //伤害参数
        Dictionary<string, object> damageParameter = new();
        //当前技能
        damageParameter.Add("LaunchedSkill", this);
        //效果名称
        damageParameter.Add("EffectName", "Effect1");
        //受到伤害的怪兽
        damageParameter.Add("EffectTarget", effectTarget);
        //伤害数值
        damageParameter.Add("DamageValue", GetSKillValue());
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Magic);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

        yield return null;
    }

    /// <summary>
    /// 判断是否是己方回合，对方场上有怪兽
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

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
