using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������
/// ʹ��ʱ����������һ˳λ���������ޣ���������˳˻ơ������롰��ˡ�״̬����ó˻Ƴ��������١�������м��ܣ����˻��볡�������״̬��
/// </summary>
public class ProlongLife : SkillInBattle
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
        Dictionary<string, string> cardData = monsterInBattle.cardData;
        string cardSkill = cardData["CardSkill"];
        Dictionary<string, int> skillDic = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardSkill);
        skillDic.Remove("prolong_life");

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length - 1; j++)
            {
                GameObject go = systemPlayerData.monsterGameObjectArray[j];
                if (go == gameObject)
                {

                    GameObject target = systemPlayerData.monsterGameObjectArray[j + 1];
                    MonsterInBattle monsterInBattle2 = target.GetComponent<MonsterInBattle>();

                    Dictionary<string, object> parameter2 = new();
                    parameter2.Add("LaunchedSkill", this);
                    parameter2.Add("EffectName", "Effect1");
                    parameter2.Add("SkillName", "prolong_life_derive");
                    parameter2.Add("SkillValue", 0);
                    parameter2.Add("Source", "Skill.ProlongLife.Effect1");
                    parameter2.Add("SkillFromProlongLife", skillDic);

                    ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
                    parameterNode2.parameter = parameter2;

                    yield return battleProcess.StartCoroutine(monsterInBattle2.DoAction(monsterInBattle2.AddSkill, parameterNode2));
                }
            }
        }

        launchMark = 0;
    }

    /// <summary>
    /// �ж��Ƿ�ʹ�õ��Ǵ˿����ҷ���û�й���
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

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

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length - 1; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject && systemPlayerData.monsterGameObjectArray[j + 1] != null)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
