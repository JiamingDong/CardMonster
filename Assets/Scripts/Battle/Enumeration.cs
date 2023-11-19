/// <summary>
/// ������Ϣ����ö��
/// </summary>
[System.Serializable]
public enum NetworkMessageType
{
    /// <summary>
    /// �Ⱥ��ֺͿ���
    /// </summary>
    PriorityAndDeck,
    /// <summary>
    /// ��ק���Ƶ�����
    /// </summary>
    DragHandCard,
    /// <summary>
    /// ����Ӣ�ۼ���
    /// </summary>
    LaunchHeroSkill,
    /// <summary>
    /// �������ƽ׶Σ���ʼս���׶�
    /// </summary>
    StartAttackPhase,
    /// <summary>
    /// ���ܸ����жϵĽ��
    /// </summary>
    ProbabilityResult
}

/// <summary>
/// ��ҵ�ö��
/// </summary>
public enum Player
{
    /// <summary>
    /// �ҷ�
    /// </summary>
    Ally,
    /// <summary>
    /// �Է�
    /// </summary>
    Enemy
}

/// <summary>
/// �������͵�ö��
/// </summary>
public enum CardKind
{
    /// <summary>
    /// ����
    /// </summary>
    Monster,
    /// <summary>
    /// ����Ʒ
    /// </summary>
    Consume,
    /// <summary>
    /// װ��
    /// </summary>
    Equipment
}

/// <summary>
/// �˺����͵�ö��
/// </summary>
public enum DamageType
{
    /// <summary>
    /// �����˺�
    /// </summary>
    Magic,
    /// <summary>
    /// �����˺�
    /// </summary>
    Physics,
    /// <summary>
    /// ��ʵ�˺�
    /// </summary>
    Real
}

/// <summary>
/// �������͵�ö��
/// </summary>
public enum SkillType
{
    /// <summary>
    /// ��������
    /// </summary>
    BasicSkill,
    /// <summary>
    /// �ǻ�������
    /// </summary>
    NonbasicSkill,
    /// <summary>
    /// ����
    /// </summary>
    Armor
}

/// <summary>
/// ս�����������׶�
/// </summary>
public enum GamePhase
{
    /// <summary>
    /// ��Ϸ��ʼǰ
    /// </summary>
    BeforeGameBegin,
    /// <summary>
    /// �غ�׼���׶�
    /// </summary>
    RoundReady,
    /// <summary>
    /// �غ���
    /// </summary>
    InRound,
    /// <summary>
    /// �غ�ս���׶�
    /// </summary>
    RoundBattle,
    /// <summary>
    /// ��Ϸ������
    /// </summary>
    AfterGameEnd
}
