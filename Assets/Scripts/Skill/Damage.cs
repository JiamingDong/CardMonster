using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 侵袭
/// 使用时，对目标敌人造成%d点真实伤害
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

        //技能所在物体
        if (consumeGameObject != gameObject)
        {
            yield break;
        }

        //伤害参数
        Dictionary<string, object> damageParameter = new();
        //当前技能
        damageParameter.Add("LaunchedSkill", this);
        //效果名称
        damageParameter.Add("EffectName", "Effect1");
        //受到伤害的怪兽
        damageParameter.Add("EffectTarget", effectTarget);
        //伤害数值
        damageParameter.Add("DamageValue", GetSKillValue());
        //伤害类型
        damageParameter.Add("DamageType", DamageType.Real);

        yield return StartCoroutine(gameAction.DoAction(gameAction.HurtMonster,damageParameter));

        yield return null;
    }

    /// <summary>
    /// 判断是否被使用的是此卡、目标是不是敌方怪兽
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int crystalAmount = (int)parameter["CrystalAmount"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (crystalAmount <= 0)
        {
            return false;
        }

        bool isAlly = false;
        bool enemyHasMonster = false;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        isAlly = true;
                    }
                }
            }
            else if (systemPlayerData.monsterGameObjectArray[0] != null)
            {
                enemyHasMonster = true;
            }
        }

        return isAlly && enemyHasMonster;
    }
}