using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϯ
/// ʹ��ʱ����Ŀ��������%d����ʵ�˺�
/// </summary>
public class Damage : SkillInBattle
{
    public Damage(GameObjectInBattle gameObjectInBattle) : base()
    {
        effectList.Add(Effect1);
    }

    [TriggerEffectCondition("After.GameAction.ChangeCrystalAmount", compareMethodName = "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> effectParameter = (Dictionary<string, object>)parameter["EffectParameter"];
        GameObject effectTarget = (GameObject)parameter["TargetMonster"];
        GameObject consumeGameObject = (GameObject)parameter["ConsumeGameObject"];

        //������������
        if (consumeGameObject != gameObject)
        {
            yield break;
        }

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
        damageParameter.Add("DamageType", DamageType.Real);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster,damageParameter));

        yield return null;
    }
}