using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ƣ�����Ʒ��
/// </summary>
public class HealConsume : SkillInBattle
{
    public HealConsume(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("AfterACardBeUsed")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();


        //�������ܵĹ���
        GameObject monsterOfLaunchingSkill = (GameObject)parameter["monsterOfLaunchingSkill"];
        //Ŀ�곡��λ�õĹ���
        GameObject monsterBeTreat = (GameObject)parameter["targetMonster"];
        //ȷ���������ܵĹ���λ�ã�ȷ���������ܵĹ���������ҵĶԷ����
        int skillValue = (int)parameter["skillValue"];

        //Ŀ����޽���ǰ�����ͷ���
        if (monsterBeTreat == null) yield break;

        //����
        Dictionary<string, object> treatParameter = new Dictionary<string, object>();
        //������ƵĹ���
        treatParameter.Add("monsterOfGenerateTreatment", monsterOfLaunchingSkill);
        //�ܵ����ƵĹ���
        treatParameter.Add("monsterBeTreat", monsterBeTreat);
        //������Ƶļ��ܵ�����
        treatParameter.Add("skillName", "ZhiLiaoXiaoHaoPin");
        //������ֵ
        treatParameter.Add("treatValue", skillValue);

        yield return StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, treatParameter));
        yield return null;

        Debug.Log("ZhiLiaoXiaoHaoPin.Action:���ƺ�");
    }

}