using System.Collections.Generic;
using UnityEngine;

public class ChangeDestinyLaunch : MonoBehaviour
{
    /// <summary>
    /// �׼�ʱ�Ŀ�����š�˳��Ϊ���ƹ��ޡ����Ƶ��ߡ����Ϲ��ޣ��˿��ǵڼ�������0��ʼ
    /// </summary>
    public int cardIndex;

    public void OnClick()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        //���ڲ���ʹ�����Ƶ�״̬���򷵻�
        if (!battleProcess.allyPlayerData.canUseHandCard)
        {
            return;
        }

        battleProcess.allyPlayerData.canUseHandCard = false;

        Dictionary<string, object> parameter = new();
        parameter.Add("TargetNumber", cardIndex);

        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "LaunchHeroSkill";
        parameterNode1.parameter = parameter;

        SocketTool.SendMessage(new NetworkMessage(NetworkMessageType.LaunchHeroSkill, parameter));

        battleProcess.StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        Destroy(GameObject.Find("ChangeDestinyPrefabbInstantiation"));
    }
}
