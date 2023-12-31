using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 撤离
/// 此卡（装备此卡的怪兽）在场上被献祭时，将自身洗入我方牌库且费用+%d
/// </summary>
public class Evacuate : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.Sacrifice$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> parameter = new();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, string> kindDictionary = new();
        kindDictionary.Add("leftKind", monsterInBattle.kind);

        Dictionary<string, string> cardData = new();
        cardData.Add("CardID", monsterInBattle.id);
        cardData.Add("CardName", monsterInBattle.cardName);
        cardData.Add("CardType", monsterInBattle.type);
        cardData.Add("CardKind", JsonConvert.SerializeObject(kindDictionary));
        cardData.Add("CardRace", monsterInBattle.race);
        cardData.Add("CardHP", monsterInBattle.maxHp.ToString());
        cardData.Add("CardFlags", "");
        cardData.Add("CardSkinID", monsterInBattle.skinId);
        cardData.Add("CardCost", (monsterInBattle.GetCost() + GetSkillValue()).ToString());
        cardData.Add("CardSkill", JsonConvert.SerializeObject(monsterInBattle.skill));
        cardData.Add("CardEliteSkill", null);
        parameter.Add("CardData", cardData);

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    parameter.Add("Player", battleProcess.systemPlayerData[i].perspectivePlayer);
                    break;
                }
            }
        }

        parameter.Add("EffectTarget", gameObject);
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect1");

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode1));
        yield return null;
    }

    /// <summary>
    /// 判断是否是本怪兽
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        int objectBeSacrificedNumber = (int)parameter["ObjectBeSacrificedNumber"];
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (objectBeSacrificedNumber >= 4 && objectBeSacrificedNumber <= 6)
        {
            int t = objectBeSacrificedNumber - 4;

            for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
            {
                PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

                if (systemPlayerData.perspectivePlayer == player)
                {
                    if (systemPlayerData.monsterGameObjectArray[t] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
