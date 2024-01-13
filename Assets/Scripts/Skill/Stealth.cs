using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ǳ��
/// λ�ں���ʱ���޷���Ϊ<Զ��><ħ��>��Ŀ��
/// </summary>
public class Stealth : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.SelectEffectTarget$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        List<GameObject> nontargetList = (List<GameObject>)parameter["NontargetList"];

        nontargetList.Add(gameObject);

        yield break;
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����,�Ƿ��ں��ţ�����˺��ļ�����Զ��/ħ��
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            GameObject[] gameObjects = battleProcess.systemPlayerData[i].monsterGameObjectArray;
            if ((gameObjects[1] == gameObject || gameObjects[2] == gameObject) && ((skillInBattle is Ranged && effectName.Equals("Effect1")) || skillInBattle is Magic && effectName.Equals("Effect1")) && (gameObjects[0] != skillInBattle.gameObject && gameObjects[1] != skillInBattle.gameObject && gameObjects[2] != skillInBattle.gameObject))
            {
                return true;
            }
        }
        return false;
    }
}
