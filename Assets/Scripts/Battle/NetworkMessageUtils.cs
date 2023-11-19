using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMessageUtils : MonoBehaviour
{
    /// <summary>
    /// 获取随机值，包括上下限
    /// </summary>
    /// <param name="lowerLimit"></param>
    /// <param name="upperLimit"></param>
    /// <returns></returns>
    public static IEnumerator GetRandomResult(int lowerLimit, int upperLimit, Dictionary<string, object> result)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (battleProcess.allyPlayerData.perspectivePlayer == Player.Ally)
        {
            int randomResult = new System.Random().Next(lowerLimit, upperLimit + 1);

            Dictionary<string, object> parameter = new Dictionary<string, object>();
            parameter.Add("RandomResult", randomResult);

            NetworkMessage networkMessage = new(NetworkMessageType.ProbabilityResult, parameter);
            SocketTool.SendMessage(networkMessage);
        }
        else
        {
            NetworkMessage networkMessage;
            do
            {
                networkMessage = SocketTool.GetNetworkMessage();
                yield return null;
            } while (networkMessage == null);
            result = networkMessage.Parameter;
        }

        yield return null;
    }
}
