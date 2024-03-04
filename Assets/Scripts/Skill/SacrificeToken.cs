using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 呼唤
/// 献祭卡牌时：在我方1号位召唤织法魔像（其具有2点生命、<魔法1>和<魔力增幅1>）。
/// </summary>
public class SacrificeToken : SkillInBattle
{
    [TriggerEffect(@"^Before\.GameAction\.Sacrifice$", "Compare1")]
    public IEnumerator Effect1(ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        GameAction gameAction = GameAction.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
            {
                if (systemPlayerData.monsterGameObjectArray[j] == gameObject)
                {
                    Dictionary<string, string> cardData = new();
                    cardData.Add("CardID", "");
                    cardData.Add("CardName", "织法魔像");
                    cardData.Add("CardType", "monster");
                    cardData.Add("CardKind", "{\"leftKind\":\"all\"}");
                    cardData.Add("CardRace", null);
                    cardData.Add("CardHP", "2");
                    cardData.Add("CardFlags", null);
                    cardData.Add("CardSkinID", "800401");
                    cardData.Add("CardCost", "2");
                    cardData.Add("CardSkill", "{\"magic\":1,\"magic_outburst\":1}");
                    cardData.Add("CardEliteSkill", null);

                    Dictionary<string, object> parameter1 = new();
                    parameter1.Add("Player", systemPlayerData.perspectivePlayer);
                    parameter1.Add("BattlePanelNumber", 0);
                    parameter1.Add("TargetPlayer", systemPlayerData.perspectivePlayer);
                    parameter1.Add("CardData", cardData);

                    ParameterNode parameterNode1 = parameterNode.AddNodeInMethod();
                    parameterNode1.parameter = parameter1;

                    yield return battleProcess.StartCoroutine(gameAction.DoAction(gameAction.UseACard, parameterNode1));

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 判断是己方献祭，场上有空位
    /// </summary>
    public bool Compare1(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        Player player = (Player)parameter["Player"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < battleProcess.systemPlayerData.Length; i++)
        {
            PlayerData systemPlayerData = battleProcess.systemPlayerData[i];

            if (systemPlayerData.perspectivePlayer == player)
            {
                for (int j = 0; j < systemPlayerData.monsterGameObjectArray.Length; j++)
                {
                    if (systemPlayerData.monsterGameObjectArray[j] == gameObject && systemPlayerData.monsterGameObjectArray[2] == null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}