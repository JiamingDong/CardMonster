using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ʼս����ť�Ľű�
/// </summary>
public class StartBattleButton : MonoBehaviour
{
    /// <summary>
    /// ����󣬽������ƽ׶�
    /// </summary>
    public void OnClick()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        PlayerAction playerAction= PlayerAction.GetInstance();

        if (battleProcess.allyPlayerData.perspectivePlayer != Player.Ally)
            return;

        Debug.Log("StartBattleButton���������ƽ׶�");

        SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.StartAttackPhase, new()));

        StartCoroutine(playerAction.DoAction(playerAction.StartRoundBattle,new()));
    }

}