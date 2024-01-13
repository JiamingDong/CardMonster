using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ƣ�����Ʒ��
/// ʹ�ú�ʹĿ�꼺�����޻ָ�%d������ֵ
/// </summary>
public class HealConsume : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        //����
        Dictionary<string, object> treatParameter = new();
        //��ǰ����
        treatParameter.Add("LaunchedSkill", this);
        //Ч������
        treatParameter.Add("EffectName", "Effect1");
        //�ܵ����ƵĹ���
        treatParameter.Add("EffectTarget", consumeTarget);
        //������ֵ
        treatParameter.Add("TreatValue", GetSkillValue());

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = treatParameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.TreatMonster, parameterNode1));
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ�ʹ�õ��Ǵ˿���Ŀ���ǲ����ҷ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        //ʹ�����Ƶ����
        Player player = (Player)parameter["Player"];
        //����Ŀ�����
        Player targetPlayer = (Player)parameter["TargetPlayer"];
        //Ŀ�����
        GameObject consumeTarget = (GameObject)parameter["ConsumeTarget"];

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

        if (player != targetPlayer)
        {
            return false;
        }

        if (consumeTarget == null)
        {
            return false;
        }

        return true;
    }
}