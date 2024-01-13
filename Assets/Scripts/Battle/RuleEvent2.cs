using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 相同时机，最后触发的游戏规则
/// </summary>
public class RuleEvent2 : OpportunityEffect
{
    /// <summary>
    /// 本类实例
    /// </summary>
    private static RuleEvent2 instance;

    /// <summary>
    /// 获得本类实例
    /// </summary>
    /// <returns></returns>
    public static RuleEvent2 GetInstance()
    {
        if (instance == null)
            instance = GameObject.Find("BattleProcessSystem").GetComponent<RuleEvent2>();
        return instance;
    }

    /// <summary>
    /// 进入回合准备阶段
    /// 时机 Opportunity.EnterTurnReady
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

            //可以使用手牌
            battleProcess.allyPlayerData.canUseHandCard = true;
            battleProcess.allyPlayerData.canSacrifice = true;

            Debug.Log("RuleEvent.EnterTurnReady:我方回合准备阶段结束");
        }
        else if (battleProcess.enemyPlayerData.perspectivePlayer == Player.Ally)
        {
            GameObject allyStartBattleButtonImage = GameObject.Find("AllyStartBattleButtonImage");
            RawImage rawImage = allyStartBattleButtonImage.GetComponent<RawImage>();

            rawImage.texture = Resources.Load<Texture2D>("Image/EnemyStartBattleButton");

            GameObject sacrificeButtonImage = GameObject.Find("SacrificeButtonImage");
            RawImage sacrificeButtonRawImage = sacrificeButtonImage.GetComponent<RawImage>();
            sacrificeButtonRawImage.texture = Resources.Load<Texture2D>("Image/SacrificeDisableButton");

            //可以使用手牌
            battleProcess.enemyPlayerData.canUseHandCard = true;
            battleProcess.enemyPlayerData.canSacrifice = true;

            Debug.Log("RuleEvent.EnterTurnReady：对方回合准备阶段开始");
        }

        yield return null;
    }

    /// <summary>
    /// 允许使用手牌
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^CheckCardTarget$")]
    [TriggerEffect("^CheckSacrifice$")]
    public IEnumerator AllowUsingCard(ParameterNode parameterNode)
    {
        //Debug.Log("允许使用手牌AllowUsingCard");
        Dictionary<string, object> parameter = parameterNode.parameter;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        Player player = (Player)parameter["Player"];

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == player)
            {
                //允许使用手牌
                battleProcess.systemPlayerData[i].canUseHandCard = true;

                //清除消耗品
                //Debug.Log("消耗品离开战场");
                Destroy(battleProcess.systemPlayerData[i].consumeGameObject);
                battleProcess.systemPlayerData[i].consumeGameObject = null;
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// 怪兽受伤后，结算死亡
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
    /// 判断生命小于1
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
