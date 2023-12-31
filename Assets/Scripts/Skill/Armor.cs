using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// �ܵ���ʵ�������˺�ʱ�����ٵ�������ֵ��������ֵ�����ױ��ƻ�ʱ�������ṩ���׵�װ������ʣ���˺���
/// 25%%���ʻرܡ�Զ�̡�����
/// </summary>
public class Armor : SkillInBattle
{
    [TriggerEffect(@"^Replace\.GameAction\.HurtMonster$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;

        result.Add("BeReplaced", true);

        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�����˺��ļ�����Զ��
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (monsterBeHurt == gameObject && skillInBattle is Ranged)
        {
            int r = RandomUtils.GetRandomNumber(1, 4);
            return r <= 1;
        }
        return false;
    }

    [TriggerEffect(@"^Replace\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        result.Add("BeReplaced", true);

        IEnumerator HurtMonster(ParameterNode parameterNode)
        {
            Dictionary<string, object> parameter = parameterNode.parameter;
            GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
            int damageValue = (int)parameter["DamageValue"];
            SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

            BattleProcess battleProcess = BattleProcess.GetInstance();

            Debug.Log(monsterBeHurt == gameObject);

            //����ͷ
            yield return battleProcess.StartCoroutine(ArrowUtils.CreateArrow(skillInBattle.gameObject.transform.position, monsterBeHurt.transform.position));
            if (skillInBattle.gameObject.TryGetComponent(out MonsterInBattle monsterInBattle1))
            {
                battleProcess.Log($"<color=#00ff00>{monsterInBattle1.cardName}</color>���{damageValue}���˺�");
            }
            else if (skillInBattle.gameObject.TryGetComponent(out ConsumeInBattle consumeInBattle))
            {
                battleProcess.Log($"<color=#00ff00>{consumeInBattle.cardName}</color>���{damageValue}���˺�");
            }
            else if (skillInBattle.gameObject.TryGetComponent(out HeroSkill heroSkill))
            {
                battleProcess.Log($"<color=#00ff00>{heroSkill.heroSkillNameText.text}</color>���{damageValue}���˺�");
            }

            MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", gameAction);
            parameter2.Add("EffectName", "HurtMonster");
            parameter2.Add("SkillName", "armor");
            parameter2.Add("SkillValue", -damageValue);
            parameter2.Add("Source", "GameAction.HurtMonster");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
            yield return null;
        }

        string fullName = "GameAction.HurtMonster";

        ParameterNode skillNode = parameterNode.Parent.Parent.superiorNode;
        List<ParameterNode> nodeInMethodList = skillNode.nodeInMethodList;

        ParameterNode parameterNode1 = new();
        parameterNode1.superiorNode = skillNode;
        parameterNode1.parameter = parameter;
        parameterNode1.result.Add("ModifiedEffect", this);

        int index = nodeInMethodList.IndexOf(parameterNode.Parent.Parent);
        nodeInMethodList[index] = parameterNode1;

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(gameAction, fullName, parameterNode1, HurtMonster));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ��˺������������˺�����ʵ�˺�
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result2 = parameterNode.result;
        GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
        DamageType damageType = (DamageType)parameter["DamageType"];

        if (result2.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (result.ContainsKey("ModifiedEffect"))
        {
            return false;
        }

        if (monsterBeHurt == gameObject && (damageType == DamageType.Physics || damageType == DamageType.Real))
        {
            return true;
        }
        return false;
    }

}