/// <summary>
/// 网络消息类型枚举
/// </summary>
[System.Serializable]
public enum NetworkMessageType
{
    /// <summary>
    /// 先后手和卡组
    /// </summary>
    PriorityAndDeck,
    /// <summary>
    /// 拖拽手牌到场上
    /// </summary>
    DragHandCard,
    /// <summary>
    /// 发动英雄技能
    /// </summary>
    LaunchHeroSkill,
    /// <summary>
    /// 结束出牌阶段，开始战斗阶段
    /// </summary>
    StartAttackPhase,
    /// <summary>
    /// 技能概率判断的结果
    /// </summary>
    ProbabilityResult
}

/// <summary>
/// 玩家的枚举
/// </summary>
public enum Player
{
    /// <summary>
    /// 我方
    /// </summary>
    Ally,
    /// <summary>
    /// 对方
    /// </summary>
    Enemy
}

/// <summary>
/// 卡牌类型的枚举
/// </summary>
public enum CardKind
{
    /// <summary>
    /// 怪兽
    /// </summary>
    Monster,
    /// <summary>
    /// 消耗品
    /// </summary>
    Consume,
    /// <summary>
    /// 装备
    /// </summary>
    Equipment
}

/// <summary>
/// 伤害类型的枚举
/// </summary>
public enum DamageType
{
    /// <summary>
    /// 法术伤害
    /// </summary>
    Magic,
    /// <summary>
    /// 物理伤害
    /// </summary>
    Physics,
    /// <summary>
    /// 真实伤害
    /// </summary>
    Real
}

/// <summary>
/// 技能类型的枚举
/// </summary>
public enum SkillType
{
    /// <summary>
    /// 基础技能
    /// </summary>
    BasicSkill,
    /// <summary>
    /// 非基础技能
    /// </summary>
    NonbasicSkill,
    /// <summary>
    /// 护甲
    /// </summary>
    Armor
}

/// <summary>
/// 战斗过程所处阶段
/// </summary>
public enum GamePhase
{
    /// <summary>
    /// 游戏开始前
    /// </summary>
    BeforeGameBegin,
    /// <summary>
    /// 回合准备阶段
    /// </summary>
    RoundReady,
    /// <summary>
    /// 回合中
    /// </summary>
    InRound,
    /// <summary>
    /// 回合战斗阶段
    /// </summary>
    RoundBattle,
    /// <summary>
    /// 游戏结束后
    /// </summary>
    AfterGameEnd
}
