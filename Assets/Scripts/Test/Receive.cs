using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Receive : MonoBehaviour
{
    //private void Awake()
    //{
    //    SocketTool.StartListening(IPAddress.Parse("127.0.0.1"), 11111);
    //    SocketTool.acceptMessageThread.Start();
    //}

    public void OnClick()
    {
        NetworkMessage networkMessage = SocketTool.GetNetworkMessage();
        Debug.Log("Receive.OnClick£º" + networkMessage.ToString());
    }
}
