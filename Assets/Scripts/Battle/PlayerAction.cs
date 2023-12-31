using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 玩家的动作类
/// </summary>
public class PlayerAction : MonoBehaviour
{
    /// <summary>
    /// 本类实例
    /// </summary>
    private static PlayerAction playerAction;
    /// <summary>
    /// 获得本类实例
    /// </summary>
    /// <returns>本类实例</returns>
    public static PlayerAction GetInstance()
    {
        if (playerAction == null)
        {
            playerAction = GameObject.Find("BattleProcessSystem").GetComponent<PlayerAction>();
        }
        return playerAction;
    }

    /// <summary>
    /// 执行对应动作
    /// </summary>
    /// <param name="effectName">效果方法委托</param>
    /// <param name="parameter">传给效果的参数</param>
    /// <returns></returns>
    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string fullName = "PlayerAction." + effect.Method.Name;

        ParameterNode parameterNode = new();
        parameterNode.SetParent(new(), ParameterNodeChildType.EffectChild);
        parameterNode.parameter = parameter;

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, fullName, parameterNode, effect));
        yield return null;
    }

    /// <summary>
    /// 发动英雄技能
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator LaunchHeroSkill(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["PlayerLaunchHeroSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        foreach (PlayerData playerData in battleProcess.systemPlayerData)
        {
            if (playerData.perspectivePlayer == player)
            {
                GameObject gameObject = playerData.heroSkillGameObject;
                HeroSkillInBattle heroSkillInBattle = gameObject.GetComponent<HeroSkillInBattle>();
                SkillInBattle skillInBattle = heroSkillInBattle.skillList[0];

                ParameterNode parameterNode1 = new();
                parameterNode1.opportunity = "LaunchHeroSkill";

                parameterNode1.parameter.Add("PreviousParameter", parameter);

                yield return StartCoroutine(skillInBattle.ExecuteEligibleEffect(parameterNode1));
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 进入回合战斗阶段
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public IEnumerator StartRoundBattle(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "TransfromOpportunity";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));
        yield return null;

        //战斗阶段开始前
        ParameterNode parameterNode2 = new();
        parameterNode2.opportunity = "BeforeRoundBattle";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode2));
        yield return null;

        battleProcess.gamePhase = GamePhase.RoundBattle;

        //战斗阶段
        ParameterNode parameterNode3 = new();
        parameterNode3.opportunity = "InRoundBattle";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode3));
        yield return null;

        //战斗阶段后
        ParameterNode parameterNode4 = new();
        parameterNode4.opportunity = "AfterRoundBattle";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
        yield return null;

        //进入下个回合
        ParameterNode parameterNode5 = new();
        parameterNode5.opportunity = "InRoundReady";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode5));
        yield return null;
    }
}
