using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMagicTeach : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.MonsterEnterBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.result;
        GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = monsterBeGenerated.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillName", "magic");
        parameter1.Add("SkillValue", 1);
        parameter1.Add("Source", "Skill.HeroMagicTeach.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
    }

    /// <summary>
    /// 判断是否是己方怪兽，具有魔法
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.result;
        GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        Player? thisPlayer = null;
        Player? targetPlayer = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData playerData = battleProcess.systemPlayerData[i];

            if (playerData.heroSkillGameObject == gameObject)
            {
                thisPlayer = playerData.perspectivePlayer;
            }

            for (int j = 0; j < playerData.monsterGameObjectArray.Length; j++)
            {
                if (playerData.monsterGameObjectArray[j] == monsterBeGenerated && monsterBeGenerated.TryGetComponent<Magic>(out _))
                {
                    targetPlayer = playerData.perspectivePlayer;
                }
            }
        }

        return targetPlayer == thisPlayer;
    }
}