using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��ս
/// Ч��1���ڻغ�ս���У��ԶԷ�1��λ������ɵ��ڼ�����ֵ�������˺�
/// Ч��2�������ս��Ч��1������Ч��Ϊ��Ч��
/// </summary>
public class Melee : SkillInBattle
{
    public Melee(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
        effectList.Add(Effect2);
    }

    [TriggerEffectCondition("InRoundBattle")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //�������ܵĹ���������ҵĶԷ����
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

        //����1��λû���޾ͷ���
        if (oppositePlayerMessage.monsterGameObjectArray[0] == null)
        {
            yield break;
        }

        GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[0];

        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "Melee.Effect1.ChoiceTarget";

        //Dictionary<string, object> targetCondition = new();
        //targetCondition.Add("LaunchedSkill", this);

        Dictionary<string, object> targetParameter = new();
        targetParameter.Add("LaunchedSkill", this);
        targetParameter.Add("SkillTarget", effectTarget);

        parameterNode1.parameter = targetParameter;
        //parameterNode1.condition = targetCondition;

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        //�˺�����
        Dictionary<string, object> damageParameter = new();
        //��ǰ����
        damageParameter.Add("LaunchedSkill", this);
        //Ч������
        damageParameter.Add("EffectName", "Effect1");
        //�ܵ��˺��Ĺ���
        damageParameter.Add("EffectTarget", effectTarget);
        //�˺���ֵ
        damageParameter.Add("DamageValue", GetSKillValue());
        //�˺�����
        damageParameter.Add("DamageType", DamageType.Physics);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, damageParameter));

        yield return null;
    }

    [TriggerEffectCondition("Replace.Melee.Effect1")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();

        List<string> replaceReason = (List<string>)parameter["ReplaceReason"];
        List<string> antiReplaceReason = (List<string>)parameter["AntiReplaceReason"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string replaceEffectName = (string)parameter["ReplaceEffectName"];

        if (skillInBattle != this)
        {
            yield break;
        }

        if (!replaceEffectName.Equals("Effect1"))
        {
            yield break;
        }

        GameObject monsterOfLaunchingSkill = skillInBattle.gameObject;

        int position = -1;//�������ܵĹ���λ��
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == monsterOfLaunchingSkill)
                {
                    position = j;
                    goto end;
                }
            }
        }
    end:;

        if (position != 0 && !antiReplaceReason.Contains("Melee.Effect2") && replaceReason.Count == 0)
        {
            replaceReason.Add("Melee.Effect2");
        }

        yield return null;
    }
}