using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 怪兽在战场上的数据
/// </summary>
public class MonsterInBattle : GameObjectInBattle
{
    public Image NonBasicSkillBackgroundImage;
    public RawImage MonsterCardImage;
    public RawImage EquipmentBackgroundLImage;
    public RawImage EquipmentBackgroundRImage;
    public RawImage EquipmentImage;
    public RawImage LifeImage;
    public Text LifeText;
    public RawImage ArmorImage;
    public Text ArmorText;
    public RawImage ColorImage;
    public Text CostText;
    public Canvas BasicSkillCanvas;
    public Canvas NonbasicSkillCanvas;

    public string id;
    public string cardName;
    public string skinId;
    public string kind;
    public string type;
    private int cost;
    public string race;
    /// <summary>
    /// 原始卡牌数据
    /// </summary>
    public Dictionary<string, string> cardData;

    /// <summary>
    /// 血量上限
    /// </summary>
    public int maxHp;
    /// <summary>
    /// 当前生命值
    /// </summary>
    private int currentHp;
    /// <summary>
    /// 怪兽上的装备
    /// </summary>
    public Dictionary<string, string> equipment;

    public void SetCurrentHp(int hp)
    {
        currentHp = hp;
        LifeText.text = hp.ToString();
    }

    public int GetCurrentHp()
    {
        return currentHp;
    }

    public void SetCost(int cost)
    {
        this.cost = cost;
        CostText.text = cost.ToString();
    }

    public int GetCost()
    {
        return cost;
    }

    void Start()
    {
        //Debug.Log("MonsterInBattle.Start()");
        //if (NonBasicSkillBackgroundImage == null)
        //{
        //    return;
        //}
        NonBasicSkillBackgroundImage.enabled = true;
        NonBasicSkillBackgroundImage.sprite = LoadAssetBundle.cardAssetBundle.LoadAsset<Sprite>("NonBasicSkillBackgroundInBattle");

        MonsterCardImage.enabled = true;
        //StartCoroutine(Utils.SAToRawImage(MonsterCardImage, "/Image/Card/" + skinId + ".jpg"));

        EquipmentBackgroundLImage.enabled = false;
        EquipmentBackgroundLImage.texture = null;

        EquipmentBackgroundRImage.enabled = false;
        EquipmentBackgroundRImage.texture = null;

        EquipmentImage.enabled = false;
        EquipmentImage.texture = null;

        LifeImage.enabled = true;
        LifeImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("LifeInBattle");

        LifeText.enabled = true;

        ArmorImage.enabled = false;
        ArmorImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("ArmorInBattle");

        ArmorText.enabled = false;

        ColorImage.enabled = true;
    }

    /// <summary>
    /// 生成的时候调用
    /// </summary>
    public IEnumerator Generate(Dictionary<string, string> cardData)
    {
        //Debug.Log("MonsterInBattle.GenerateMonster()");
        id = cardData["CardID"];
        cardName = cardData["CardName"];
        type = cardData["CardType"];
        kind = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardData["CardKind"])["leftKind"];
        race = cardData["CardRace"];
        maxHp = Convert.ToInt32(cardData["CardHP"]);
        skinId = cardData["CardSkinID"];
        int cost = Convert.ToInt32(cardData["CardCost"]);
        this.cardData = cardData;

        //equipment = new();

        SetCurrentHp(maxHp);
        SetCost(cost);
        GenerateCardImage();

        Dictionary<string, int> skillConfig = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardData["CardSkill"]);
        foreach (var keyValuePair in skillConfig)
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("LaunchedSkill", this);
            parameter.Add("EffectName", "Generate");
            parameter.Add("SkillName", keyValuePair.Key);
            parameter.Add("SkillValue", keyValuePair.Value);
            parameter.Add("Source", "Monster");

            ParameterNode parameterNode1 = new();
            parameterNode1.parameter = parameter;

            yield return StartCoroutine(DoAction(AddSkill, parameterNode1));
        }
    }

    /// <summary>
    /// 生成卡牌图像
    /// </summary>
    private void GenerateCardImage()
    {
        //Debug.Log("GenerateCardImage---------------");
        //Debug.Log(LifeText.text);
        //LifeText.text = currentHp.ToString();
        CostText.text = cost.ToString();

        ColorImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_" + kind);

        //皮肤
        //Debug.Log("GenerateCardImage():skinId=" + skinId);
        string monsterSkinResPath = Database.cardMonster.Query("AllSkinConfig", " and SkinID='" + skinId + "'")[0]["SkinResPath"];
        StartCoroutine(Utils.SAToRawImage(MonsterCardImage, "/CardImage/" + kind + "/" + monsterSkinResPath + ".png"));

        if (equipment == null)
        {
            EquipmentBackgroundLImage.enabled = false;
            EquipmentBackgroundLImage.texture = null;
            EquipmentBackgroundRImage.enabled = false;
            EquipmentBackgroundRImage.texture = null;
            EquipmentImage.enabled = false;
            EquipmentImage.texture = null;
        }
        //Debug.Log("GenerateCardImage+++++++++++++++");
    }

    /// <summary>
    /// 加载技能的图像和数值
    /// </summary>
    /// <param name="skillName"></param>
    /// <param name="skillValue"></param>
    public void LoadSkillImageValue(string skillName, int skillValue, string type)
    {
        SkillType skillType = SkillUtils.GetSkillType(skillName);

        switch (skillType)
        {
            case SkillType.Armor:
                LoadArmorImageValue(skillValue);
                break;
            case SkillType.BasicSkill:
                LoadBasicSkillImageValue(skillName, skillValue);
                break;
            default:
                LoadNonbasicSkillImageValue(skillName, skillValue, type);
                break;
        }
    }

    /// <summary>
    /// 加载护甲的图像和数值
    /// </summary>
    public void LoadArmorImageValue(int skillValue)
    {
        ArmorImage.enabled = true;
        ArmorText.text = skillValue.ToString();
        ArmorText.enabled = true;
    }

    /// <summary>
    /// 加载基础技能的图像和数值
    /// </summary>
    public void LoadBasicSkillImageValue(string skillName, int skillValue)
    {
        Transform basicSkillCanvasTransform = BasicSkillCanvas.transform;
        int basicSkillPosition = -1;
        for (int i = 0, length = basicSkillCanvasTransform.childCount; i < length; i++)
        {
            if (basicSkillCanvasTransform.GetChild(i).name == skillName)
            {
                basicSkillPosition = i;
                break;
            }
        }
        //小于0且有这个物体，移除物体，后面的物体下移
        if (skillValue < 0 && basicSkillPosition > -1)
        {
            Destroy(basicSkillCanvasTransform.GetChild(basicSkillPosition).gameObject);

            for (int i = basicSkillPosition, length = basicSkillCanvasTransform.childCount; i < length; i++)
            {
                Vector3 vector3 = basicSkillCanvasTransform.GetChild(i).position;
                basicSkillCanvasTransform.GetChild(i).position = new Vector3(vector3.x, vector3.y - 50, vector3.z);
            }
        }
        //大于等于0且没有现成的这个物体，就加一个
        if (skillValue >= 0 && basicSkillPosition == -1)
        {
            GameObject basicSkillInBattlePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("BasicSkillInBattlePrefab");
            GameObject basicSkillInBattle = Instantiate(basicSkillInBattlePrefab, basicSkillCanvasTransform);
            basicSkillInBattle.transform.localPosition = new Vector3(0, basicSkillCanvasTransform.childCount * 50 - 175, 0);
            basicSkillInBattle.transform.name = skillName;

            GameObject canvas = basicSkillInBattle.transform.Find("Canvas").gameObject;
            RectTransform rectTransform = canvas.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(40, 50);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = new Vector3(1, 1, 1);

            GameObject basicSkillImage = canvas.transform.Find("BasicSkillImage").gameObject;
            RawImage rawImage = basicSkillImage.GetComponent<RawImage>();
            rawImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillName);

            GameObject basicSkillText = canvas.transform.Find("BasicSkillText").gameObject;
            Text text = basicSkillText.GetComponent<Text>();
            //基础技能数值为0也要显示
            text.text = skillValue.ToString();
        }
        //大于0且有现成的这个物体
        if (skillValue >= 0 && basicSkillPosition > -1)
        {
            GameObject basicSkillText = basicSkillCanvasTransform.GetChild(basicSkillPosition).Find("Canvas").Find("BasicSkillText").gameObject;
            Text text = basicSkillText.GetComponent<Text>();
            text.text = skillValue.ToString();
        }
    }

    /// <summary>
    /// 加载非基础技能的图像和数值
    /// </summary>
    public void LoadNonbasicSkillImageValue(string skillName, int skillValue, string type)
    {
        //Debug.Log(skillName + " " + skillValue);
        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
        var skillImageName = skillConfig["SkillImageName"];

        Transform nonbasicCanvasTransform = NonbasicSkillCanvas.transform;
        int nonbasicSkillPosition = -1;
        for (int i = 0, length = nonbasicCanvasTransform.childCount; i < length; i++)
        {
            if (nonbasicCanvasTransform.GetChild(i).name == skillName)
            {
                nonbasicSkillPosition = i;
                break;
            }
        }

        switch (type)
        {
            case "state":
                //小于0且有这个物体，移除物体，前面的物体右移，后面的物体左移
                if (skillValue < 0 && nonbasicSkillPosition > -1)
                {
                    Destroy(nonbasicCanvasTransform.GetChild(nonbasicSkillPosition).gameObject);

                    for (int i = 0, length = nonbasicCanvasTransform.childCount; i < length; i++)
                    {
                        Vector3 vector3 = nonbasicCanvasTransform.GetChild(i).position;
                        if (i < nonbasicSkillPosition)
                        {
                            nonbasicCanvasTransform.GetChild(i).position = new Vector3(vector3.x + 20, vector3.y, vector3.z);
                        }
                        else
                        {
                            nonbasicCanvasTransform.GetChild(i).position = new Vector3(vector3.x - 20, vector3.y, vector3.z);
                        }
                    }
                }
                //大于等于0且没有现成的这个物体，就加一个
                if (skillValue >= 0 && nonbasicSkillPosition == -1)
                {
                    //所有的物体向左移动
                    for (int i = 0, length = nonbasicCanvasTransform.childCount; i < length; i++)
                    {
                        Vector3 vector3 = nonbasicCanvasTransform.GetChild(i).localPosition;
                        nonbasicCanvasTransform.GetChild(i).localPosition = new Vector3(vector3.x - 20, 0, 0);
                    }

                    GameObject nonbasicSkillInBattlePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("NonbasicSkillInBattlePrefab");
                    GameObject nonbasicSkillInBattle = Instantiate(nonbasicSkillInBattlePrefab, nonbasicCanvasTransform);
                    nonbasicSkillInBattle.transform.localPosition = new Vector3((nonbasicCanvasTransform.childCount - 1) * 20, 0, 0);
                    nonbasicSkillInBattle.transform.name = skillName;

                    GameObject canvas = nonbasicSkillInBattle.transform.Find("Canvas").gameObject;
                    RectTransform rectTransform = canvas.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(40, 50);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.localScale = new Vector3(1, 1, 1);

                    GameObject nonbasicSkillImage = canvas.transform.Find("IconRawImage").gameObject;
                    RawImage rawImage = nonbasicSkillImage.GetComponent<RawImage>();
                    rawImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillImageName);

                    GameObject nonbasicSkillText = canvas.transform.Find("ValueText").gameObject;
                    Text text = nonbasicSkillText.GetComponent<Text>();
                    text.text = "";
                }
                break;

            case "value":
                //小于等于0且有这个物体，移除物体，前面的物体右移，后面的物体左移
                if (skillValue <= 0 && nonbasicSkillPosition > -1)
                {
                    Destroy(nonbasicCanvasTransform.GetChild(nonbasicSkillPosition).gameObject);

                    for (int i = 0, length = nonbasicCanvasTransform.childCount; i < length; i++)
                    {
                        Vector3 vector3 = nonbasicCanvasTransform.GetChild(i).position;
                        if (i < nonbasicSkillPosition)
                        {
                            nonbasicCanvasTransform.GetChild(i).position = new Vector3(vector3.x + 20, vector3.y, vector3.z);
                        }
                        else
                        {
                            nonbasicCanvasTransform.GetChild(i).position = new Vector3(vector3.x - 20, vector3.y, vector3.z);
                        }
                    }
                }
                //大于0且没有现成的这个物体，就加一个
                if (skillValue > 0 && nonbasicSkillPosition == -1)
                {
                    //所有的物体向左移动
                    for (int i = 0, length = nonbasicCanvasTransform.childCount; i < length; i++)
                    {
                        Vector3 vector3 = nonbasicCanvasTransform.GetChild(i).localPosition;
                        nonbasicCanvasTransform.GetChild(i).localPosition = new Vector3(vector3.x - 20, 0, 0);
                    }

                    GameObject nonbasicSkillInBattlePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("NonbasicSkillInBattlePrefab");
                    GameObject nonbasicSkillInBattle = Instantiate(nonbasicSkillInBattlePrefab, nonbasicCanvasTransform);
                    nonbasicSkillInBattle.transform.localPosition = new Vector3((nonbasicCanvasTransform.childCount - 1) * 20, 0, 0);
                    nonbasicSkillInBattle.transform.name = skillName;

                    GameObject canvas = nonbasicSkillInBattle.transform.Find("Canvas").gameObject;
                    RectTransform rectTransform = canvas.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(40, 50);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.localScale = new Vector3(1, 1, 1);

                    GameObject nonbasicSkillImage = canvas.transform.Find("IconRawImage").gameObject;
                    RawImage rawImage = nonbasicSkillImage.GetComponent<RawImage>();
                    rawImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillImageName);

                    GameObject nonbasicSkillText = canvas.transform.Find("ValueText").gameObject;
                    Text text = nonbasicSkillText.GetComponent<Text>();
                    text.text = skillValue.ToString();
                }
                //大于0且有现成的这个物体
                if (skillValue > 0 && nonbasicSkillPosition > -1)
                {
                    GameObject basicSkillText = nonbasicCanvasTransform.GetChild(nonbasicSkillPosition).Find("Canvas").Find("ValueText").gameObject;
                    Text text = basicSkillText.GetComponent<Text>();
                    text.text = skillValue.ToString();
                }
                break;
        }

    }

    /// <summary>
    /// 怪兽添加装备
    /// </summary>
    /// <param name="equipData">从数据库查询出来的数据格式相同</param>
    public IEnumerator AddEquipment(Dictionary<string, string> equipData)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        yield return battleProcess.StartCoroutine(RemoveEquipment());

        //新装备数据
        equipment = equipData;

        //新装备的图像
        string equipKind = JsonConvert.DeserializeObject<Dictionary<string, string>>(equipment["CardKind"])["leftKind"];
        EquipmentBackgroundLImage.enabled = true;
        EquipmentBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundL" + equipKind);
        EquipmentBackgroundRImage.enabled = true;
        EquipmentBackgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundR" + equipKind);
        EquipmentImage.enabled = true;
        string equipSkinResPath = Database.cardMonster.Query("AllSkinConfig", " and SkinID='" + equipment["CardSkinID"] + "'")[0]["SkinResPath"];
        yield return StartCoroutine(Utils.SAToRawImage(EquipmentImage, "/CardImage/item/" + equipSkinResPath + ".png"));

        //新装备的技能
        if (Convert.ToInt32(equipData["CardHP"]) > 0)
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("LaunchedSkill", this);
            parameter.Add("EffectName", "AddEquipment");
            parameter.Add("SkillName", "armor");
            parameter.Add("SkillValue", Convert.ToInt32(equipData["CardHP"]));
            parameter.Add("Source", "Equipment");

            ParameterNode parameterNode2 = new();
            parameterNode2.parameter = parameter;

            yield return StartCoroutine(DoAction(AddSkill, parameterNode2));
        }

        string cardSkill = equipment["CardSkill"];
        //Debug.Log(cardSkill);
        if (!string.IsNullOrEmpty(cardSkill))
        {
            Dictionary<string, int> cardP = JsonConvert.DeserializeObject<Dictionary<string, int>>(equipment["CardSkill"]);
            foreach (var keyValuePair in cardP)
            {
                Dictionary<string, object> parameter2 = new();
                parameter2.Add("LaunchedSkill", this);
                parameter2.Add("EffectName", "AddEquipment");
                parameter2.Add("SkillName", keyValuePair.Key);
                parameter2.Add("SkillValue", keyValuePair.Value);
                parameter2.Add("Source", "Equipment");

                ParameterNode parameterNode3 = new();
                parameterNode3.parameter = parameter2;

                yield return StartCoroutine(DoAction(AddSkill, parameterNode3));
            }
        }
    }

    public IEnumerator RemoveEquipment()
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        for (int i = 0; i < skillList.Count; i++)
        {
            SkillInBattle skillInBattle = skillList[i];

            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillClassName='" + skillList[i].GetType().Name + "'")[0];
            var skillEnglishName = skillConfig["SkillEnglishName"];

            Dictionary<string, object> parameter1 = new();
            parameter1.Add("LaunchedSkill", this);
            parameter1.Add("EffectName", "RemoveEquipment");
            parameter1.Add("SkillName", skillEnglishName);
            parameter1.Add("Source", "Equipment");

            ParameterNode parameterNode1 = new();
            parameterNode1.parameter = parameter1;

            yield return battleProcess.StartCoroutine(DoAction(DeleteSkillSource, parameterNode1));

            if (skillInBattle == null)
            {
                i--;
            }
        }

        equipment = null;
        ArmorImage.enabled = false;
        ArmorText.enabled = false;
        EquipmentBackgroundLImage.enabled = false;
        EquipmentBackgroundLImage.texture = null;
        EquipmentBackgroundRImage.enabled = false;
        EquipmentBackgroundRImage.texture = null;
        EquipmentImage.enabled = false;
        EquipmentImage.texture = null;
        yield return null;
    }

    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, ParameterNode parameterNode)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();

        string effectName = effect.Method.Name;
        string fullName = "MonsterInBattle." + effectName;

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, fullName, parameterNode, effect));
        yield return null;
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    /// <param name="skillName">技能类名</param>
    /// <param name="skillValue">数值</param>
    /// <param name="source">来源</param>
    public IEnumerator AddSkill(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        int skillValue = (int)parameter["SkillValue"];
        string source = (string)parameter["Source"];
        object launchedSkill = parameter["LaunchedSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        //Debug.Log($"{cardName}添加技能{skillName}数值{skillValue}来源{source}");
        if (launchedSkill is SkillInBattle skill1 && skill1.gameObject != gameObject)
        {
            yield return StartCoroutine(ArrowUtils.CreateArrow(skill1.gameObject.transform.position, gameObject.transform.position));
        }

        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
        var skillClassName = skillConfig["SkillClassName"];

        var skillTypeConfig = Database.cardMonster.Query("SkillTypeConfig", "and SkillName='" + skillName + "'")[0];
        var skillType = skillTypeConfig["SkillType"];

        //先看看是否已有这个技能
        foreach (SkillInBattle skillInCard in skillList)
        {
            if (skillInCard.GetType().Name.Equals(skillClassName))
            {
                skillValue = skillInCard.AddValue(source, skillValue);
                //Debug.Log(skillClassName + "----" + skillValue);
                if (skillValue < 0 || (skillValue < 1 && skillType == "value"))
                {
                    //skillList.Remove(skillInCard);
                    //Destroy(skillInCard);

                    List<string> needRemoveSource = new();
                    foreach (KeyValuePair<string, int> keyValuePair in skillInCard.sourceAndValue)
                    {
                        needRemoveSource.Add(keyValuePair.Key);
                    }

                    foreach (var item in needRemoveSource)
                    {
                        Dictionary<string, object> parameter4 = new();
                        parameter4.Add("LaunchedSkill", this);
                        parameter4.Add("EffectName", "AddSkill");
                        parameter4.Add("SkillName", skillName);
                        parameter4.Add("Source", item);

                        ParameterNode parameterNode4 = new();
                        parameterNode4.parameter = parameter4;

                        yield return battleProcess.StartCoroutine(DoAction(DeleteSkillSource, parameterNode4));
                    }
                }
                LoadSkillImageValue(skillName, skillValue, skillType);
                yield break;
            }
        }

        Type type = Type.GetType(skillClassName);

        if (type != null)
        {
            SkillInBattle skill = (SkillInBattle)gameObject.AddComponent(type);

            skillValue = skill.AddValue(source, skillValue);
            skillList.Add(skill);

            //排序
            skillList.Sort((a, b) => battleProcess.skillPriority[b.GetType().Name].CompareTo(battleProcess.skillPriority[a.GetType().Name]));
        }

        LoadSkillImageValue(skillName, skillValue, skillType);
    }


    /// <summary>
    /// 删除技能的某个来源
    /// </summary>
    /// <param name="skillName">技能类名</param>
    /// <param name="source">来源</param>
    public IEnumerator DeleteSkillSource(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        string source = (string)parameter["Source"];
        object launchedSkill = parameter["LaunchedSkill"];

        BattleProcess battleProcess = BattleProcess.GetInstance();

        if (launchedSkill is SkillInBattle skill1 && skill1.gameObject != gameObject)
        {
            yield return StartCoroutine(ArrowUtils.CreateArrow(skill1.gameObject.transform.position, gameObject.transform.position));
        }

        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillName + "'")[0];
        var skillClassName = skillConfig["SkillClassName"];

        var skillTypeConfig = Database.cardMonster.Query("SkillTypeConfig", "and SkillName='" + skillName + "'")[0];
        var skillType = skillTypeConfig["SkillType"];

        //先看看是否已有这个技能
        foreach (SkillInBattle skillInBattle in skillList)
        {
            if (skillInBattle.GetType().Name.Equals(skillClassName))
            {
                skillInBattle.RemoveValue(source);

                int skillValue = skillInBattle.GetSkillValue();

                LoadSkillImageValue(skillName, skillValue, skillType);

                if (skillValue < 0 || (skillValue < 1 && skillType == "value"))
                {
                    skillList.Remove(skillInBattle);
                    Destroy(skillInBattle);

                    //排序
                    skillList.Sort((a, b) => battleProcess.skillPriority[b.GetType().Name].CompareTo(battleProcess.skillPriority[a.GetType().Name]));
                }

                yield break;
            }
        }
        yield break;
        //yield return null;
    }
}
