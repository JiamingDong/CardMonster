using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 践踏
/// 以<近战>、<远程>攻击对方怪兽后，对敌方所有其他怪兽造成总量等同于过量伤害数值的物理伤害(平均分配)
/// </summary>
public class Tread : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        Dictionary<string, object> parameter2 = parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.parameter;
        Dictionary<string, object> result2 = parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.result;

        GameObject monsterBeHurt = (GameObject)parameter2["EffectTarget"];
        int excessiveDamage = (int)result2["ExcessiveDamage"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //对方玩家
        PlayerData oppositePlayerData = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerData = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        int monsterAmount = 0;
        for (int i = 0; i < oppositePlayerData.monsterGameObjectArray.Length; i++)
        {
            if (oppositePlayerData.monsterGameObjectArray[i] != null && oppositePlayerData.monsterGameObjectArray[i] != monsterBeHurt)
            {
                monsterAmount++;
            }
        }

        int damageValue = Mathf.FloorToInt((float)excessiveDamage / monsterAmount);

        for (int i = oppositePlayerData.monsterGameObjectArray.Length - 1; i > -1; i--)
        {
            GameObject go = oppositePlayerData.monsterGameObjectArray[i];
            if (go != null)
            {
                Dictionary<string, object> damageParameter = new();
                damageParameter.Add("LaunchedSkill", this);
                damageParameter.Add("EffectName", "Effect1");
                damageParameter.Add("EffectTarget", go);
                damageParameter.Add("DamageValue", damageValue);
                damageParameter.Add("DamageType", DamageType.Physics);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
            }
        }

        yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽，近战或远程，对方后排有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        foreach (var item in parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.parameter)
        {
            Debug.Log(item.Key + "=" + item.Value);
        }

        Dictionary<string, object> parameter2 = parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.parameter;
        Dictionary<string, object> result2 = parameterNode.Parent.EffectChild.nodeInMethodList[0].EffectChild.result;

        GameObject monsterBeHurt = (GameObject)parameter2["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (result2.ContainsKey("ExcessiveDamage"))
        {
            Debug.Log("ExcessiveDamage");
            return false;
        }

        if (!(((launchedSkill is Melee && effectName.Equals("Effect1")) || (launchedSkill is Ranged && effectName.Equals("Effect1"))) && launchedSkill.gameObject == gameObject))
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            bool isEnemy = true;
            bool hasOtherMonster = false;
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    isEnemy = false;
                }

                if (systemPlayerData.monsterGameObjectArray[j] != monsterBeHurt && systemPlayerData.monsterGameObjectArray[j] != null)
                {
                    hasOtherMonster = true;
                }
            }

            if (isEnemy && hasOtherMonster)
            {
                return true;
            }
        }

        return false;
    }
}
