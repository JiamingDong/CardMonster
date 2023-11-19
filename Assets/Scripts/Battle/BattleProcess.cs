using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��Ϸս������
/// </summary>
public class BattleProcess : MonoBehaviour
{
    /// <summary>
    /// ����Ʒ��������
    /// </summary>
    public GameObject settlementAreaPanel;

    /// <summary>
    /// ��Ϸ�����׶�
    /// </summary>
    public GamePhase gamePhase;

    /// <summary>
    /// �ҷ������Ϣ
    /// </summary>
    public PlayerData allyPlayerData;
    /// <summary>
    /// �Է������Ϣ
    /// </summary>
    public PlayerData enemyPlayerData;

    /// <summary>
    /// ��һ���غ���˭�Ļغ�
    /// </summary>
    public Player nextTurn;

    /// <summary>
    /// ϵͳ�ӽ������Ϣ����һ��Ϊ�ӽ�������ң��ڶ���Ϊ�ӽǶ�������
    /// </summary>
    public PlayerData[] systemPlayerData;

    /// <summary>
    /// ���������ڵ�
    /// </summary>
    public ParameterNode root;

    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static BattleProcess instance;

    /// <summary>
    /// ��ñ���ʵ��
    /// </summary>
    /// <returns></returns>
    public static BattleProcess GetInstance()
    {
        if (instance == null)
            instance = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
        return instance;
    }

    /// <summary>
    /// ��ʼ��˫����Ϣ
    /// </summary>
    private void Awake()
    {
        //Assembly assembly = Assembly.GetExecutingAssembly();
        //Debug.Log(assembly.GetName().Name);
        //Debug.Log(assembly.GetName().Version);
        //Debug.Log(assembly.GetName().CultureName);
        //Debug.Log(assembly.GetName().CultureInfo.Name);
        //Debug.Log(assembly.GetName().GetPublicKeyToken());

        root = new();

        //��������
        settlementAreaPanel = GameObject.Find("SettlementAreaPanel");

        //������Ϸ��ʼǰ
        gamePhase = GamePhase.BeforeGameBegin;

        //��ʼ���ҷ���Ϣ
        allyPlayerData = new();

        allyPlayerData.actualPlayer = Player.Ally;

        allyPlayerData.roundNumber = 0;

        allyPlayerData.handMonsterPanel = new GameObject[2];
        allyPlayerData.handMonsterPanel[0] = GameObject.Find("AllyHandMonsterPanel1");
        allyPlayerData.handMonsterPanel[1] = GameObject.Find("AllyHandMonsterPanel2");

        allyPlayerData.handItemPanel = new GameObject[2];
        allyPlayerData.handItemPanel[0] = GameObject.Find("AllyHandItemPanel1");
        allyPlayerData.handItemPanel[1] = GameObject.Find("AllyHandItemPanel2");

        allyPlayerData.monsterInBattlePanel = new GameObject[3];
        allyPlayerData.monsterInBattlePanel[0] = GameObject.Find("AllyMonsterInBattlePanel1");
        allyPlayerData.monsterInBattlePanel[1] = GameObject.Find("AllyMonsterInBattlePanel2");
        allyPlayerData.monsterInBattlePanel[2] = GameObject.Find("AllyMonsterInBattlePanel3");

        allyPlayerData.heroSkillGameObject = GameObject.Find("AllyHeroSkillPrefab");

        allyPlayerData.surplusMonsterAmountText = GameObject.Find("AllySurplusMonsterAmountText").GetComponent<Text>();
        allyPlayerData.surplusItemAmountText = GameObject.Find("AllySurplusItemAmountText").GetComponent<Text>();
        allyPlayerData.surplusCrystalText = GameObject.Find("AllySurplusCrystalText").GetComponent<Text>();

        allyPlayerData.monsterDeck = new List<Dictionary<string, string>>();
        allyPlayerData.itemDeck = new List<Dictionary<string, string>>();
        allyPlayerData.surplusCrystal = 0;
        allyPlayerData.handMonster = new Dictionary<string, string>[2];
        allyPlayerData.handItem = new Dictionary<string, string>[2];
        allyPlayerData.monsterGameObjectArray = new GameObject[3];

        //��ʼ���Է���Ϣ
        enemyPlayerData = new();

        enemyPlayerData.roundNumber = 0;

        enemyPlayerData.actualPlayer = Player.Enemy;

        enemyPlayerData.monsterInBattlePanel = new GameObject[3];
        enemyPlayerData.monsterInBattlePanel[0] = GameObject.Find("EnemyMonsterInBattlePanel1");
        enemyPlayerData.monsterInBattlePanel[1] = GameObject.Find("EnemyMonsterInBattlePanel2");
        enemyPlayerData.monsterInBattlePanel[2] = GameObject.Find("EnemyMonsterInBattlePanel3");

        enemyPlayerData.heroSkillGameObject = GameObject.Find("EnemyHeroSkillPrefab");

        enemyPlayerData.surplusMonsterAmountText = GameObject.Find("EnemySurplusMonsterAmountText").GetComponent<Text>();
        enemyPlayerData.surplusItemAmountText = GameObject.Find("EnemySurplusItemAmountText").GetComponent<Text>();
        enemyPlayerData.surplusCrystalText = GameObject.Find("EnemySurplusCrystalText").GetComponent<Text>();

        enemyPlayerData.monsterDeck = new List<Dictionary<string, string>>();
        enemyPlayerData.itemDeck = new List<Dictionary<string, string>>();
        enemyPlayerData.surplusCrystal = 0;
        enemyPlayerData.handMonster = new Dictionary<string, string>[2];
        enemyPlayerData.handItem = new Dictionary<string, string>[2];
        enemyPlayerData.monsterGameObjectArray = new GameObject[3];

        //��������Ϸ׼���׶μ�������ͼ��
        systemPlayerData = new PlayerData[2];
        systemPlayerData[0] = allyPlayerData;
        systemPlayerData[1] = enemyPlayerData;
    }

    void Start()
    {
        StartCoroutine(EnterGame());
    }

    /// <summary>
    /// ������Ϸ��ִ��
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnterGame()
    {
        //��Ϸ׼��
        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "GameReady";

        yield return StartCoroutine(ExecuteEvent(parameterNode1));

        //��ʼ��Ϸ��һ���غ�
        ParameterNode parameterNode2 = new();
        parameterNode2.opportunity = "EnterRoundReady";

        yield return StartCoroutine(ExecuteEvent(parameterNode2));
    }

    /// <summary>
    /// ����ʱ������������
    /// </summary>
    /// <param name="condition">��������</param>
    /// <param name="parameter">Ч������</param>
    /// <returns></returns>
    public IEnumerator ExecuteEvent(ParameterNode parameterNode)
    {
        yield return null;

        //����������
        RuleEvent ruleEvent = RuleEvent.GetInstance();
        //Debug.Log(parameterNode);
        yield return StartCoroutine(ruleEvent.ExecuteEligibleEffect(parameterNode));
        yield return null;

        //����Ӣ�ۼ��ܣ���ϵͳ���ӽ���ѯ
        for (int i = 0; i < systemPlayerData.Length; i++)
        {
            //��ȡӢ�ۼ���ʱ���ͼ��ܵ��б�
            GameObject heroSkill = systemPlayerData[i].heroSkillGameObject;
            if (heroSkill.TryGetComponent<HeroSkillInBattle>(out var heroSkillInBattle))
            {
                yield return StartCoroutine(heroSkillInBattle.LaunchSkill(parameterNode));
                yield return null;
            }
        }

        //�������Ƽ��ܣ���ϵͳ���ӽ���ѯ���
        for (int i = 0; i < systemPlayerData.Length; i++)
        {
            //δ���ǰ���������������
            //ѯ�����й����Ƿ񷢶�����
            for (int j = 2; j > -1; j--)
            {
                GameObject monsterGameObject = systemPlayerData[i].monsterGameObjectArray[j];
                if (monsterGameObject != null)
                {
                    MonsterInBattle monsterInBattle = monsterGameObject.GetComponent<MonsterInBattle>();
                    yield return StartCoroutine(monsterInBattle.LaunchSkill(parameterNode));
                    yield return null;
                }
            }

            //ѯ������Ʒ
            GameObject consumeGameObject = systemPlayerData[i].consumeGameObject;
            if (consumeGameObject != null)
            {
                ConsumeInBattle consumeInBattle = consumeGameObject.GetComponent<ConsumeInBattle>();
                yield return StartCoroutine(consumeInBattle.LaunchSkill(parameterNode));
                yield return null;
            }
        }

    }

    /// <summary>
    /// �л�ϵͳ�ӽ�
    /// </summary>
    public void SwitchSystemPerspective()
    {
        if (nextTurn == Player.Ally)
        {
            allyPlayerData.perspectivePlayer = Player.Ally;
            enemyPlayerData.perspectivePlayer = Player.Enemy;
        }
        else if (nextTurn == Player.Enemy)
        {
            allyPlayerData.perspectivePlayer = Player.Enemy;
            enemyPlayerData.perspectivePlayer = Player.Ally;
        }
    }

    /// <summary>
    /// ִ�м���Ч��
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="parameterNode"></param>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public IEnumerator ExecuteEffect(object nodeCreator, Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter, string fullName)
    {
        BattleProcess battleProcess = GetInstance();

        //ִ��Ч��ǰ����ִ�еĴ���
        Debug.Log(fullName + "----��ʼ");

        //��ֹ�������antiReplace
        ParameterNode parameterNode1 = new();
        parameterNode1.creator = nodeCreator;
        parameterNode1.opportunity = "AntiReplace." + fullName;
        //parameterNode1.condition.Add("NodeCreator", nodeCreator);

        List<string> antiReplaceReason = new();
        parameterNode1.parameter.Add("AntiReplaceReason", antiReplaceReason);
        parameterNode1.parameter.Add("ReplaceEffectName", fullName);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        //Debug.Log(fullName + "----1");

        //�������replace
        ParameterNode parameterNode2 = new();
        parameterNode2.creator = nodeCreator;
        parameterNode2.opportunity = "Replace." + fullName;
        //parameterNode2.condition.Add("NodeCreator", nodeCreator);

        List<string> replaceReason = new();
        parameterNode2.parameter.Add("ReplaceReason", replaceReason);
        parameterNode2.parameter.Add("AntiReplaceReason", antiReplaceReason);
        parameterNode2.parameter.Add("ReplaceEffectName", fullName);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode2));

        yield return null;

        if (replaceReason.Count > 0)
        {
            Debug.Log(fullName + "----����");
            yield break;
        }

        //Debug.Log(fullName + "----2");

        //ִ��Ч��ǰ�����ļ���before
        ParameterNode parameterNode3 = new();
        parameterNode3.creator = nodeCreator;
        parameterNode3.opportunity = "Before." + fullName;
        //parameterNode3.condition.Add("NodeCreator", nodeCreator);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode3));
        yield return null;

        //Debug.Log(fullName + "----3");

        //�Ƿ��޸�Ч��modify
        ParameterNode parameterNode4 = new();
        parameterNode3.creator = nodeCreator;
        parameterNode4.opportunity = "Modify." + fullName;
        //parameterNode4.condition.Add("NodeCreator", nodeCreator);

        List<string> modifyReason = new();
        List<string> antiModifyReason = new();
        parameterNode4.parameter.Add("ModifyReason", modifyReason);
        parameterNode4.parameter.Add("AntiModifyReason", antiModifyReason);
        parameterNode4.parameter.Add("ModifyEffectName", fullName);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode4));

        yield return null;

        if (modifyReason.Count > 0)
        {
            Debug.Log(fullName + "----�޸�");
        }
        //������޸�Ч����������ִ��
        else
        {
            ParameterNode parameterNode5 = new();
            parameterNode5.creator = nodeCreator;
            parameterNode5.parameter = parameter;

            yield return StartCoroutine(effect(parameterNode5));
            yield return null;
        }

        //Debug.Log(fullName + "----4");

        //ִ��Ч���󴥷��ļ���after
        ParameterNode parameterNode6 = new();
        parameterNode6.creator = nodeCreator;
        parameterNode6.opportunity = "After." + fullName;
        //parameterNode5.condition.Add("NodeCreator", nodeCreator);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode6));
        yield return null;

        Debug.Log(fullName + "----����");
    }
}
