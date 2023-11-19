using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerAccept : MonoBehaviour
{
    public void OnClick()
    {
        SocketTool.AcceptMessage();
    }
}
