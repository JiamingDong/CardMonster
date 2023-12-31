using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// ��Ϊ<Զ��>������Ŀ��
/// </summary>
public class Aggro : MonoBehaviour
{
    [TriggerEffect(@"^Before\.GameAction\.SelectEffectTarget$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        List<GameObject> priorTargetList = (List<GameObject>)parameter["PriorTargetList"];

        priorTargetList.Add(gameObject);

        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��ǶԷ����޵�Զ��
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (skillInBattle is Ranged && effectName.Equals("Effect1"))
        {
            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                bool isThis = false;
                bool isEnemy = false;

                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    GameObject go = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                    if (go == skillInBattle.gameObject)
                    {
                        isEnemy = true;
                    }

                    if (go == gameObject)
                    {
                        isThis = true;
                    }
                }

                if (isThis && !isEnemy)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
