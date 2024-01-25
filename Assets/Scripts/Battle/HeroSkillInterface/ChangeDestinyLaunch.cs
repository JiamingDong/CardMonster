using System.Collections.Generic;
using UnityEngine;

public class ChangeDestinyLaunch : MonoBehaviour
{
    /// <summary>
    /// 献祭时的卡牌序号。顺序为手牌怪兽、手牌道具、场上怪兽，此卡是第几个，从0开始
    /// </summary>
    public int cardIndex;

    public void OnClick()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        //处于不能使用手牌的状态，则返回
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
