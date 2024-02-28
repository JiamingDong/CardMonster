using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϯ
/// ʹ��ʱ����Ŀ��������%d����ʵ�˺�
/// </summary>
public class Damage : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$",  "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //�˺�����
        Dictionary<string, object> damageParameter = new();
        //��ǰ����
        damageParameter.Add("LaunchedSkill", this);
        //Ч������
        damageParameter.Add("EffectName", "Effect1");
        //�ܵ��˺��Ĺ���
        damageParameter.Add("EffectTarget", consumeTarget);
        //�˺���ֵ
        damageParameter.Add("DamageValue", GetSkillValue());
        //�˺�����
        damageParameter.Add("DamageType", DamageType.Real);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = damageParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ�ʹ�õ��Ǵ˿���Ŀ���ǲ��ǵз�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //ʹ�����Ƶ����
        Player player = (Player)parameter["Player"];
        //����Ŀ�����
        Player targetPlayer = (Player)parameter["TargetPlayer"];

        //����Ʒ����
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        if (player == targetPlayer)
        {
            return false;
        }

        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];
        if (consumeTarget == null)
        {
            return false;
        }

        return true;
    }
}