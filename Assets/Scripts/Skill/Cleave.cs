using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 贯穿
/// 破坏敌人护甲时，剩余伤害不会被敌人格挡
/// 替代护甲效果，伤害大于护甲时，造成多余伤害
/// </summary>
public class Cleave : SkillInBattle
{
    [TriggerEffect(@"^Replace\.Armor\.Effect2$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
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
            //yield return null;
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
        //yield return null;
    }

    /// <summary>
    /// 判断伤害来源是否是本怪兽，伤害类型是物理或真实伤害
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Dictionary<string, object> result = parameterNode.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameter["LaunchedSkill"];
        DamageType damageType = (DamageType)parameter["DamageType"];

        if (result.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (skillInBattle.gameObject == gameObject && (damageType == DamageType.Real || damageType == DamageType.Physics))
        {
            return true;
        }
        return false;
    }
}
