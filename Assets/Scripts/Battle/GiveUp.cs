using UnityEngine;

/// <summary>
/// ս�����棬Ͷ����ť
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
