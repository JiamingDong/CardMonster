//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Text;
//using UnityEngine;

//public class SocketClient : MonoBehaviour
//{
//    // Create a TCP/IP socket.
//    public static Socket sender;
//    public static string endMark = "<EOF>";

//    /// <summary>
//    /// 建立连接
//    /// </summary>
//    public static void StartClient(IPAddress ipAddress, int port)
//    {
//        IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
//        sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//        try
//        {
//            sender.Connect(remoteEP);
//            Debug.Log("SocketClient StartClient " + sender.RemoteEndPoint.ToString());
//        }
//        catch (ArgumentNullException ane)
//        {
//            Debug.Log("ArgumentNullException : {0}" + ane.ToString());
//        }
//        catch (SocketException se)
//        {
//            Debug.Log("SocketException : {0}" + se.ToString());
//        }
//        catch (Exception e)
//        {
//            Debug.Log("Unexpected exception : {0}" + e.ToString());
//        }
//    }

//    /// <summary>
//    /// 发送信息
//    /// </summary>
//    /// <param name="message"></param>
//    public static void SendMessageFromClient(NetworkMessage message)
//    {
//        BinaryFormatter bf = new BinaryFormatter(); //创建序列化对象
//        MemoryStream ms = new MemoryStream(); //创建内存流对象
//        bf.Serialize(ms, message); //将p序列化到内存流中
//        sender.Send(ms.GetBuffer(), (int)ms.Length, SocketFlags.None);// 发送,长度为内存流中数据的长度
//        Debug.Log("SocketClient SendMessageFromClient 发送信息");
//    }

//    /// <summary>
//    /// 接收消息
//    /// </summary>
//    /// <returns></returns>
//    public static NetworkMessage AcceptMessageFromClient()
//    {
//        byte[] buffer = new byte[1024 * 1024];
//        int len = sender.Receive(buffer);
//        MemoryStream memory = new MemoryStream();
//        BinaryFormatter bf = new BinaryFormatter();

//        memory.Write(buffer, 0, len);
//        memory.Flush();
//        memory.Position = 0;

//        NetworkMessage message = bf.Deserialize(memory) as NetworkMessage;
//        Debug.Log("SocketClient AcceptMessage 接收消息"+ message.Parameter["type"]);
//        return message;
//    }

//    /// <summary>
//    /// 本机是客户端，关闭连接
//    /// </summary>
//    public static void CloseListening()
//    {
//        if (sender != null)
//        {
//            Debug.Log("sender需要释放");
//            if (sender.Connected) sender.Shutdown(SocketShutdown.Both);
//            sender.Close();
//            sender.Dispose();
//            sender = null;
//        }
//        Debug.Log("SocketClient EndListening ");
//    }
//}
