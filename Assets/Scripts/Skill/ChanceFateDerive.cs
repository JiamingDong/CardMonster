using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 强运（衍生）
/// <随机伤害>造成最大值伤害。对方回合结束时，清除【强运】效果
/// </summary>
public class ChanceFateDerive : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Chance\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        result.Add("BeReplaced", true);

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

        Chance chance = gameObject.GetComponent<Chance>();

        IEnumerator Effect(ParameterNode parameterNode)
        {
            //选取技能目标
            List<GameObject> priorTargetList = new();

            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", chance);
            parameter1.Add("EffectName", "Effect1");
            parameter1.Add("PriorTargetList", priorTargetList);

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.SelectEffectTarget, parameterNode1));

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

            Dictionary<string, object> damageParameter = new();
            damageParameter.Add("LaunchedSkill", chance);
            damageParameter.Add("EffectName", "Effect1");
            damageParameter.Add("EffectTarget", effectTarget);
            damageParameter.Add("DamageValue", chance.GetSkillValue());
            damageParameter.Add("DamageType", DamageType.Real);

            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
            parameterNode2.parameter = damageParameter;

            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode2));
        }

        string fullName = "Chance.Effect1";

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.SetParent(parameterNode.Parent.Parent.Parent, ParameterNodeChildType.EffectChild);
        parameterNode1.opportunity = "InRoundBattle";
        parameterNode1.result = new(parameterNode.Parent.Parent.result);
        parameterNode1.result.Add("ModifiedEffect", true);

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(chance, fullName, parameterNode1, Effect));
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = gameObject.GetComponentInChildren<MonsterInBattle>();
        string cardName = monsterInBattle.cardName;

        Dictionary<string, object> result1 = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        Dictionary<string, object> result2 = parameterNode.result;
        GameObject go = skillInBattle.gameObject;

        if (go != gameObject)
        {
            return false;
        }

        if (result1.ContainsKey("ModifiedEffect"))
        {
            return false;
        }

        if (result2.ContainsKey("BeReplaced"))
        {
            return false;
        }

        return true;
    }

    [TriggerEffect("^AfterRoundBattle$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect2");
        parameter2.Add("SkillName", "chance_fate_derive");
        parameter2.Add("Source", "Skill.ChanceFate.Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
    }

    /// <summary>
    /// 判断是否是对方回合
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Enemy)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}