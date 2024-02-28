using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 吞噬
/// 消灭一只怪兽后，此卡的【生命上限】【近战】、【远程】、【随机】、【魔法】永久上升%d。（必须有以上技能才会上升。）
/// </summary>
public class Devouring : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.DestroyMonster$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        monsterInBattle.maxHp += GetSkillValue();
        monsterInBattle.SetCurrentHp(monsterInBattle.GetCurrentHp() + GetSkillValue());

        foreach (var basicAttackEffect in SkillUtils.basicAttackEffectSet)
        {
            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + basicAttackEffect + "'")[0];
            var skillClassName = skillConfig["SkillClassName"];

            object[] parameters = { null };

            var mi = typeof(GameObject).GetMethods().Where(method => method.Name == "TryGetComponent");
            MethodInfo methodInfo = null;
            foreach (var item in mi)
            {
                if (item.IsGenericMethod)
                {
                    methodInfo = item.MakeGenericMethod(Type.GetType(skillClassName));
                    break;
                }
            }

            bool hasSkill = (bool)methodInfo.Invoke(gameObject, parameters);
            if (hasSkill)
            {
                Dictionary<string, object> parameter1 = new();
                parameter1.Add("LaunchedSkill", this);
                parameter1.Add("EffectName", "Effect1");
                parameter1.Add("SkillName", basicAttackEffect);
                parameter1.Add("SkillValue", GetSkillValue());
                parameter1.Add("Source", "Skill.Devouring.Effect1");

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = parameter1;

                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
            }
        }
    }

    /// <summary>
    /// 判断是否是本怪兽/消耗品
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        GameObject destroyer = (GameObject)parameter["Destroyer"];
        if (destroyer == gameObject)
        {
            return true;
        }
        return false;
    }
}
