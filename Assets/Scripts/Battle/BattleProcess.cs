using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Networking.UnityWebRequest;

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
    /// ս����¼
    /// </summary>
    public Text[] combatLogTexts;

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
    /// �ҷ���������
    /// </summary>
    public Queue<NetworkMessage> allyActionQueue;

    /// <summary>
    /// ���������ڵ�
    /// </summary>
    //public ParameterNode root;

    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static BattleProcess instance;

    //private int effectIndex;
    //private int executeEventIndex;

    Dictionary<string, string> skillClassToChinese;

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
        //effectIndex = 0;
        //executeEventIndex = 0;
        //root = new();

        //�������նԷ���Ϣ���߳�
        //SocketTool.acceptMessageThread.Start();

        //��������
        settlementAreaPanel = GameObject.Find("SettlementAreaPanel");

        //ս����¼
        combatLogTexts = new Text[10];
        combatLogTexts[0] = GameObject.Find("CombatLogText1").GetComponent<Text>();
        combatLogTexts[1] = GameObject.Find("CombatLogText2").GetComponent<Text>();
        combatLogTexts[2] = GameObject.Find("CombatLogText3").GetComponent<Text>();
        combatLogTexts[3] = GameObject.Find("CombatLogText4").GetComponent<Text>();
        combatLogTexts[4] = GameObject.Find("CombatLogText5").GetComponent<Text>();
        combatLogTexts[5] = GameObject.Find("CombatLogText6").GetComponent<Text>();
        combatLogTexts[6] = GameObject.Find("CombatLogText7").GetComponent<Text>();
        combatLogTexts[7] = GameObject.Find("CombatLogText8").GetComponent<Text>();
        combatLogTexts[8] = GameObject.Find("CombatLogText9").GetComponent<Text>();
        combatLogTexts[9] = GameObject.Find("CombatLogText10").GetComponent<Text>();

        //������Ϸ��ʼǰ
        gamePhase = GamePhase.BeforeGameBegin;

        //��ʼ���ҷ���Ϣ
        allyPlayerData = new();

        allyPlayerData.actualPlayer = Player.Ally;

        allyPlayerData.perspectivePlayer = Player.Ally;

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

        allyPlayerData.monsterDeck = new();
        allyPlayerData.itemDeck = new();
        allyPlayerData.surplusCrystal = 0;
        allyPlayerData.handMonster = new Dictionary<string, string>[2];
        allyPlayerData.handItem = new Dictionary<string, string>[2];
        allyPlayerData.monsterGameObjectArray = new GameObject[3];

        //��ʼ���Է���Ϣ
        enemyPlayerData = new();

        enemyPlayerData.roundNumber = 0;

        enemyPlayerData.actualPlayer = Player.Enemy;

        enemyPlayerData.perspectivePlayer = Player.Enemy;

        enemyPlayerData.monsterInBattlePanel = new GameObject[3];
        enemyPlayerData.monsterInBattlePanel[0] = GameObject.Find("EnemyMonsterInBattlePanel1");
        enemyPlayerData.monsterInBattlePanel[1] = GameObject.Find("EnemyMonsterInBattlePanel2");
        enemyPlayerData.monsterInBattlePanel[2] = GameObject.Find("EnemyMonsterInBattlePanel3");

        enemyPlayerData.heroSkillGameObject = GameObject.Find("EnemyHeroSkillPrefab");

        enemyPlayerData.surplusMonsterAmountText = GameObject.Find("EnemySurplusMonsterAmountText").GetComponent<Text>();
        enemyPlayerData.surplusItemAmountText = GameObject.Find("EnemySurplusItemAmountText").GetComponent<Text>();
        enemyPlayerData.surplusCrystalText = GameObject.Find("EnemySurplusCrystalText").GetComponent<Text>();

        enemyPlayerData.monsterDeck = new();
        enemyPlayerData.itemDeck = new();
        enemyPlayerData.surplusCrystal = 0;
        enemyPlayerData.handMonster = new Dictionary<string, string>[2];
        enemyPlayerData.handItem = new Dictionary<string, string>[2];
        enemyPlayerData.monsterGameObjectArray = new GameObject[3];

        //��������Ϸ׼���׶μ�������ͼ��
        systemPlayerData = new PlayerData[2];
        systemPlayerData[0] = allyPlayerData;
        systemPlayerData[1] = enemyPlayerData;

        var allSkillConfig = Database.cardMonster.Query("AllSkillConfig", "");
        skillClassToChinese = new();
        foreach (var item in allSkillConfig)
        {
            skillClassToChinese.Add(item["SkillClassName"], item["SkillChineseName"]);
        }
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
        parameterNode2.opportunity = "InRoundReady";

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
        //����������
        RuleEvent ruleEvent = RuleEvent.GetInstance();
        yield return StartCoroutine(ruleEvent.ExecuteEligibleEffect(parameterNode));
        //yield return null;

        //����Ӣ�ۼ��ܣ���ϵͳ���ӽ���ѯ
        for (int i = 0; i < systemPlayerData.Length; i++)
        {
            //��ȡӢ�ۼ���ʱ���ͼ��ܵ��б�
            GameObject heroSkill = systemPlayerData[i].heroSkillGameObject;
            if (heroSkill.TryGetComponent<HeroSkillInBattle>(out var heroSkillInBattle))
            {
                yield return StartCoroutine(heroSkillInBattle.LaunchSkill(parameterNode));
                //yield return null;
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
                    //Debug.Log($"{parameterNode.opportunity}----{monsterInBattle.cardName}");
                    yield return StartCoroutine(monsterInBattle.LaunchSkill(parameterNode));
                    //yield return null;
                }
            }

            //ѯ������Ʒ
            GameObject consumeGameObject = systemPlayerData[i].consumeGameObject;
            if (consumeGameObject != null)
            {
                //Debug.Log($"{parameterNode.opportunity}----ѯ������Ʒ");
                ConsumeInBattle consumeInBattle = consumeGameObject.GetComponent<ConsumeInBattle>();
                yield return StartCoroutine(consumeInBattle.LaunchSkill(parameterNode));
                //yield return null;
            }
        }

        //��������2
        //Debug.Log($"{parameterNode.opportunity}----��������2");
        RuleEvent2 ruleEvent2 = RuleEvent2.GetInstance();
        yield return StartCoroutine(ruleEvent2.ExecuteEligibleEffect(parameterNode));
    }

    /// <summary>
    /// ִ�м���Ч��
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="parameterNode"></param>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public IEnumerator ExecuteEffect(object nodeCreator, string fullName, ParameterNode parameterNode, Func<ParameterNode, IEnumerator> effect)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;

        //�������replace
        ParameterNode parameterNode1 = new();
        parameterNode1.creator = nodeCreator;
        parameterNode1.opportunity = "Replace." + fullName;
        parameterNode1.parameter = parameter;

        parameterNode1.SetParent(parameterNode, ParameterNodeChildType.ReplaceChild);

        yield return StartCoroutine(ExecuteEvent(parameterNode1));

        if (parameterNode1.result.ContainsKey("BeReplaced"))
        {
            Debug.Log("----����----" + fullName);
            yield break;
        }

        ParameterNode parameterNode2 = new();
        parameterNode2.creator = nodeCreator;
        parameterNode2.opportunity = "Before." + fullName;
        parameterNode2.parameter = parameter;

        parameterNode2.SetParent(parameterNode, ParameterNodeChildType.BeforeChild);

        yield return StartCoroutine(ExecuteEvent(parameterNode2));

        //���޷�������ʱ���Ϸ���ͼ��
        if (nodeCreator is SkillInBattle skillInBattle)
        {
            bool isCard = false;
            for (int i = 0; i < systemPlayerData.Length; i++)
            {
                for (int j = 0; j < systemPlayerData[i].monsterGameObjectArray.Length; j++)
                {
                    if (skillInBattle.gameObject == systemPlayerData[i].monsterGameObjectArray[j])
                    {
                        Log($"<color=#00ff00>{skillInBattle.gameObject.GetComponent<MonsterInBattle>().cardName}</color>����<color=#ffff00>{skillClassToChinese[skillInBattle.GetType().Name]}</color>");
                        isCard = true;
                    }
                }

                if (skillInBattle.gameObject == systemPlayerData[i].consumeGameObject)
                {
                    Log($"<color=#00ff00>{skillInBattle.gameObject.GetComponent<ConsumeInBattle>().cardName}</color>����<color=#ffff00>{skillClassToChinese[skillInBattle.GetType().Name]}</color>");
                    isCard = true;
                }
            }

            if (isCard)
            {
                GameObject launchSkillPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("LaunchSkillPrefab");
                GameObject launchSkillInBattle = Instantiate(launchSkillPrefab, skillInBattle.gameObject.transform);
                launchSkillInBattle.GetComponent<Transform>().localPosition = new Vector3(0, 150, 0);
                GameObject skillCanvas = launchSkillInBattle.transform.GetChild(0).gameObject;
                skillCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 1);
                launchSkillInBattle.GetComponent<InitLaunchSkillPrefab>().Init(skillInBattle.GetType().Name, skillInBattle.GetSkillValue());
                yield return new WaitForSecondsRealtime(1);

            }
        }

        ParameterNode parameterNode3 = new();
        parameterNode3.creator = nodeCreator;
        parameterNode3.parameter = parameter;

        parameterNode3.SetParent(parameterNode, ParameterNodeChildType.EffectChild);

        yield return StartCoroutine(effect(parameterNode3));

        ParameterNode parameterNode4 = new();
        parameterNode4.creator = nodeCreator;
        parameterNode4.opportunity = "After." + fullName;
        parameterNode4.parameter = parameter;

        parameterNode4.SetParent(parameterNode, ParameterNodeChildType.AfterChild);

        yield return StartCoroutine(ExecuteEvent(parameterNode4));
    }

    public void Log(string message)
    {
        for (int i = combatLogTexts.Length - 1; i > 0; i--)
        {
            combatLogTexts[i].text = combatLogTexts[i - 1].text;
        }
        combatLogTexts[0].text = message;
    }
}
