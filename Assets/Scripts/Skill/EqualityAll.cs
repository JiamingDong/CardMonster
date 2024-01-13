using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualityAll : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                GameObject monsterGameObject = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                if (monsterGameObject == gameObject)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Dictionary<string, string> keyValuePairs = battleProcess.systemPlayerData[i].handMonster[k];

                        if (keyValuePairs != null)
                        {
                            keyValuePairs["CardCost"] = GetSkillValue().ToString();

                            if (battleProcess.systemPlayerData[i].handMonsterPanel != null)
                            {
                                CardForShow cardForShow = battleProcess.systemPlayerData[i].handMonsterPanel[k].GetComponent<Transform>().Find("CardForShow").gameObject.GetComponent<CardForShow>();
                                cardForShow.cost = GetSkillValue();
                                cardForShow.costText.text = GetSkillValue().ToString();
                            }
                        }
                    }

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是当前回合角色
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}