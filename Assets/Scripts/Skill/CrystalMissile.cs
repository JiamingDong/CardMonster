using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 水晶飞弹
/// 我方获得水晶时，对随机一个敌人造成获得水晶数量乘以%d的法术伤害。
/// </summary>
public class CrystalMissile : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.ChangeCrystalAmount$",  "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int crystalAmount = (int)parameter["CrystalAmount"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer != player)
            {
                //选取技能目标
                GameObject effectTarget = null;
                for (int j = 0; j < 3; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] != null)
                    {
                        int r = RandomUtils.GetRandomNumber(0, j);
                        effectTarget = systemPlayerData.monsterGameObjectArray[r];
                    }
                }

                Dictionary<string, object> damageParameter = new();
                damageParameter.Add("LaunchedSkill", this);
                damageParameter.Add("EffectName", "Effect1");
                damageParameter.Add("EffectTarget", effectTarget);
                damageParameter.Add("DamageValue", crystalAmount * GetSkillValue());
                damageParameter.Add("DamageType", DamageType.Magic);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
            }
        }

        //yield return null;
    }

    /// <summary>
    /// 判断是否是我方、加水晶、对方有怪兽
    /// </summary>
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
