using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 网络传输的消息
/// </summary>
[System.Serializable]
public class NetworkMessage
{
    private NetworkMessageType type;
    private Dictionary<string, object> parameter;

    public NetworkMessage(NetworkMessageType type, Dictionary<string, object> parameter)
    {
        this.type = type;
        this.parameter = parameter;
    }
    public int Id { get; set; }
    public NetworkMessageType Type { get; set; }
    public Dictionary<string, object> Parameter { get; set; }

    public override string ToString()
    {
        string networkMessageToString = "type=" + type;
        if (parameter != null)
        {
            foreach (KeyValuePair<string, object> keyValuePair in parameter)
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
