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
    public static Thread acceptMessageThread = new(ReceiveMessage);
    /// <summary>
    /// �Է���������
    /// </summary>
    public static Queue<NetworkMessage> enemyActionQueue = new();

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="iPAddress">IP��ַ</param>
    /// <param name="port">�˿ں�</param>
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
    /// ��������
    /// </summary>
    /// <param name="iPAddress">IP��ַ��Ĭ��127.0.0.1</param>
    /// <param name="port">�˿ں�</param>
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

        BinaryFormatter bf = new();
        MemoryStream ms = new();
        bf.Serialize(ms, message);

        Debug.Log("ms.Length=" + (int)ms.Length);

        server.Send(ms.GetBuffer(), (int)ms.Length, SocketFlags.None);
        Debug.Log("SocketClient SendMessageFromListener ������Ϣ" + message.ToString());

        ms.Close();
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <returns></returns>
    public static void ReceiveMessage()
    {
        //Debug.Log("SocketClient.AcceptMessage:��ʼ������Ϣ");
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
                Debug.Log("SocketClient.ReceiveMessage ������Ϣ=" + message.ToString());
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
