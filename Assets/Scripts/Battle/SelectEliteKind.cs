using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ʹ�þ�Ӣ����ʱ��������ѡ����Ӫ�Ľ�������ſ��ϹҵĽű���������ѡ�������Ӫ
/// </summary>
public class SelectEliteKind : MonoBehaviour
{
    /// <summary>
    /// DragHandCard�������Ŀ���Ŀ��λ�õ���Ϣ
    /// </summary>
    public Dictionary<string, object> parameter = new();
    /// <summary>
    /// ѡ��Ӣ���ܵĽ��棬�˿��ǵڼ�������0��ʼ
    /// </summary>
    public int cardIndex;

    public void OnClick()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        parameter.Add("CardIndexBeSelect", cardIndex);

        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "CheckCardTarget";
        parameterNode1.parameter = parameter;

        SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.UseHandCard, parameter));

        battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        Destroy(GameObject.Find("SelectEliteKindPrefabInstantiation"));
    }
}
