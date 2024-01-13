using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ǹ���
/// ��ÿ�ֻ������ܸ���һ�Ρ���������˺�/ħ��ʱ����Ϊ�����ез����޸�����һ������˺�/ħ����ÿ�ֻ������ܳ���һ�η�������Ϊ���ⷢ�������ᴥ��Ӱ�켼��Ŀ��ļ��ܡ�
/// </summary>
public class CoverageAttack : SkillInBattle
{
    private bool canLanuchMagic = true;
    private bool canLaunchChance = true;

    [TriggerEffect(@"^Replace\.Magic\.Effect1$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        result.Add("BeReplaced", true);

        canLanuchMagic = false;

        //�Է����
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        Magic magic = gameObject.GetComponent<Magic>();

        bool isAdditionalExecute = false;

        //ѡȡ����Ŀ��
        for (int i = 2; i > -1; i--)
        {
            GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[i];

            if (effectTarget == null)
            {
                continue;
            }

            IEnumerator Effect(ParameterNode parameterNode)
            {
                //�˺�����
                Dictionary<string, object> damageParameter = new();
                //��ǰ����
                damageParameter.Add("LaunchedSkill", magic);
                //Ч������
                damageParameter.Add("EffectName", "Effect1");
                //�ܵ��˺��Ĺ���
                damageParameter.Add("EffectTarget", effectTarget);
                //�˺���ֵ
                damageParameter.Add("DamageValue", magic.GetSkillValue());
                //�˺�����
                damageParameter.Add("DamageType", DamageType.Magic);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
                //yield return null;
            }

            string fullName = "Magic.Effect1";

            ParameterNode parameterNode1 = new();
            parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
            if (!isAdditionalExecute)
            {
                isAdditionalExecute = true;
            }
            else
            {
                parameterNode1.result.Add("isAdditionalExecute", true);
            }

            yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(magic, fullName, parameterNode1, Effect));
            //yield return null;
        }
    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        Dictionary<string, object> result1 = parameterNode.result;
        GameObject go = skillInBattle.gameObject;

        if (result1.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (!canLanuchMagic)
        {
            return false;
        }

        if (go != gameObject)
        {
            //Debug.Log("isAdditionalExecute2");
            return false;
        }

        if (result.ContainsKey("isAdditionalExecute"))
        {
            //Debug.Log("isAdditionalExecute1");
            return false;
        }

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
        if (monsterInBattle.kind != "balance")
        {
            return false;
        }

        return true;
    }

    [TriggerEffect(@"^Replace\.Chance\.Effect1$", "Compare2")]
    public IEnumerator Effect2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        Dictionary<string, object> parameter = parameterNode.parameter;
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        result.Add("BeReplaced", true);

        canLaunchChance = false;

        //�Է����
        PlayerData oppositePlayerMessage = null;
        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 2; j > -1; j--)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    oppositePlayerMessage = battleProcess.systemPlayerData[(i + 1) % battleProcess.systemPlayerData.Length];
                    goto end;
                }
            }
        }
    end:;

        Chance chance = gameObject.GetComponent<Chance>();

        bool isAdditionalExecute = false;

        //ѡȡ����Ŀ��
        for (int i = 2; i > -1; i--)
        {
            GameObject effectTarget = oppositePlayerMessage.monsterGameObjectArray[i];

            if (effectTarget == null)
            {
                continue;
            }

            IEnumerator Effect(ParameterNode parameterNode)
            {
                int skillValue = chance.GetSkillValue();

                if (skillValue > 0)
                {
                    skillValue = RandomUtils.GetRandomNumber(1, skillValue);
                }

                //�˺�����
                Dictionary<string, object> damageParameter = new();
                //��ǰ����
                damageParameter.Add("LaunchedSkill", chance);
                //Ч������
                damageParameter.Add("EffectName", "Effect1");
                //�ܵ��˺��Ĺ���
                damageParameter.Add("EffectTarget", effectTarget);
                //�˺���ֵ
                damageParameter.Add("DamageValue", skillValue);
                //�˺�����
                damageParameter.Add("DamageType", DamageType.Real);

                ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                parameterNode1.parameter = damageParameter;

                yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.HurtMonster, parameterNode1));
                yield return null;
            }

            string fullName = "Chance.Effect1";

            ParameterNode parameterNode1 = new();
            parameterNode1.SetParent(new(), ParameterNodeChildType.EffectChild);
            if (!isAdditionalExecute)
            {
                isAdditionalExecute = true;
            }
            else
            {
                parameterNode1.result.Add("isAdditionalExecute", true);
            }

            yield return battleProcess.StartCoroutine(battleProcess.ExecuteEffect(chance, fullName, parameterNode1, Effect));
            yield return null;
        }

    }

    /// <summary>
    /// �ж��Ƿ��Ǳ�����
    /// </summary>
    public bool Compare2(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.result;
        SkillInBattle skillInBattle = (SkillInBattle)parameterNode.creator;
        Dictionary<string, object> result1 = parameterNode.result;
        GameObject go = skillInBattle.gameObject;

        if (result1.ContainsKey("BeReplaced"))
        {
            return false;
        }

        if (!canLaunchChance)
        {
            return false;
        }

        if (go != gameObject)
        {
            return false;
        }

        if (result.ContainsKey("isAdditionalExecute"))
        {
            return false;
        }

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
        if (monsterInBattle.kind != "fortune")
        {
            return false;
        }

        return true;
    }
}
