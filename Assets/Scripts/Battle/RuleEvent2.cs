using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ͬʱ������󴥷�����Ϸ����
/// </summary>
public class RuleEvent2 : OpportunityEffect
{
    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static RuleEvent2 instance;

    /// <summary>
    /// ��ñ���ʵ��
    /// </summary>
    /// <returns></returns>
    public static RuleEvent2 GetInstance()
    {
        if (instance == null)
            instance = GameObject.Find("BattleProcessSystem").GetComponent<RuleEvent2>();
        return instance;
    }

    /// <summary>
    /// ����غ�׼���׶�
    /// ʱ�� Opportunity.EnterTurnReady
    /// </summary>
    [TriggerEffect("^InRoundReady$")]
    public IEnumerator InRoundReady(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (battleProcess.allyPlayerData.perspectivePlayer == Player.Ally)
        {
            GameObject allyStartBattleButtonImage = GameObject.Find("AllyStartBattleButtonImage");
            RawImage rawImage = allyStartBattleButtonImage.GetComponent<RawImage>();
            rawImage.texture = Resources.Load<Texture2D>("Image/AllyStartBattleButton");

            GameObject sacrificeButtonImage = GameObject.Find("SacrificeButtonImage");
            RawImage sacrificeButtonRawImage = sacrificeButtonImage.GetComponent<RawImage>();
            sacrificeButtonRawImage.texture = Resources.Load<Texture2D>("Image/SacrificeEnableButton");

            //����ʹ������
            battleProcess.allyPlayerData.canSacrifice = true;
            battleProcess.allyPlayerData.canUseHandCard = true;

            //Debug.Log("RuleEvent.EnterTurnReady:�ҷ��غ�׼���׶ν���");
        }
        else if (battleProcess.enemyPlayerData.perspectivePlayer == Player.Ally)
        {
            GameObject allyStartBattleButtonImage = GameObject.Find("AllyStartBattleButtonImage");
            RawImage rawImage = allyStartBattleButtonImage.GetComponent<RawImage>();

            rawImage.texture = Resources.Load<Texture2D>("Image/EnemyStartBattleButton");

            GameObject sacrificeButtonImage = GameObject.Find("SacrificeButtonImage");
            RawImage sacrificeButtonRawImage = sacrificeButtonImage.GetComponent<RawImage>();
            sacrificeButtonRawImage.texture = Resources.Load<Texture2D>("Image/SacrificeDisableButton");

            //����ʹ������
            battleProcess.enemyPlayerData.canSacrifice = true;
            battleProcess.enemyPlayerData.canUseHandCard = true;

            //Debug.Log("RuleEvent.EnterTurnReady���Է��غ�׼���׶ο�ʼ");
        }

        yield break;
    }

    /// <summary>
    /// ����ʹ������
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^CheckCardTarget$")]
    [TriggerEffect("^CheckSacrifice$")]
    [TriggerEffect("^LaunchHeroSkill$")]
    public IEnumerator AllowUsingCard(ParameterNode parameterNode)
    {
        //Debug.Log(parameterNode.opportunity);
        //Debug.Log("����ʹ������1");
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];
            if (playerData.perspectivePlayer == Player.Ally)
            {
                //Debug.Log("����ʹ������2");
                //����ʹ������
                playerData.canUseHandCard = true;

                //�������Ʒ
                if (playerData.consumeGameObject != null)
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("Player", Player.Ally);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ConsumeLeave, parameterNode1));
                }

                break;
            }
        }

        yield break;
    }

    /// <summary>
    /// �������˺󣬽�������
    /// </summary>
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator DestroyMonster(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> destroyParameter = new();
        destroyParameter.Add("EffectTarget", monsterBeHurt);
        destroyParameter.Add("Destroyer", skillInBattle.gameObject);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = destroyParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyMonster, parameterNode1));
        yield return null;
    }

    /// <summary>
    /// �ж�����С��1
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];

        if (monsterBeHurt == null)
        {
            return false;
        }

        MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();

        return monsterInBattle.GetCurrentHp() < 1;
    }
}
