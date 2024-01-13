using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ˮ���ɵ�
/// �ҷ����ˮ��ʱ�������һ��������ɻ��ˮ����������%d�ķ����˺���
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
                //ѡȡ����Ŀ��
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
    /// �ж��Ƿ����ҷ�����ˮ�����Է��й���
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
