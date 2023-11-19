using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using UnityEngine;

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
    public static Thread acceptMessageThread = new Thread(AcceptMessage);
    /// <summary>
    /// 消息队列
    /// </summary>
    public static LinkedList<NetworkMessage> networkMessagQueue = new LinkedList<NetworkMessage>();

    /// <summary>
    /// 发起连接
    /// </summary>
    /// <param name="iPAddress">IP地址</param>
    /// <param name="port">端口号</param>
    public static void StartClient(IPAddress iPAddress, int port)
    {
        CloseListening();

        IPEndPoint remoteEP = new IPEndPoint(iPAddress, port);
        link = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientOrListening = true;
            link.Connect(remoteEP);
            Debug.Log("SocketTool.StartClient:" + link.RemoteEndPoint.ToString());
        }
        catch (ArgumentNullException ane)
        {
            Debug.Log("SocketTool.StartClient:ArgumentNullException=" + ane.ToString());
        }
        catch (SocketException se)
        {
            Debug.Log("SocketTool.StartClient:SocketException=" + se.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("SocketTool.StartClient:Unexpected exception=" + e.ToString());
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

        IPEndPoint localEndPoint = new IPEndPoint(iPAddress, port);
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

        BinaryFormatter bf = new BinaryFormatter(); //创建序列化对象
        MemoryStream ms = new MemoryStream(); //创建内存流对象
        bf.Serialize(ms, message); //将p序列化到内存流中
        server.Send(ms.GetBuffer(), (int)ms.Length, SocketFlags.None);// 发送,长度为内存流中数据的长度
        Debug.Log("SocketClient SendMessageFromListener 发送信息" + message.ToString());


    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <returns></returns>
    public static void AcceptMessage()
    {
        Debug.Log("SocketClient.AcceptMessage:开始接收消息");
        if (server == null)
        {
            if (clientOrListening) server = link;
            else server = link.Accept();
        }
        while (true)
        {
            byte[] buffer = new byte[1024 * 1024];
            int len = server.Receive(buffer);
            MemoryStream memory = new();
            BinaryFormatter bf = new();

            memory.Write(buffer, 0, len);
            memory.Flush();
            memory.Position = 0;

            NetworkMessage message = bf.Deserialize(memory) as NetworkMessage;
            //将isAllyAction设置为false
            //if (message.Parameter.ContainsKey("isAllyAction")) message.Parameter["message.Parameter"] = false;
            Debug.Log("SocketClient AcceptMessage 接收消息" + message.ToString());
            networkMessagQueue.AddLast(message);
        }

    }

    public static NetworkMessage GetNetworkMessage()
    {
        if (networkMessagQueue.Count > 0)
        {
            NetworkMessage networkMessage = networkMessagQueue.First.Value;
            networkMessagQueue.RemoveFirst();
            return networkMessage;
        }
        return null;
    }
}
