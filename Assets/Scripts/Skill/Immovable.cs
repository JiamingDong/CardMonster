using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �鶯
/// ��ǰ�ſ���ʹ��Զ�̣��ں��ſ���ʹ�ý�ս��
/// </summary>
public class Immovable : SkillInBattle
{
    [TriggerEffect("^InRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        GameObject effectTarget = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Enemy)
            {
                effectTarget = systemPlayerData.monsterGameObjectArray[0];
            }
        }

        Melee melee = gameObject.GetComponent<Melee>();

        //��Melee.Effect1��дΪָ��Ŀ���Ч��
        IEnumerator Effect(ParameterNode parameterNode)
        {
            Dictionary<string, object> damageParameter = new();
            damageParameter.Add("LaunchedSkill", melee);
            damageParameter.Add("EffectName", "Effect1");
            damageParameter.Add("EffectTarget", effectTarget);
            damageParameter.Add("DamageValue", melee.GetSkillValue());
            damageParameter.Add("DamageType", DamageType.Physics);

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = damageParameter;

            yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
            yield return null;
        }

        string fullName = "Melee.Effect1";

        ParameterNode parameterNode1 = new();
        parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(melee, fullName, parameterNode1, Effect));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غϣ��Լ��ڶ�������λ���Է������й��ޣ��н�ս
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (!gameObject.TryGetComponent<Melee>(out _))
        {
            return false;
        }

        bool isAlly = false;
        bool enemyHasMonster = false;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally && (systemPlayerData.monsterGameObjectArray[1] == gameObject || systemPlayerData.monsterGameObjectArray[2] == gameObject))
            {
                isAlly = true;
            }

            if (systemPlayerData.perspectivePlayer == Player.Enemy && systemPlayerData.monsterGameObjectArray[0] != null)
            {
                enemyHasMonster = true;
            }
        }

        return isAlly && enemyHasMonster;
    }

    [TriggerEffect("^InRoundBattle$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        GameObject effectTarget = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Enemy)
            {
                effectTarget = systemPlayerData.monsterGameObjectArray[0];
            }
        }

        Ranged ranged = gameObject.GetComponent<Ranged>();

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
            yield return null;
        }

        string fullName = "Ranged.Effect1";

        ParameterNode parameterNode1 = new();
        parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(ranged, fullName, parameterNode1, Effect));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǽ����غϣ��Լ���һ��λ���Է������й��ޣ��н�Զ��
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (!gameObject.TryGetComponent<Ranged>(out _))
        {
            return false;
        }

        bool isAlly = false;
        bool enemyHasMonster = false;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally && systemPlayerData.monsterGameObjectArray[0] == gameObject)
            {
                isAlly = true;
            }

            if (systemPlayerData.perspectivePlayer == Player.Enemy && systemPlayerData.monsterGameObjectArray[0] != null)
            {
                enemyHasMonster = true;
            }
        }

        return isAlly && enemyHasMonster;
    }
}
