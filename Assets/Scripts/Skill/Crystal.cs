using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ����ˮ��
/// ���׼�ʱ��������%d��ˮ��
/// </summary>
public class Crystal : SkillInBattle
{
    //[TriggerEffect(@"^Before\.GameAction\.ChangeCrystalAmount$", "Compare1")]
    //public IEnumerator Effect1(ParameterNode parameterNode)
    //{
    //    Dictionary<string, object> parameter = parameterNode.parameter;
    //    int crystalAmount = (int)parameter["CrystalAmount"];

    //    parameter["CrystalAmount"] = crystalAmount + GetSkillValue();

    //    yield break;
    //}

    ///// <summary>
    ///// �ж��׼����ˮ�������׼����Ǳ�����
    ///// </summary>
    //public bool Compare1(ParameterNode parameterNode)
    //{
    //    BattleProcess battleProcess = BattleProcess.GetInstance();

    //    Dictionary<string, object> parameter = parameterNode.parameter;

    //    if (parameter.ContainsKey("LaunchedSkill") && parameter.ContainsKey("EffectName"))
    //    {
    //        object launchedSkill = parameter["LaunchedSkill"];
    //        string effectName = (string)parameter["EffectName"];

    //        if (launchedSkill is GameAction && effectName == "Sacrifice")
    //        {
    //            Dictionary<string, object> parameter2 = parameterNode.Parent.superiorNode.parameter;

    //            int objectBeSacrificedNumber = (int)parameter2["ObjectBeSacrificedNumber"];
    //            Player player = (Player)parameter2["Player"];

    //            if (objectBeSacrificedNumber >= 4 && objectBeSacrificedNumber <= 6)
    //            {
    //                int t = objectBeSacrificedNumber - 4;

    //                for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
    //                {
    //                    PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

    //                    if (systemPlayerData.perspectivePlayer == player)
    //                    {
    //                        if (systemPlayerData.monsterGameObjectArray[t] == gameObject)
    //                        {
    //                            return true;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return false;
    //}
}
