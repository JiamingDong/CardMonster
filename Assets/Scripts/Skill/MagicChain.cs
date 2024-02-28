using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ը��ħ��
/// ����<ħ��>����˺�ʱ�������ħ���˺���ת�Ƹ���һ��Ŀ�ꡣ
/// </summary>
public class MagicChain : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        Dictionary<string, object> parameter2 = parameterNode.Parent.Parent.EffectChild.parameter;
        Dictionary<string, object> result2 = parameterNode.Parent.Parent.EffectChild.result;

        GameObject monsterBeHurt = (GameObject)parameter2["EffectTarget"];
        int excessiveDamage = (int)result2["ExcessiveDamage"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //�Է����
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

        for (int i = 0; i < 3; i++)
        {
            GameObject go = oppositePlayerData.monsterGameObjectArray[i];
            if (go == monsterBeHurt)
            {
                GameObject target = i < 2 && oppositePlayerData.monsterGameObjectArray[i + 1] != null ? oppositePlayerData.monsterGameObjectArray[i + 1] : oppositePlayerData.monsterGameObjectArray[0];

                Dictionary<string, object> damageParameter = new();
                damageParameter.Add("LaunchedSkill", this);
                damageParameter.Add("EffectName", "Effect1");
                damageParameter.Add("EffectTarget", target);
                damageParameter.Add("DamageValue", excessiveDamage);
                damageParameter.Add("DamageType", DamageType.Magic);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�ħ��������������
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        SkillInBattle launchedSkill = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        Dictionary<string, object> parameter2 = parameterNode.Parent.EffectChild.parameter;
        Dictionary<string, object> result2 = parameterNode.Parent.EffectChild.result;

        GameObject monsterBeHurt = (GameObject)parameter2["EffectTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (!result2.ContainsKey("ExcessiveDamage"))
        {
            return false;
        }

        if (!(((launchedSkill is Magic && effectName.Equals("Effect1")) || (launchedSkill is MagicChain && effectName.Equals("Effect1"))) && launchedSkill.gameObject == gameObject))
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

                if (systemPlayerData.monsterGameObjectArray[j] != monsterBeHurt && systemPlayerData.monsterGameObjectArray[j] != null && systemPlayerData.monsterGameObjectArray[j].GetComponent<MonsterInBattle>().GetCurrentHp() > 0)
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