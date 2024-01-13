using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 精准随机
/// 此卡或者装备此卡的怪兽使用<随机>进行攻击时，将目标改为敌方当前血量最低的怪兽
/// </summary>
public class MaliciousChance : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.SelectEffectTarget$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        List<GameObject> priorTargetList = (List<GameObject>)parameter["PriorTargetList"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            bool isEnemy = true;
            GameObject effectTarget = null;
            int hp = 0;

            for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
            {
                GameObject go = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                if (go == gameObject)
                {
                    isEnemy = false;
                }
            }

            if (isEnemy)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    GameObject go = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                    if (go != null)
                    {
                        MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();
                        int currentHp = monsterInBattle.GetCurrentHp();

                        if (hp == 0 || currentHp < hp)
                        {
                            effectTarget = go;
                            hp = currentHp;
                        }
                    }
                }

                priorTargetList.Add(effectTarget);
                goto end;
            }
        }

    end:;

        yield break;
        //yield return null;
    }

    /// <summary>
    /// 判断是否是自己的随机
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        return skillInBattle is Chance && effectName.Equals("Effect1") && skillInBattle.gameObject == gameObject;
    }
}
