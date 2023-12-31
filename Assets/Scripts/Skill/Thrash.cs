using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɨ
/// <��ս>���е���ʱ����һ��������ŵ������%d����ʵ�˺�
/// </summary>
public class Thrash : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
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

        //ѡȡ����Ŀ��
        GameObject effectTarget = null;
        if (oppositePlayerData.monsterGameObjectArray[2] == null)
        {
            effectTarget = oppositePlayerData.monsterGameObjectArray[1];
        }
        else
        {
            int r = RandomUtils.GetRandomNumber(1, 2);
            effectTarget = oppositePlayerData.monsterGameObjectArray[r];
        }

        int skillValue = GetSkillValue();

        //�˺�����
        Dictionary<string, object> damageParameter = new();
        //��ǰ����
        damageParameter.Add("LaunchedSkill", this);
        //Ч������
        damageParameter.Add("EffectName", "Effect1");
        //�ܵ��˺��Ĺ���
        damageParameter.Add("EffectTarget", effectTarget);
        //�˺���ֵ
        damageParameter.Add("DamageValue", skillValue);
        //�˺�����
        damageParameter.Add("DamageType", DamageType.Real);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ���ս���Է������й���
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
