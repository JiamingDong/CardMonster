using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 护甲
/// 25%%几率回避“远程”攻击
/// 受到真实或物理伤害时，减少等量护甲值而非生命值；护甲被破坏时，消灭提供护甲的装备，格挡剩余伤害；
/// </summary>
public class Armor : SkillInBattle
{
    [TriggerEffect(@"^Replace\.GameAction\.HurtMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;

        result.Add("BeReplaced", true);

        yield break;
    }

    /// <summary>
    /// 判断是否是本怪兽，造成伤害的技能是远程
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
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

    [TriggerEffect(@"^Replace\.GameAction\.HurtMonster$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
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

            //Debug.Log(monsterBeHurt == gameObject);

            MonsterInBattle monsterInBattle = monsterBeHurt.GetComponent<MonsterInBattle>();

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

            //画伤害值
            GameObject healthValueChangePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("HealthValueChangePrefab");
            GameObject healthValueChange = Instantiate(healthValueChangePrefab, monsterInBattle.gameObject.transform);
            healthValueChange.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
            GameObject valueCanvas = healthValueChange.transform.GetChild(0).gameObject;
            valueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            healthValueChange.GetComponent<InitHealthValueChangePrefab>().Init("-" + damageValue, "#ff0000");
            yield return new WaitForSecondsRealtime(0.5f);

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", gameAction);
            parameter2.Add("EffectName", "HurtMonster");
            parameter2.Add("SkillName", "armor");
            parameter2.Add("SkillValue", -damageValue);
            parameter2.Add("Source", "GameAction.HurtMonster");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        string fullName = "GameAction.HurtMonster";

        ParameterNode skillNode = parameterNode.Parent.Parent.superiorNode;
        List<ParameterNode> nodeInMethodList = skillNode.nodeInMethodList;

        ParameterNode parameterNode1 = new();
        parameterNode1.superiorNode = skillNode;
        parameterNode1.parameter = parameter;
        parameterNode1.result.Add("ModifiedEffect", this);

        int armorValue = GetSkillValue();
        if (damageValue > armorValue)
        {
            //贯穿需要
            parameterNode1.result.Add("SurplusDamageValue", damageValue - armorValue);
        }

        int index = nodeInMethodList.IndexOf(parameterNode.Parent.Parent);
        nodeInMethodList[index] = parameterNode1;

        yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(gameAction, fullName, parameterNode1, HurtMonster));
        //yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽，伤害类型是物理伤害或真实伤害
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;

        Dictionary<string, object> parameter2 = parameterNode.parameter;
        Dictionary<string, object> result2 = parameterNode.result;
        GameObject monsterBeHurt = (GameObject)parameter2["EffectTarget"];
        DamageType damageType = (DamageType)parameter2["DamageType"];

        if (result.ContainsKey("ModifiedEffect"))
        {
            return false;
        }

        if (result2.ContainsKey("BeReplaced"))
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