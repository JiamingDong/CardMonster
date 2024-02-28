using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ج������
/// <����>������������25%%��
/// </summary>
public class HeroNightmareGrow : SkillInBattle
{
    [TriggerEffect(@"^After\.MonsterInBattle\.AddSkill$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;

        Entangle entangle = monsterInBattle.GetComponent<Entangle>();
        entangle.probabilityParts.Add("Skill.HeroNightmareGrow.Effect1", 25);

        yield break;
    }

    /// <summary>
    /// �жϹ����Ǽ������ޣ������ǡ�������
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        MonsterInBattle monsterInBattle = (MonsterInBattle)parameterNode.creator;
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (!skillName.Equals("entangle"))
        {
            return false;
        }

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
                if (playerData.monsterGameObjectArray[j] == monsterInBattle)
                {
                    targetPlayer = playerData.perspectivePlayer;
                }
            }
        }

        if (thisPlayer != targetPlayer)
        {
            return false;
        }

        if (!parameterNode.Parent.EffectChild.result.ContainsKey("AddNewSkill"))
        {
            return false;
        }

        return true;
    }
}
