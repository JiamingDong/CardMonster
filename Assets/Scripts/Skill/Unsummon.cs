using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 反召唤
/// 将目标怪兽洗入其拥有者的牌库
/// </summary>
public class Unsummon : SkillInBattle
{
    [TriggerEffect(@"^After\.GameAction\.UseACard$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];

        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        Dictionary<string, object> parameter = new();

        MonsterInBattle monsterInBattle = consumeTarget.GetComponent<MonsterInBattle>();

        Dictionary<string, string> kindDictionary = new();
        kindDictionary.Add("leftKind", monsterInBattle.kind);

        Dictionary<string, string> cardData = new();
        cardData.Add("CardID", monsterInBattle.id);
        cardData.Add("CardName", monsterInBattle.cardName);
        cardData.Add("CardType", monsterInBattle.type);
        cardData.Add("CardKind", JsonConvert.SerializeObject(kindDictionary));
        cardData.Add("CardRace", monsterInBattle.race);
        cardData.Add("CardHP", monsterInBattle.maxHp.ToString());
        cardData.Add("CardFlags", null);
        cardData.Add("CardSkinID", monsterInBattle.skinId);
        cardData.Add("CardCost", monsterInBattle.GetCost().ToString());
        cardData.Add("CardSkill", monsterInBattle.skill);
        cardData.Add("CardEliteSkill", null);
        parameter.Add("CardData", cardData);

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == consumeTarget)
                {
                    parameter.Add("Player", battleProcess.systemPlayerData[i].perspectivePlayer);
                    break;
                }
            }
        }

        parameter.Add("EffectTarget", consumeTarget);
        parameter.Add("LaunchedSkill", this);
        parameter.Add("EffectName", "Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.MonsterInBattleToDeck, parameterNode2));
        yield return null;
    }

    /// <summary>
    /// 判断是否被使用的是此卡
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> result = parameterNode.Parent.EffectChild.nodeInMethodList[1].EffectChild.result;
        GameObject consumeTarget = (GameObject)result["ConsumeTarget"];

        //消耗品物体
        if (result.ContainsKey("ConsumeBeGenerated"))
        {
            GameObject consumeBeGenerated = (GameObject)result["ConsumeBeGenerated"];
            if (consumeBeGenerated != gameObject)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        if (consumeTarget == null)
        {
            return false;
        }

        return true;
    }
}
