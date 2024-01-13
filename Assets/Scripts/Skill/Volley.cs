using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭雨
/// <远程>攻击时，以所有后排敌人为目标，且无视<嘲讽><潜行>
/// </summary>
public class Volley : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Ranged\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        result.Add("BeReplaced", true);

        //对方玩家
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        Ranged ranged = gameObject.GetComponent<Ranged>();

        //选取技能目标
        for (int i = 2; i > 0; i--)
        {
            GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[i];

            if (effectTarget == null)
            {
                continue;
            }

            IEnumerator Effect(ParameterNode parameterNode)
            {
                Dictionary<string, object> damageParameter = new();
                damageParameter.Add("LaunchedSkill", ranged);
                damageParameter.Add("EffectName", "Effect1");
                damageParameter.Add("EffectTarget", effectTarget);
                damageParameter.Add("DamageValue", ranged.GetSkillValue());
                damageParameter.Add("DamageType", DamageType.Physics);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
                //yield return null;
            }

            string fullName = "Ranged.Effect1";

            ParameterNode parameterNode1 = new();
            parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
            parameterNode1.result.Add("isAdditionalExecute", true);

            yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(ranged, fullName, parameterNode1, Effect));
            //yield return null;
        }
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        Dictionary<string, object> result1 = parameterNode.result;
        GameObject go = skillInBattle.gameObject;

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (result1.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (result.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        if (go != gameObject)
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
