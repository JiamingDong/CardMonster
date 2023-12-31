using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 开始战斗按钮的脚本
/// </summary>
public class StartBattleButton : MonoBehaviour
{
    /// <summary>
    /// 点击后，结束出牌阶段
    /// </summary>
    public void OnClick()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        PlayerAction playerAction = PlayerAction.GetInstance();

        if (battleProcess.allyPlayerData.perspectivePlayer != Player.Ally || battleProcess.allyPlayerData.canUseHandCard == false)
            return;

        Debug.Log("StartBattleButton：结束出牌阶段");

        battleProcess.allyPlayerData.canUseHandCard = false;
        battleProcess.allyPlayerData.canSacrifice = false;

        SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.StartAttackPhase, new()));

        StartCoroutine(playerAction.DoAction(playerAction.StartRoundBattle, new()));
    }

}
