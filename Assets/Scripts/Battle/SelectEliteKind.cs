using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用精英怪兽时，弹出的选择阵营的界面的两张卡上挂的脚本，点击后会选择这个阵营
/// </summary>
public class SelectEliteKind : MonoBehaviour
{
    /// <summary>
    /// DragHandCard传过来的卡牌目标位置等信息
    /// </summary>
    public Dictionary<string, object> parameter = new();
    /// <summary>
    /// 选精英技能的界面，此卡是第几个，从0开始
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
