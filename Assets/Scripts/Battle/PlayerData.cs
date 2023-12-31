using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家信息类
/// </summary>
[System.Serializable]
[SerializeField]
public class PlayerData
{
    /// <summary>
    /// 实际是哪个玩家的信息
    /// </summary>
    public Player actualPlayer;

    /// <summary>
    /// 以系统视角观察，是哪个玩家的信息
    /// </summary>
    public Player perspectivePlayer;

    /// <summary>
    /// 轮数
    /// </summary>
    public int roundNumber;

    /// <summary>
    /// 是否可以使用手牌、发动英雄技能
    /// </summary>
    public bool canUseHandCard;

    /// <summary>
    /// 可以献祭
    /// </summary>
    public bool canSacrifice;

    /// <summary>
    /// 手牌怪兽区域
    /// </summary>
    public GameObject[] handMonsterPanel;
    /// <summary>
    /// 手牌道具区域
    /// </summary>
    public GameObject[] handItemPanel;
    /// <summary>
    /// 场上怪兽区域
    /// </summary>
    public GameObject[] monsterInBattlePanel;
    /// <summary>
    /// 英雄技能
    /// </summary>
    public GameObject heroSkillGameObject;

    /// <summary>
    /// 剩余怪兽数量的显示文本
    /// </summary>
    public Text surplusMonsterAmountText;
    /// <summary>
    /// 剩余道具数量的显示文本
    /// </summary>
    public Text surplusItemAmountText;
    /// <summary>
    /// 剩余水晶数量的显示文本
    /// </summary>
    public Text surplusCrystalText;

    /// <summary>
    /// 剩余水晶数量
    /// </summary>
    public int surplusCrystal;

    /// <summary>
    /// 牌库怪兽
    /// </summary>
    public Queue<Dictionary<string, string>> monsterDeck;
    /// <summary>
    /// 牌库道具
    /// </summary>
    public Queue<Dictionary<string, string>> itemDeck;
    /// <summary>
    /// 手牌怪兽数组
    /// </summary>
    public Dictionary<string,string>[] handMonster;
    /// <summary>
    /// 手牌道具数组
    /// </summary>
    public Dictionary<string, string>[] handItem;
    /// <summary>
    /// 场上怪兽数组
    /// </summary>
    public GameObject[] monsterGameObjectArray;
    /// <summary>
    /// 正在结算的消耗品
    /// </summary>
    public GameObject consumeGameObject;

    /// <summary>
    /// 标记，例如献祭点SacrificeNumber
    /// </summary>
    public Dictionary<string, object> marker;
}
