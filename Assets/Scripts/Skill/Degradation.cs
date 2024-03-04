using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 恶化
/// 献祭卡牌或自身被献祭（包括从手牌中）时：使所有盟友造成的真实伤害+1，持续到敌方回合结束
/// </summary>
public class Degradation : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.Sacrifice$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    for (int k = 0; k < systemPlayerData.monsterGameObjectArray.Length; k++)
                    {
                        if (systemPlayerData.monsterGameObjectArray[k] != null)
                        {
                            MonsterInBattle monsterInBattle = systemPlayerData.monsterGameObjectArray[k].GetComponent<MonsterInBattle>();

                            Dictionary<string, object> parameter2 = new();
                            parameter2.Add("LaunchedSkill", this);
                            parameter2.Add("EffectName", "Effect1");
                            parameter2.Add("SkillName", "degradation_derive");
                            parameter2.Add("SkillValue", GetSkillValue());
                            parameter2.Add("Source", "Skill.Degradation.Effect1");

                            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                            parameterNode2.parameter = parameter2;

                            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 判断是己方献祭
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
