using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Send : MonoBehaviour
{
    public void OnClick()
    {
        NetworkMessageType networkMessageType = NetworkMessageType.UseHandCard;
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        keyValuePairs.Add("test", "testMessage");
        NetworkMessage networkMessage = new NetworkMessage(networkMessageType, keyValuePairs);
        SocketTool.StartClient(IPAddress.Parse("127.0.0.1"), 11111);
        SocketTool.SendMessage(networkMessage);
    }
}
