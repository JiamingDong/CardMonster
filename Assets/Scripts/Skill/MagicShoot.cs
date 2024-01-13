using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ħ��
/// Զ�̹����ض����У��������������ܡ�Ǳ�С������Ҹ����ᴩЧ����
/// �й�����ΪԶ�̷������ס����衢�����ܡ�Ǳ�С�����ʱ�����Ǳ����޵ķ�����Զ�̣�����֮��
/// Զ�̾��йᴩ
/// ���ס����衢�����ܵĲ����Ǵ�<see cref="GameAction.HurtMonster(ParameterNode)"/>����
/// Ǳ�С�����Ĳ����Ǵ�<see cref="GameAction.SelectEffectTarget(ParameterNode)"/>����
/// ��Ȼ����ͬ�����ֿ�д
/// </summary>
public class MagicShoot : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Armor\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Flying\.Effect1$", "Compare1")]
    [TriggerEffect(@"^Replace\.Shield\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        result.Add("BeReplaced", true);
        yield break;
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�����˺��ļ�����Զ��
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
        yield break;
        //yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ����ޣ�ѡ��Ŀ��ļ�����Զ��
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

    [TriggerEffect(@"^Replace\.Armor\.Effect2$", "Compare3")]
    public IEnumerator Effect3(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        int damageValue = (int)parameter["DamageValue"];

        Dictionary<string, object> result2 = parameterNode.Parent.Parent.result;

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        result.Add("BeReplaced", true);
        result2.Add("BeReplaced", true);

        IEnumerator HurtMonster(ParameterNode parameterNode)
        {
            Dictionary<string, object> parameter = parameterNode.parameter;
            GameObject monsterBeHurt = (GameObject)parameter["EffectTarget"];
            int damageValue = (int)parameter["DamageValue"];
            SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];

            MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();

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

            //���˺�ֵ
            GameObject healthValueChangePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("HealthValueChangePrefab");
            GameObject healthValueChange = Instantiate(healthValueChangePrefab, monsterInBattle.gameObject.transform);
            healthValueChange.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
            GameObject valueCanvas = healthValueChange.transform.GetChild(0).gameObject;
            valueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            healthValueChange.GetComponent<InitHealthValueChangePrefab>().Init("-" + damageValue, "#ff0000");
            yield return new WaitForSecondsRealtime(0.5f);

            Armor armor = monsterInBattle.GetComponent<Armor>();

            int armorValue = armor.GetSkillValue();

            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", gameAction);
            parameter1.Add("EffectName", "HurtMonster");
            parameter1.Add("SkillName", "armor");
            parameter1.Add("SkillValue", -damageValue);
            parameter1.Add("Source", "GameAction.HurtMonster");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

            if (armorValue < damageValue)
            {
                int surplusDamageValue = damageValue - armorValue;

                int currentHp = monsterInBattle.GetCurrentHp();
                monsterInBattle.SetCurrentHp(currentHp - surplusDamageValue);

                parameterNode.result.Add("CauseDamageToHealth", true);
                if (surplusDamageValue > currentHp)
                {
                    parameterNode.result.Add("ExcessiveDamage", surplusDamageValue - currentHp);
                }
            }
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
    }

    /// <summary>
    /// �ж��˺���Դ�Ƿ��Ǳ����ޣ��˺���Զ����ɵ�
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
