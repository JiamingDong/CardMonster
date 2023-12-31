using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ��ҵĶ�����
/// </summary>
public class PlayerAction : MonoBehaviour
{
    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static PlayerAction playerAction;
    /// <summary>
    /// ��ñ���ʵ��
    /// </summary>
    /// <returns>����ʵ��</returns>
    public static PlayerAction GetInstance()
    {
        if (playerAction == null)
        {
            playerAction = GameObject.Find("BattleProcessSystem").GetComponent<PlayerAction>();
        }
        return playerAction;
    }

    /// <summary>
    /// ִ�ж�Ӧ����
    /// </summary>
    /// <param name="effectName">Ч������ί��</param>
    /// <param name="parameter">����Ч���Ĳ���</param>
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
    /// ����Ӣ�ۼ���
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
    /// ����غ�ս���׶�
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

        //ս���׶ο�ʼǰ
        ParameterNode parameterNode2 = new();
        parameterNode2.opportunity = "BeforeRoundBattle";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode2));
        yield return null;

        battleProcess.gamePhase = GamePhase.RoundBattle;

        //ս���׶�
        ParameterNode parameterNode3 = new();
        parameterNode3.opportunity = "InRoundBattle";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode3));
        yield return null;

        //ս���׶κ�
        ParameterNode parameterNode4 = new();
        parameterNode4.opportunity = "AfterRoundBattle";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));
        yield return null;

        //�����¸��غ�
        ParameterNode parameterNode5 = new();
        parameterNode5.opportunity = "InRoundReady";

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode5));
        yield return null;
    }
}
