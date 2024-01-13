using UnityEngine;

/// <summary>
/// 战斗界面，投降按钮
/// </summary>
public class GiveUp : MonoBehaviour
{
    public void OnClick()
    {
        NetworkMessage networkMessage = new();
        networkMessage.Type = NetworkMessageType.ExitBattle;
        networkMessage.Parameter = new();

        SocketTool.SendMessage(networkMessage);

        BattleProcess battleProcess=BattleProcess.GetInstance();
        battleProcess.GameDefeat();
    }
}
