using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����֮��
/// ʹ��ʱ�����ݳ���������Ӫ������¼��ܣ�ս��-<ս��֮��%s>������-<����֮��%s>,ƽ��-<����֮��%s>������-<����֮��%s>����Ȼ-<����֮��%s>��
/// </summary>
public class FivedragonForce : SkillInBattle
{
    int launchMark = 0;

    public override int AddValue(string source, int value)
    {
        launchMark = 1;

        if (sourceAndValue.ContainsKey(source))
        {
            sourceAndValue[source] += value;
        }
        else
        {
            sourceAndValue.Add(source, value);
        }
        return GetSkillValue();
    }

    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        HashSet<string> set = new();
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                GameObject monsterGameObject = battleProcess.systemPlayerData[i].monsterGameObjectArray[j];

                if (monsterGameObject == gameObject)
                {
                    for (int k = 2; k > -1; k--)
                    {
                        GameObject go = battleProcess.systemPlayerData[i].monsterGameObjectArray[k];
                        if (go != null)
                        {
                            MonsterInBattle monsterInBattle1 = go.GetComponent<MonsterInBattle>();
                            string kind=monsterInBattle1.kind;
                            set.Add(kind);
                        }
                    }

                    goto a;
                }
            }
        }

    a:;

        if (set.Contains("balance"))
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("LaunchedSkill", this);
            parameter.Add("EffectName", "Effect1");
            parameter.Add("SkillName", "force_of_life");
            parameter.Add("SkillValue", GetSkillValue());
            parameter.Add("Source", "Skill.FivedragonForce.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter;
            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        if (set.Contains("war"))
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("LaunchedSkill", this);
            parameter.Add("EffectName", "Effect1");
            parameter.Add("SkillName", "pick_up_weapon");
            parameter.Add("SkillValue", GetSkillValue());
            parameter.Add("Source", "Skill.FivedragonForce.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter;
            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        if (set.Contains("fortune"))
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("LaunchedSkill", this);
            parameter.Add("EffectName", "Effect1");
            parameter.Add("SkillName", "force_of_fate");
            parameter.Add("SkillValue", GetSkillValue());
            parameter.Add("Source", "Skill.FivedragonForce.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter;
            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        if (set.Contains("chaos"))
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("LaunchedSkill", this);
            parameter.Add("EffectName", "Effect1");
            parameter.Add("SkillName", "power_of_arcane");
            parameter.Add("SkillValue", GetSkillValue());
            parameter.Add("Source", "Skill.FivedragonForce.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter;
            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        if (set.Contains("nature"))
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("LaunchedSkill", this);
            parameter.Add("EffectName", "Effect1");
            parameter.Add("SkillName", "rain_of_arrows");
            parameter.Add("SkillValue", GetSkillValue());
            parameter.Add("Source", "Skill.FivedragonForce.Effect1");

            ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
            parameterNode1.parameter = parameter;
            yield return battleProcess.StartCoroutine(monsterInBattle.DoAction(monsterInBattle.AddSkill, parameterNode1));
        }

        launchMark = 0;
    }

    /// <summary>
    /// �ж��Ƿ�ʹ�õ��Ǵ˿�
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;

        if (launchMark < 1)
        {
            return false;
        }

        //����
        if (result.ContainsKey("MonsterBeGenerated"))
        {
            GameObject monsterBeGenerated = (GameObject)result["MonsterBeGenerated"];
            if (monsterBeGenerated != gameObject)
            {
                return false;
            }
        }
        //װ��
        else if (result.ContainsKey("MonsterBeEquipped"))
        {
            GameObject monsterBeEquipped = (GameObject)result["MonsterBeEquipped"];
            if (monsterBeEquipped != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}