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
            battleProcess.allyPlayerData.canUseHandCard = true;
            battleProcess.allyPlayerData.canSacrifice = true;

            Debug.Log("RuleEvent.EnterTurnReady:�ҷ��غ�׼���׶ν���");
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
            battleProcess.enemyPlayerData.canUseHandCard = true;
            battleProcess.enemyPlayerData.canSacrifice = true;

            Debug.Log("RuleEvent.EnterTurnReady���Է��غ�׼���׶ο�ʼ");
        }

        yield return null;
    }

    /// <summary>
    /// ����ʹ������
    /// </summary>
    /// <param name="parameterNode"></param>
    /// <returns></returns>
    [TriggerEffect("^CheckCardTarget$")]
    [TriggerEffect("^CheckSacrifice$")]
    public IEnumerator AllowUsingCard(ParameterNode parameterNode)
    {
        //Debug.Log("����ʹ������AllowUsingCard");
        Dictionary<string, object> parameter = parameterNode.parameter;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        Player player = (Player)parameter["Player"];

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == player)
            {
                //����ʹ������
                battleProcess.systemPlayerData[i].canUseHandCard = true;

                //�������Ʒ
                //Debug.Log("����Ʒ�뿪ս��");
                Destroy(battleProcess.systemPlayerData[i].consumeGameObject);
                battleProcess.systemPlayerData[i].consumeGameObject = null;
                break;
            }
        }

        yield return null;
    }
}
