using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����Ϣ��
/// </summary>
[System.Serializable]
[SerializeField]
public class PlayerData
{
    /// <summary>
    /// ʵ�����ĸ���ҵ���Ϣ
    /// </summary>
    public Player actualPlayer;

    /// <summary>
    /// ��ϵͳ�ӽǹ۲죬���ĸ���ҵ���Ϣ
    /// </summary>
    public Player perspectivePlayer;

    /// <summary>
    /// ����
    /// </summary>
    public int roundNumber;

    /// <summary>
    /// �Ƿ����ʹ�����ơ�����Ӣ�ۼ���
    /// </summary>
    public bool canUseHandCard;

    /// <summary>
    /// ���ƹ�������
    /// </summary>
    public GameObject[] handMonsterPanel;
    /// <summary>
    /// ���Ƶ�������
    /// </summary>
    public GameObject[] handItemPanel;
    /// <summary>
    /// ���Ϲ�������
    /// </summary>
    public GameObject[] monsterInBattlePanel;
    /// <summary>
    /// Ӣ�ۼ���
    /// </summary>
    public GameObject heroSkillGameObject;

    /// <summary>
    /// ʣ�������������ʾ�ı�
    /// </summary>
    public Text surplusMonsterAmountText;
    /// <summary>
    /// ʣ�������������ʾ�ı�
    /// </summary>
    public Text surplusItemAmountText;
    /// <summary>
    /// ʣ��ˮ����������ʾ�ı�
    /// </summary>
    public Text surplusCrystalText;
    /// <summary>
    /// �ƿ����
    /// </summary>
    public List<Dictionary<string, string>> monsterDeck;
    //public List<Card> monsterDeck;
    /// <summary>
    /// �ƿ����
    /// </summary>
    public List<Dictionary<string, string>> itemDeck;
    //public List<Card> itemDeck;
    /// <summary>
    /// ʣ��ˮ������
    /// </summary>
    public int surplusCrystal;
    /// <summary>
    /// ���ƹ�������
    /// </summary>
    public Dictionary<string,string>[] handMonster;
    //public Card[] handMonster;
    /// <summary>
    /// ���Ƶ�������
    /// </summary>
    public Dictionary<string, string>[] handItem;
    /// <summary>
    /// ���Ϲ�������
    /// </summary>
    public GameObject[] monsterGameObjectArray;
    /// <summary>
    /// ���ڽ��������Ʒ
    /// </summary>
    public GameObject consumeGameObject;
}
