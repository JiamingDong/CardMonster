using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeButton : MonoBehaviour
{
    /// <summary>
    /// �׼�ʱ�Ŀ�����š�˳��Ϊ���ƹ��ޡ����Ƶ��ߡ����Ϲ��ޣ��˿��ǵڼ�������0��ʼ
    /// </summary>
    public int cardIndex;

    public void OnClick()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        //���ڲ���ʹ�����Ƶ�״̬���򷵻�
        if (!battleProcess.allyPlayerData.canUseHandCard || !battleProcess.allyPlayerData.canSacrifice)
        {
            return;
        }

        battleProcess.allyPlayerData.canUseHandCard = false;

        Dictionary<string, object> parameter = new();
        parameter.Add("Player", Player.Ally);
        parameter.Add("ObjectBeSacrificedNumber", cardIndex);

        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "CheckSacrifice";
        parameterNode1.parameter = parameter;

        SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.SacrificeCard, parameter));

        battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        Destroy(GameObject.Find("SacrificeInterfacePrefabInstantiation"));
    }
}
