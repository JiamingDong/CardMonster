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
/// �������ӹ�����
/// </summary>
public class SocketTool
{
    /// <summary>
    /// �������ӵ�socket
    /// </summary>
    public static Socket link;
    /// <summary>
    /// ���ա����͵�socket
    /// </summary>
    public static Socket server;
    /// <summary>
    /// ��������ӻ��Ǽ��������ӣ�true����false����
    /// </summary>
    public static bool clientOrListening;

    /// <summary>
    /// ������Ϣ�߳�
    /// </summary>
    public static Thread acceptMessageThread = new Thread(AcceptMessage);
    /// <summary>
    /// ��Ϣ����
    /// </summary>
    public static LinkedList<NetworkMessage> networkMessagQueue = new LinkedList<NetworkMessage>();

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="iPAddress">IP��ַ</param>
    /// <param name="port">�˿ں�</param>
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
    /// ��������
    /// </summary>
    /// <param name="iPAddress">IP��ַ��Ĭ��127.0.0.1</param>
    /// <param name="port">�˿ں�</param>
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

        Debug.Log("���������ɹ���");

    }

    /// <summary>
    /// �ر�����
    /// </summary>
    public static void CloseListening()
    {
        if (link != null)
        {
            Debug.Log("listener��Ҫ�ͷ�");
            if (link.Connected) link.Shutdown(SocketShutdown.Both);
            link.Close();
            link = null;
        }
        if (server != null)
        {
            Debug.Log("handler��Ҫ�ͷ�");
            if (server.Connected) server.Shutdown(SocketShutdown.Both);
            server.Close();
            server.Dispose();
            server = null;
        }
        Debug.Log("�ر��׽��ֳɹ���");
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="message">��Ϣ</param>
    public static void SendMessage(NetworkMessage message)
    {
        if (server == null)
        {
            if (clientOrListening) server = link;
            else server = link.Accept();
        }

        BinaryFormatter bf = new BinaryFormatter(); //�������л�����
        MemoryStream ms = new MemoryStream(); //�����ڴ�������
        bf.Serialize(ms, message); //��p���л����ڴ�����
        server.Send(ms.GetBuffer(), (int)ms.Length, SocketFlags.None);// ����,����Ϊ�ڴ��������ݵĳ���
        Debug.Log("SocketClient SendMessageFromListener ������Ϣ" + message.ToString());


    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <returns></returns>
    public static void AcceptMessage()
    {
        Debug.Log("SocketClient.AcceptMessage:��ʼ������Ϣ");
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
            //��isAllyAction����Ϊfalse
            //if (message.Parameter.ContainsKey("isAllyAction")) message.Parameter["message.Parameter"] = false;
            Debug.Log("SocketClient AcceptMessage ������Ϣ" + message.ToString());
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
