using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机伤害
/// 随机选择对方一个怪兽作为目标，若技能数值为0，对目标造成0点伤害，否则，随机造成1到技能数值的伤害
/// </summary>
public class Chance : SkillInBattle
{
    [TriggerEffect("^InRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //对方玩家
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        //选取技能目标
        //List<GameObject> nontargetList = new();
        List<GameObject> priorTargetList = new();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        //parameter1.Add("NontargetList", nontargetList);
        parameter1.Add("PriorTargetList", priorTargetList);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.SelectEffectTarget, parameterNode1));
        yield return null;

        GameObject effectTarget = null;
        if (priorTargetList.Count > 0)
        {
            effectTarget = priorTargetList[0];
        }
        else
        {
            for (int i = 2; i > -1; i--)
            {
                if (oppositePlayerMessage.monsterGameObjectArray[i] != null)
                {
                    effectTarget = oppositePlayerMessage.monsterGameObjectArray[RandomUtils.GetRandomNumber(0, i)];
                    goto endOfTarget;
                }
            }
        endOfTarget:;
        }

        int skillValue = GetSkillValue();

        if (skillValue > 0)
        {
            skillValue = RandomUtils.GetRandomNumber(1, skillValue);
        }

        //伤害参数
        Dictionary<string, object> damageParameter = new();
        //当前技能
        damageParameter.Add("LaunchedSkill", this);
        //效果名称
        damageParameter.Add("EffectName", "Effect1");
        //受到伤害的怪兽
        damageParameter.Add("EffectTarget", effectTarget);
        //伤害数值
        damageParameter.Add("DamageValue", skillValue);
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Real);

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode2));
        yield return null;
    }

    /// <summary>
    /// 判断是否是己方回合，对方场上有怪兽
    /// </summary>
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