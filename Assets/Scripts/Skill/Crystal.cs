using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ˮ��
/// ���׼�ʱ��������%d��ˮ��
/// �����еġ�����ˮ����ͨ��RuleEvent.Crystal����
/// </summary>
public class Crystal : SkillInBattle
{
    void Start()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("Before.GameAction.Sacrifice", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("CrystalAmount", GetSKillValue());
        parameter1.Add("Player", player);
        yield return StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameter1));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (objectBeSacrificedNumber >= 4 && objectBeSacrificedNumber <= 6)
        {
            int t = objectBeSacrificedNumber - 4;

            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

                if (systemPlayerData.perspectivePlayer == player)
                {
                    if (systemPlayerData.monsterGameObjectArray[t] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
