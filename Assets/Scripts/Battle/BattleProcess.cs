using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 游戏战斗进程
/// </summary>
public class BattleProcess : MonoBehaviour
{
    /// <summary>
    /// 消耗品结算区域
    /// </summary>
    public GameObject settlementAreaPanel;

    /// <summary>
    /// 游戏所处阶段
    /// </summary>
    public GamePhase gamePhase;

    /// <summary>
    /// 我方玩家信息
    /// </summary>
    public PlayerData allyPlayerData;
    /// <summary>
    /// 对方玩家信息
    /// </summary>
    public PlayerData enemyPlayerData;

    /// <summary>
    /// 下一个回合是谁的回合
    /// </summary>
    public Player nextTurn;

    /// <summary>
    /// 系统视角玩家信息，第一个为视角所在玩家，第二个为视角对面的玩家
    /// </summary>
    public PlayerData[] systemPlayerData;

    /// <summary>
    /// 参数树根节点
    /// </summary>
    public ParameterNode root;

    /// <summary>
    /// 本类实例
    /// </summary>
    private static BattleProcess instance;

    /// <summary>
    /// 获得本类实例
    /// </summary>
    /// <returns></returns>
    public static BattleProcess GetInstance()
    {
        if (instance == null)
            instance = GameObject.Find("BattleProcessSystem").GetComponent<BattleProcess>();
        return instance;
    }

    /// <summary>
    /// 初始化双方信息
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

        //结算区域
        settlementAreaPanel = GameObject.Find("SettlementAreaPanel");

        //处于游戏开始前
        gamePhase = GamePhase.BeforeGameBegin;

        //初始化我方信息
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

        //初始化对方信息
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

        //方便在游戏准备阶段加载手牌图像
        systemPlayerData = new PlayerData[2];
        systemPlayerData[0] = allyPlayerData;
        systemPlayerData[1] = enemyPlayerData;
    }

    void Start()
    {
        StartCoroutine(EnterGame());
    }

    /// <summary>
    /// 进入游戏后执行
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnterGame()
    {
        //游戏准备
        ParameterNode parameterNode1 = new();
        parameterNode1.opportunity = "GameReady";

        yield return StartCoroutine(ExecuteEvent(parameterNode1));

        //开始游戏第一个回合
        ParameterNode parameterNode2 = new();
        parameterNode2.opportunity = "EnterRoundReady";

        yield return StartCoroutine(ExecuteEvent(parameterNode2));
    }

    /// <summary>
    /// 根据时机，触发技能
    /// </summary>
    /// <param name="condition">条件参数</param>
    /// <param name="parameter">效果参数</param>
    /// <returns></returns>
    public IEnumerator ExecuteEvent(ParameterNode parameterNode)
    {
        yield return null;

        //发动规则技能
        RuleEvent ruleEvent = RuleEvent.GetInstance();
        //Debug.Log(parameterNode);
        yield return StartCoroutine(ruleEvent.ExecuteEligibleEffect(parameterNode));
        yield return null;

        //发动英雄技能，从系统主视角轮询
        for (int i = 0; i < systemPlayerData.Length; i++)
        {
            //获取英雄技能时机和技能的列表
            GameObject heroSkill = systemPlayerData[i].heroSkillGameObject;
            if (heroSkill.TryGetComponent<HeroSkillInBattle>(out var heroSkillInBattle))
            {
                yield return StartCoroutine(heroSkillInBattle.LaunchSkill(parameterNode));
                yield return null;
            }
        }

        //发动卡牌技能，从系统主视角轮询玩家
        for (int i = 0; i < systemPlayerData.Length; i++)
        {
            //未解决前方怪兽死亡的情况
            //询问所有怪兽是否发动技能
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

            //询问消耗品
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
    /// 切换系统视角
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
    /// 执行技能效果
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="parameterNode"></param>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public IEnumerator ExecuteEffect(object nodeCreator, Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter, string fullName)
    {
        BattleProcess battleProcess = GetInstance();

        //执行效果前必须执行的代码
        Debug.Log(fullName + "----开始");

        //阻止替代技能antiReplace
        ParameterNode parameterNode1 = new();
        parameterNode1.creator = nodeCreator;
        parameterNode1.opportunity = "AntiReplace." + fullName;
        //parameterNode1.condition.Add("NodeCreator", nodeCreator);

        List<string> antiReplaceReason = new();
        parameterNode1.parameter.Add("AntiReplaceReason", antiReplaceReason);
        parameterNode1.parameter.Add("ReplaceEffectName", fullName);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode1));

        //Debug.Log(fullName + "----1");

        //替代技能replace
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
            Debug.Log(fullName + "----代替");
            yield break;
        }

        //Debug.Log(fullName + "----2");

        //执行效果前触发的技能before
        ParameterNode parameterNode3 = new();
        parameterNode3.creator = nodeCreator;
        parameterNode3.opportunity = "Before." + fullName;
        //parameterNode3.condition.Add("NodeCreator", nodeCreator);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode3));
        yield return null;

        //Debug.Log(fullName + "----3");

        //是否修改效果modify
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
            Debug.Log(fullName + "----修改");
        }
        //如果不修改效果，就正常执行
        else
        {
            ParameterNode parameterNode5 = new();
            parameterNode5.creator = nodeCreator;
            parameterNode5.parameter = parameter;

            yield return StartCoroutine(effect(parameterNode5));
            yield return null;
        }

        //Debug.Log(fullName + "----4");

        //执行效果后触发的技能after
        ParameterNode parameterNode6 = new();
        parameterNode6.creator = nodeCreator;
        parameterNode6.opportunity = "After." + fullName;
        //parameterNode5.condition.Add("NodeCreator", nodeCreator);

        yield return StartCoroutine(battleProcess.ExecuteEvent(parameterNode6));
        yield return null;

        Debug.Log(fullName + "----结束");
    }
}
