using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ˮ����ȡ
/// �����������ʱ�����%d��ˮ��
/// </summary>
public class DrainCrystal : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.DestroyMonster$",  "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("CrystalAmount", GetSkillValue());
                    parameter1.Add("Player", systemPlayerData.perspectivePlayer);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����/����Ʒ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject destroyer = (GameObject)parameter["Destroyer"];
        if (destroyer == gameObject)
        {
            return true;
        }
        return false;
    }
}
