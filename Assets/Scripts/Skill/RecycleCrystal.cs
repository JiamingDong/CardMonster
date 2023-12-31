using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ˮ������
/// ���������������%d��ˮ����
/// </summary>
public class RecycleCrystal : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.DestroyMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, object> parameter = new();
                    parameter.Add("CrystalAmount", GetSkillValue());
                    parameter.Add("Player", systemPlayerData.perspectivePlayer);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.ChangeCrystalAmount, parameterNode1));
                    goto a;
                }
            }
        }

    a:;

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
        GameObject monsterBeDestroy = (GameObject)parameter["MonsterBeDestroy"];
        return monsterBeDestroy == gameObject;
    }
}
