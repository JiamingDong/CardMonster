using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ħ��������������
/// �غϽ���ʱ�������ħ��������Ч��
/// </summary>
public class MagicOutburstDerive : SkillInBattle
{
    [TriggerEffect("^AfterRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, object> parameter1 = new();
        parameter1.Add("LaunchedSkill", this);
        parameter1.Add("EffectName", "Effect1");
        parameter1.Add("SkillName", "magic");
        parameter1.Add("Source", "Skill.MagicOutburst.Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter1;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode1));

        Dictionary<string, object> parameter2 = new();
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect1");
        parameter2.Add("SkillName", "magic_outburst_derive");
        parameter2.Add("Source", "Skill.MagicOutburst.Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
        yield return null;
    }

    /// <summary>
    /// �ж��Ƿ��ǶԷ��غ�
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Enemy)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}