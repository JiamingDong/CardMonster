using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ħ��
/// ��<ħ��>����ʱ��50%%���ʶԹ����߷���һ��<��ս>����
/// </summary>
public class MagicCounter : SkillInBattle
{
    /// <summary>
    /// <see cref="Melee.Effect1"/>
    /// </summary>
    [TriggerEffect(@"^After\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Melee melee = gameObject.GetComponent<Melee>();

        //��Melee.Effect1��дΪָ��Ŀ���Ч��
        IEnumerator Effect(ParameterNode parameterNode)
        {
            //ѡȡ����Ŀ��
            GameObject effectTarget = skillInBattle.gameObject;

            //�˺�����
            Dictionary<string, object> damageParameter = new();
            //��ǰ����
            damageParameter.Add("LaunchedSkill", melee);
            //Ч������
            damageParameter.Add("EffectName", "Effect1");
            //�ܵ��˺��Ĺ���
            damageParameter.Add("EffectTarget", effectTarget);
            //�˺���ֵ
            damageParameter.Add("DamageValue", melee.GetSkillValue());
            //�˺�����
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
    /// �ж��Ǳ����ޡ��˺���Դ��ħ�������н�ս
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        var parameter = parameterNode.parameter;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        if (monsterBeHurt == gameObject && skillInBattle is Magic && effectName.Equals("Effect1") && skillInBattle.gameObject != null && gameObject.TryGetComponent(out Melee _))
        {
            return true;
        }

        return false;
    }
}
