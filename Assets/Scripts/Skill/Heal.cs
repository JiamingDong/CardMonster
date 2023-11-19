using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 治疗
/// 
/// </summary>
public class Heal : SkillInBattle
{
    public Heal(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("InRoundBattle")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //发动技能的怪兽所属玩家
        PlayerData playerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    playerMessage = battleProcess.systemPlayerData[i];
                    goto end;
                }
            }
        }
    end:;

        GameObject effectTarget = null;
        int previousLostHP = 0;
        for (int i = 2; i > -1; i--)
        {
            GameObject gameObject = playerMessage.monsterGameObjectArray[i];
            if (gameObject != null)
            {
                MonsterInBattle monsterInBattle2 = gameObject.GetComponent<MonsterInBattle>();
                int lostHP = monsterInBattle2.maximumHp - monsterInBattle2.currentHp;
                if (effectTarget == null && lostHP > 0)
                {
                    effectTarget = gameObject;
                }
                else if (lostHP > previousLostHP)
                {
                    effectTarget = gameObject;
                }
                previousLostHP = lostHP;
            }
        }
        if (effectTarget == null)
        {
            yield break;
        }

        //治疗
        Dictionary<string, object> treatParameter = new();
        //当前技能
        treatParameter.Add("LaunchedSkill", this);
        //效果名称
        treatParameter.Add("EffectName", "Effect1");
        //受到治疗的怪兽
        treatParameter.Add("MonsterBeTreat", effectTarget);
        //治疗数值
        treatParameter.Add("TreatValue", GetSKillValue());

        yield return StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, treatParameter));

        yield return null;
    }
}