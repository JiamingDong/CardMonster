using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���
/// �ҷ�ս���׶ο�ʼʱ��<��ս>ֵ+%d���������з��غϽ�����ֻ�ܷ���һ��
/// </summary>
public class Charge : SkillInBattle
{
    [TriggerEffect("^BeforeRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        if (gameObject.TryGetComponent<Melee>(out var melee))
        {
            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "Effect1");
            parameter1.Add("SkillName", "melee");
            parameter1.Add("SkillValue", GetSkillValue());
            parameter1.Add("Source", "Skill.Charge.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));

            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", this);
            parameter2.Add("EffectName", "Effect1");
            parameter2.Add("SkillName", "charge_derive");
            parameter2.Add("SkillValue", 0);
            parameter2.Add("Source", "Skill.Charge.Effect1");

            ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
            parameterNode2.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode2));
            yield return null;
        }

        //Dictionary<string, object> parameter4 = new();
        //parameter4.Add("LaunchedSkill", this);
        //parameter4.Add("EffectName", "Effect1");
        //parameter4.Add("SkillName", "melee");
        //parameter4.Add("SkillValue", -GetSkillValue());
        //parameter4.Add("Source", "Skill.Charge.Effect1");

        //ParameterNode parameterNode4 = parameterNode.AddNodeInMethod();
        //parameterNode4.parameter = parameter4;

        //yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode4));
        //yield return null;

        List<string> needRemoveSource = new();
        foreach (KeyValuePair<string, int> keyValuePair in sourceAndValue)
        {
            needRemoveSource.Add(keyValuePair.Key);
        }

        foreach (var item in needRemoveSource)
        {
            Dictionary<string, object> parameter2 = new();
            parameter2.Add("LaunchedSkill", this);
            parameter2.Add("EffectName", "Effect1");
            parameter2.Add("SkillName", "charge");
            parameter2.Add("Source", item);

            ParameterNode parameterNode2 = new();
            parameterNode2.parameter = parameter2;

            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.DeleteSkillSource, parameterNode2));
        }
    }

    /// <summary>
    /// �ж��Ƿ��ǵ�ǰ�غϽ�ɫ
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            if (battleProcess.systemPlayerData[i].perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}