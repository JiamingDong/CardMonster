using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 兔之狡黠
/// 我方回合结束时：将自身和自身的一个复制洗入我方的牌库，且均失去<兔之狡黠>
/// </summary>
public class RabbitTricky : SkillInBattle
{
    [TriggerEffect("^AfterRoundBattle$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();

        Dictionary<string, string> cardData = new();
        cardData.Add("CardID", "");
        cardData.Add("CardName", "兔宝");
        cardData.Add("CardType", "monster");
        cardData.Add("CardKind", "{\"leftKind\":\"chaos\"}");
        cardData.Add("CardRace", null);
        cardData.Add("CardHP", "7");
        cardData.Add("CardFlags", null);
        cardData.Add("CardSkinID", "151061");
        cardData.Add("CardCost", "2");
        cardData.Add("CardSkill", "{\"magic\":2,\"melee\":1,\"final_doll\":4}");
        cardData.Add("CardEliteSkill", null);

        Dictionary<string, object> parameter = new();
        Dictionary<string, object> parameter2 = new();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            for (int j = 0; j < battleProcess.systemPlayerData[i].monsterGameObjectArray.Length; j++)
            {
                if (battleProcess.systemPlayerData[i].monsterGameObjectArray[j] == gameObject)
                {
                    parameter.Add("Player", battleProcess.systemPlayerData[i].perspectivePlayer);
                    parameter2.Add("Player", battleProcess.systemPlayerData[i].perspectivePlayer);
                    break;
                }
            }
        }

        //将复制品洗入牌库
        parameter.Add("CardData", cardData);

        ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
        parameterNode1.parameter = parameter;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.AddCardToDeck, parameterNode1));

        //自身撤离
        parameter2.Add("CardData", cardData);
        parameter2.Add("EffectTarget", gameObject);
        parameter2.Add("LaunchedSkill", this);
        parameter2.Add("EffectName", "Effect1");

        ParameterNode parameterNode2 = parameterNode.AddNodeInMethod();
        parameterNode2.parameter = parameter2;

        yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.MonsterInBattleToDeck, parameterNode2));
    }

    /// <summary>
    /// 判断是否是己方回合
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == Player.Ally)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
