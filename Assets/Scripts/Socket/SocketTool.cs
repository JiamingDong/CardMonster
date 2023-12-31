using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// 网络连接工具类
/// </summary>
public class SocketTool
{
    /// <summary>
    /// 建立连接的socket
    /// </summary>
    public static Socket link;
    /// <summary>
    /// 接收、发送的socket
    /// </summary>
    public static Socket server;
    /// <summary>
    /// 发起的连接还是监听的连接，true发起，false监听
    /// </summary>
    public static bool clientOrListening;

    /// <summary>
    /// 接收消息线程
    /// </summary>
    public static Thread acceptMessageThread = new(ReceiveMessage);
    /// <summary>
    /// 对方动作队列
    /// </summary>
    public static Queue<NetworkMessage> enemyActionQueue = new();

    /// <summary>
    /// 发起连接
    /// </summary>
    /// <param name="iPAddress">IP地址</param>
    /// <param name="port">端口号</param>
    public static void StartClient(IPAddress iPAddress, int port)
    {
        CloseListening();

        IPEndPoint remoteEP = new(iPAddress, port);
        link = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientOrListening = true;
            link.Connect(remoteEP);
            Debug.Log("SocketTool.StartClient:" + link.RemoteEndPoint.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// 监听连接
    /// </summary>
    /// <param name="iPAddress">IP地址，默认127.0.0.1</param>
    /// <param name="port">端口号</param>
    public static void StartListening(IPAddress iPAddress, int port)
    {
        CloseListening();

        IPEndPoint localEndPoint = new(iPAddress, port);
        link = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientOrListening = false;
            link.Bind(localEndPoint);
            link.Listen(10);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        Debug.Log("开启监听成功！");

    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public static void CloseListening()
    {
        if (link != null)
        {
            Debug.Log("listener需要释放");
            if (link.Connected) link.Shutdown(SocketShutdown.Both);
            link.Close();
            link = null;
        }
        if (server != null)
        {
            Debug.Log("handler需要释放");
            if (server.Connected) server.Shutdown(SocketShutdown.Both);
            server.Close();
            server.Dispose();
            server = null;
        }
        Debug.Log("关闭套接字成功！");
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    /// <param name="message">消息</param>
    public static void SendMessage(NetworkMessage message)
    {
        if (server == null)
        {
            if (clientOrListening) server = link;
            else server = link.Accept();
        }

        BinaryFormatter bf = new();
        MemoryStream ms = new();
        bf.Serialize(ms, message);

        Debug.Log("ms.Length=" + (int)ms.Length);

        server.Send(ms.GetBuffer(), (int)ms.Length, SocketFlags.None);
        Debug.Log("SocketClient SendMessageFromListener 发送信息" + message.ToString());

        ms.Close();
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <returns></returns>
    public static void ReceiveMessage()
    {
        //Debug.Log("SocketClient.AcceptMessage:开始接收消息");
        if (server == null)
        {
            if (clientOrListening) server = link;
            else server = link.Accept();
        }

        while (true)
        {
            byte[] buffer = new byte[1024 * 1024];
            int currentLength = server.Receive(buffer);

            while (server.Available > 0)
            {
                int available = server.Available;

                Debug.Log("available=" + available);

                byte[] bytes = new byte[1460];
                int receiveLength = server.Receive(bytes);

                Array.Copy(bytes, 0, buffer, currentLength, receiveLength);

                currentLength += receiveLength;
            }

            if (currentLength > 0)
            {
                MemoryStream memory = new();
                memory.Write(buffer, 0, currentLength);
                memory.Position = 0;

                BinaryFormatter bf = new();
                NetworkMessage message = bf.Deserialize(memory) as NetworkMessage;
                Debug.Log("SocketClient.ReceiveMessage 接收消息=" + message.ToString());
                enemyActionQueue.Enqueue(message);

                memory.Close();
            }
        }
    }

    public static NetworkMessage GetNetworkMessage()
    {
        if (enemyActionQueue.Count > 0)
        {
            return enemyActionQueue.Dequeue();
        }
        return null;
    }
}
