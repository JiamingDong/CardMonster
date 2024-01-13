using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 狂扫
/// 近战攻击命中后，对所有其他敌人造成%d点真实伤害。
/// </summary>
public class Trample : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        Dictionary<string, object> parameter2 = parameterNode.Parent.Parent.EffectChild.parameter;

        GameObject monsterBeHurt = (GameObject)parameter2["EffectTarget"];

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

        for (int i = oppositePlayerData.monsterGameObjectArray.Length - 1; i > -1; i--)
        {
            GameObject go = oppositePlayerData.monsterGameObjectArray[i];
            if (go != null && go != monsterBeHurt)
            {
                Dictionary<string, object> damageParameter = new();
                damageParameter.Add("LaunchedSkill", this);
                damageParameter.Add("EffectName", "Effect1");
                damageParameter.Add("EffectTarget", go);
                damageParameter.Add("DamageValue", GetSkillValue());
                damageParameter.Add("DamageType", DamageType.Real);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
            }
        }

        //yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽，近战，对方后排有怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (!(launchedSkill is Melee && effectName.Equals("Effect1") && launchedSkill.gameObject == gameObject))
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            bool isEnemy = true;
            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    isEnemy = false;
                }
            }

            if (isEnemy && systemPlayerData.monsterGameObjectArray[1] == null)
            {
                return false;
            }
        }

        return true;
    }
}
