using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔弹
/// 远程攻击必定命中，并且无视物理护盾、潜行、嘲讽且附带贯穿效果。
/// 有怪兽因为远程发动护甲、飞翔、物理护盾、潜行、嘲讽时，若是本怪兽的发动的远程，代替之。
/// 远程具有贯穿
/// 护甲、飞翔、物理护盾的参数是从<see cref="GameAction.HurtMonster(ParameterNode)"/>来的
/// 潜行、嘲讽的参数是从<see cref="GameAction.SelectEffectTarget(ParameterNode)"/>来的
/// 虽然键相同，但分开写
/// </summary>
public class MagicShoot : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Armor\.Effect2$", "Compare1")]
    [TriggerEffect(@"^Replace\.Flying\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Shield\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽，造成伤害的技能是远程
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        if (skillInBattle.gameObject == gameObject && skillInBattle is Ranged)
        {
            return true;
        }
        return false;
    }

    [TriggerEffect(@"^Replace\.Aggro\.Effect1$", "Compare2")]
    [TriggerEffect(@"^Replace\.Stealth\.Effect1$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽，选择目标的技能是远程
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        if (skillInBattle.gameObject == gameObject && skillInBattle is Ranged)
        {
            return true;
        }
        return false;
    }

    [TriggerEffect(@"^Replace\.Armor\.Effect1$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        Debug.Log(parameterNode.Parent.creator.GetType().Name);

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
            GameAction gameAction = GameAction.GetInstance();

            Debug.Log(monsterBeHurt == gameObject);

            //画箭头
            yield return battleProcess.StartCoroutine(ArrowUtils.CreateArrow(skillInBattle.gameObject.transform.position, monsterBeHurt.transform.position));
            if (skillInBattle.gameObject.TryGetComponent(out MonsterInBattle monsterInBattle1))
            {
                battleProcess.Log($"<color=#00ff00>{monsterInBattle1.cardName}</color>造成{damageValue}点伤害");
            }
            else if (skillInBattle.gameObject.TryGetComponent(out ConsumeInBattle consumeInBattle))
            {
                battleProcess.Log($"<color=#00ff00>{consumeInBattle.cardName}</color>造成{damageValue}点伤害");
            }
            else if (skillInBattle.gameObject.TryGetComponent(out HeroSkill heroSkill))
            {
                battleProcess.Log($"<color=#00ff00>{heroSkill.heroSkillNameText.text}</color>造成{damageValue}点伤害");
            }

            MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();

            Armor armor = monsterInBattle.GetComponent<Armor>();

            int armorValue = armor.GetSkillValue();

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", gameAction);
            parameter2.Add("EffectName", "HurtMonster");
            parameter2.Add("SkillName", "armor");
            parameter2.Add("SkillValue", -damageValue);
            parameter2.Add("Source", "GameAction.HurtMonster");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

            if (armorValue < damageValue)
            {
                int surplusDamageValue = damageValue - armorValue;

                int currentHp = monsterInBattle.GetCurrentHp();
                monsterInBattle.SetCurrentHp(currentHp - surplusDamageValue);

                if (monsterInBattle.GetCurrentHp() < 1)
                {
                    Dictionary<string, object> destroyParameter = new();
                    destroyParameter.Add("EffectTarget", monsterBeHurt);
                    destroyParameter.Add("Destroyer", skillInBattle.gameObject);

                    ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    parameterNode2.parameter = destroyParameter;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.DestroyMonster, parameterNode2));
                }
            }
            yield return null;
        }

        string fullName = "GameAction.HurtMonster";

        ParameterNode skillNode = parameterNode.Parent.Parent.Parent.superiorNode;
        List<ParameterNode> nodeInMethodList = skillNode.nodeInMethodList;

        ParameterNode parameterNode1 = new();
        parameterNode1.superiorNode = skillNode;
        parameterNode1.parameter = parameter;
        parameterNode1.result.Add("ModifiedEffect", this);

        int index = nodeInMethodList.IndexOf(parameterNode.Parent.Parent.Parent);
        nodeInMethodList[index] = parameterNode1;

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(gameAction, fullName, parameterNode1, HurtMonster));
        yield return null;
    }

    /// <summary>
    /// 判断伤害来源是否是本怪兽，伤害是远程造成的
    /// </summary>
    public bool Compare3(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        string effectName = (string)parameter["EffectName"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }
        if (skillInBattle.gameObject == gameObject && skillInBattle is Ranged && effectName.Equals("Effect1"))
        {
            return true;
        }
        return false;
    }
}
