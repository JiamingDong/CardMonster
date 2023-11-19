using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ս���ϵ�����
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
    public Canvas BasicSkillCanvas;
    public Canvas NonbasicSkillCanvas;

    public string id;
    public string cardName;
    public string skinID;
    public string kind;
    public string type;
    public int cost;
    public string race;
    public Dictionary<string, int> p;

    /// <summary>
    /// Ѫ������
    /// </summary>
    public int maximumHp;
    /// <summary>
    /// ��ǰ����ֵ
    /// </summary>
    public int currentHp;
    /// <summary>
    /// �����ϵ�װ��
    /// </summary>
    public Dictionary<string, object> equipment;

    void Start()
    {

        NonBasicSkillBackgroundImage.enabled = true;
        NonBasicSkillBackgroundImage.sprite = LoadAssetBundle.cardAssetBundle.LoadAsset<Sprite>("NonBasicSkillBackgroundInBattle");

        MonsterCardImage.enabled = true;
        StartCoroutine(Utils.SAToRawImage(MonsterCardImage, "/Image/Card/" + skinID + ".jpg"));

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
    /// ���ɵ�ʱ�����
    /// </summary>
    public IEnumerator GenerateMonster(Dictionary<string, object> cardData)
    {
        id = (string)cardData["CardID"];
        cardName = (string)cardData["CardName"];
        type = (string)cardData["CardType"];
        kind = (string)cardData["CardKind"];
        race = (string)cardData["CardRace"];
        maximumHp = (int)cardData["CardHP"];
        skinID = (string)cardData["CardSkinID"];
        cost = (int)cardData["CardCost"];
        p = (Dictionary<string, int>)cardData["CardP"];

        equipment = new();

        currentHp = maximumHp;

        GenerateCardImage();

        foreach (var keyValuePair in p)
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("SkillName", keyValuePair.Key);
            parameter.Add("SkillValue", keyValuePair.Value);
            parameter.Add("Source", "Monster");
            yield return StartCoroutine(DoAction(AddSkill, parameter));
            //AddSkill(keyValuePair.Key, keyValuePair.Value, "Monster");
        }
    }

    /// <summary>
    /// ���ɿ���ͼ��
    /// </summary>
    private void GenerateCardImage()
    {
        //Debug.Log("GenerateCardImage---------------");
        //Debug.Log(LifeText.text);
        LifeText.text = currentHp.ToString();

        ColorImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_" + kind);

        //Ƥ��
        string monsterSkinResPath = Database.cardMonster.Query("AllSkinConfig", " and SkinID='" + skinID + "'")[0]["SkinResPath"];
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
    /// ���ؼ��ܵ�ͼ�����ֵ
    /// </summary>
    /// <param name="skillName"></param>
    /// <param name="skillValue"></param>
    public void LoadSkillImageValue(string skillName, int skillValue)
    {
        SkillType skillType = SkillInBattle.GetSkillType(skillName);

        switch (skillType)
        {
            case SkillType.Armor:
                LoadArmorImageValue(skillValue);
                break;
            case SkillType.BasicSkill:
                LoadBasicSkillImageValue(skillName, skillValue);
                break;
            default:
                LoadNonbasicSkillImageValue(skillName, skillValue);
                break;
        }
    }

    /// <summary>
    /// ���ػ��׵�ͼ�����ֵ
    /// </summary>
    public void LoadArmorImageValue(int skillValue)
    {
        if (skillValue < 1)
        {
            ArmorImage.enabled = false;
            ArmorText.enabled = false;
        }
        else
        {
            ArmorImage.enabled = true;
            ArmorText.text = skillValue.ToString();
            ArmorText.enabled = true;
        }
    }

    /// <summary>
    /// ���ػ������ܵ�ͼ�����ֵ
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
        //С��0����������壬�Ƴ����壬�������������
        if (skillValue < 0 && basicSkillPosition > -1)
        {
            Destroy(basicSkillCanvasTransform.GetChild(basicSkillPosition).gameObject);

            for (int i = basicSkillPosition, length = basicSkillCanvasTransform.childCount; i < length; i++)
            {
                Vector3 vector3 = basicSkillCanvasTransform.GetChild(i).position;
                basicSkillCanvasTransform.GetChild(i).position = new Vector3(vector3.x, vector3.y - 50, vector3.z);
            }
        }
        //���ڵ���0��û���ֳɵ�������壬�ͼ�һ��
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
            //����������ֵΪ0ҲҪ��ʾ
            text.text = skillValue.ToString();
        }
        //����0�����ֳɵ��������
        if (skillValue >= 0 && basicSkillPosition > -1)
        {
            GameObject basicSkillText = basicSkillCanvasTransform.GetChild(basicSkillPosition).Find("BasicSkillText").gameObject;
            Text text = basicSkillText.GetComponent<Text>();
            text.text = skillValue.ToString();
        }
    }

    /// <summary>
    /// ���طǻ������ܵ�ͼ�����ֵ
    /// </summary>
    public void LoadNonbasicSkillImageValue(string skillName, int skillValue)
    {
        //Debug.Log(skillName + " " + skillValue);
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
        //С��0����������壬�Ƴ����壬ǰ����������ƣ��������������
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
        //���ڵ���0��û���ֳɵ�������壬�ͼ�һ��
        if (skillValue >= 0 && nonbasicSkillPosition == -1)
        {
            //���е����������ƶ�
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
            rawImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillName);

            GameObject nonbasicSkillText = canvas.transform.Find("ValueText").gameObject;
            Text text = nonbasicSkillText.GetComponent<Text>();
            //�ǻ���������ֵΪ0����ʾ�հ�
            if (skillValue == 0)
            {
                text.text = "";
            }
            else
            {
                text.text = skillValue.ToString();
            }
        }
        //����0�����ֳɵ��������
        if (skillValue >= 0 && nonbasicSkillPosition > -1)
        {
            GameObject basicSkillText = nonbasicCanvasTransform.GetChild(nonbasicSkillPosition).Find("ValueText").gameObject;
            Text text = basicSkillText.GetComponent<Text>();
            text.text = skillValue.ToString();
        }
    }

    /// <summary>
    /// �������װ��
    /// </summary>
    /// <param name="equipData">�����ݿ��ѯ���������ݸ�ʽ��ͬ</param>
    public IEnumerator AddEquipment(Dictionary<string, object> equipData)
    {
        //�Ƴ�ԭ��װ���ļ���
        for (int i = skillList.Count - 1; i >= 0; i--)
        {
            skillList[i].RemoveValue("Equipment");
            if (skillList[i].sourceAndValue.Count == 0)
                skillList.Remove(skillList[i]);
        }

        //��װ������
        equipment = equipData;

        //��װ����ͼ��
        string equipKind = (string)equipment["CardKind"];
        EquipmentBackgroundLImage.enabled = true;
        EquipmentBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundL" + equipKind);
        EquipmentBackgroundRImage.enabled = true;
        EquipmentBackgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundR" + equipKind);
        EquipmentImage.enabled = true;
        string equipSkinResPath = Database.cardMonster.Query("AllSkinConfig", " and SkinID='" + equipment["CardSkinID"] + "'")[0]["SkinResPath"];
        yield return StartCoroutine(Utils.SAToRawImage(EquipmentImage, "/CardImage/item/" + equipSkinResPath + ".png"));

        //��װ���ļ���
        Dictionary<string, int> cardP = (Dictionary<string, int>)equipment["CardP"];
        foreach (var keyValuePair in cardP)
        {
            Dictionary<string, object> parameter = new();
            parameter.Add("SkillName", keyValuePair.Key);
            parameter.Add("SkillValue", keyValuePair.Value);
            parameter.Add("Source", "Equipment");
            yield return StartCoroutine(DoAction(AddSkill, parameter));
            //AddSkill(keyValuePair.Key, keyValuePair.Value, "Equipment");
        }
    }

    public IEnumerator DoAction(Func<ParameterNode, IEnumerator> effect, Dictionary<string, object> parameter)
    {
        BattleProcess battleProcess = BattleProcess.GetInstance();
        string effectName = effect.Method.Name;
        string fullName = "MonsterInBattle." + effectName;

        yield return StartCoroutine(battleProcess.ExecuteEffect(this, effect, parameter, fullName));

        yield return null;
    }

    /// <summary>
    /// ��Ӽ���
    /// </summary>
    /// <param name="skillName">��������</param>
    /// <param name="skillValue">��ֵ</param>
    /// <param name="source">��Դ</param>
    public IEnumerator AddSkill(ParameterNode parameterNode)
    {
        Dictionary<string, object> parameter = parameterNode.parameter;
        string skillName = (string)parameter["SkillName"];
        int skillValue = (int)parameter["SkillValue"];
        string source = (string)parameter["Source"];

        //�ȿ����Ƿ������������
        foreach (SkillInBattle skillInCard in skillList)
        {
            if (skillInCard.GetType().Name.Equals(skillName))
            {
                skillValue = skillInCard.AddValue(source, skillValue);
                LoadSkillImageValue(skillName, skillValue);
                yield break;
            }
        }

        Type type = Type.GetType(skillName);

        if (type != null)
        {
            SkillInBattle skill = (SkillInBattle)gameObject.AddComponent(type);

            skillValue = skill.AddValue(source, skillValue);
            skillList.Add(skill);
        }

        LoadSkillImageValue(skillName, skillValue);
    }

}
