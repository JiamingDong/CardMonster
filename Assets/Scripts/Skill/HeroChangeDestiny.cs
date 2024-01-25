using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 逆天改命
/// 每局游戏限一次。目标我方怪兽获得“强运”
/// 英雄技能
/// </summary>
public class HeroChangeDestiny : SkillInBattle
{
    int launchedTimes = 0;

    [TriggerEffect("^LaunchHeroSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int targetNumber = (int)parameter["TargetNumber"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        launchedTimes++;

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            if (playerData.perspectivePlayer == Player.Ally)
            {
                GameObject go = battleProcess.systemPlayerData[i].monsterGameObjectArray[targetNumber];
                MonsterInBattle monsterInBattle = go.GetComponent<MonsterInBattle>();

                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "Effect1");
                parameter2.Add("SkillName", "chance_fate");
                parameter2.Add("SkillValue", 0);
                parameter2.Add("Source", "Skill.HeroChangeDestiny.Effect1");

                ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                parameterNode2.parameter = parameter2;
                yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
            }
        }
    }

    /// <summary>
    /// 判断是否是己方发动
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int targetNumber = (int)parameter["TargetNumber"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchedTimes > 0)
        {
            return false;
        }

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            if (playerData.perspectivePlayer == Player.Ally && playerData.heroSkillGameObject == gameObject && playerData.monsterGameObjectArray[targetNumber] != null)
            {
                return true;
            }
        }

        return false;
    }
}