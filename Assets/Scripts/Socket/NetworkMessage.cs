using System.Collections.Generic;

/// <summary>
/// 网络传输的消息
/// </summary>
[System.Serializable]
public class NetworkMessage
{
    private NetworkMessageType type;
    private Dictionary<string, object> parameter;

    public NetworkMessageType Type { get => type; set => type = value; }
    public Dictionary<string, object> Parameter { get => parameter; set => parameter = value; }

    public NetworkMessage()
    {
    }

    public NetworkMessage(NetworkMessageType type, Dictionary<string, object> parameter)
    {
        Type = type;
        Parameter = parameter;
    }

    public override string ToString()
    {
        string networkMessageToString = "type=" + Type;
        if (Parameter != null)
        {
            foreach (KeyValuePair<string, object> keyValuePair in Parameter)
            {
                networkMessageToString += "," + keyValuePair.Key + "," + keyValuePair.Value.ToString();
            }
        }
        else
        {
            networkMessageToString += ",parameter=null";
        }
        return networkMessageToString;
    }
}
