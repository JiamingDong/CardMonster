using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 收藏里的卡和手牌
/// </summary>
public class CardForShow : MonoBehaviour
{
    public string id;
    public string cardName;
    public string skinID;
    public string kind;
    public string type;
    public int cost;
    public string race;
    public int hp;
    public string flags;
    public string skill;
    public string eliteSkill;

    public RawImage cardBackImage;
    public RawImage cardBackgroundImage;
    public RawImage eliteConsumeBackgroundImage;
    public RawImage consumeBackgroundImage;
    public RawImage backgroundL;
    public RawImage backgroundR;
    public RawImage cardImage;
    public RawImage frameLImage;
    public RawImage frameRImage;
    public RawImage typeBackgroundImage;
    public RawImage typeImage;
    public RawImage colorBackgroundLImage;
    public RawImage colorBackgroundRImage;
    public RawImage colorLImage;
    public RawImage colorRImage;
    public RawImage nameBackgroundLImage;
    public RawImage nameBackgroundRImage;
    public RawImage goldenLineImage;
    public Text cardNameText;
    public RawImage crystalImage;
    public Text costText;
    public RawImage nonBasicSkillShadow;
    public RawImage[] skillBackgroundImage;
    public RawImage[] skillImage;
    public Text[] skillValueText;
    public RawImage[] eliteSkillBackgroundImage;
    public RawImage[] eliteSkillImage;
    public Text[] eliteSkillValueText;
    public RawImage[] raceImage;
    public RawImage armorImage;
    public RawImage lifeImage;
    public Text hPText;

    void Start()
    {

        cardBackImage.enabled = true;
        cardBackImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("CardBack");

        cardBackgroundImage.enabled = true;
        cardBackgroundImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("CardBackground");

        typeBackgroundImage.enabled = true;
        typeBackgroundImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("TypeBackground");

        goldenLineImage.enabled = true;
        goldenLineImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("GoldenLine");

        crystalImage.enabled = true;
        crystalImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("CostBackground");

    }

    /// <summary>
    /// 销毁所有texture引用的东西，防止一直占用内存
    /// </summary>
    private void OnDestroy()
    {
        Destroy(cardImage.texture);
    }

    /// <summary>
    /// 加载卡牌属性
    /// </summary>
    /// <param name="cardData">必须和从数据库取出的形式一样</param>
    public void SetAllAttribute(Dictionary<string, string> cardData)
    {
        id = cardData["CardID"];
        cardName = cardData["CardName"];
        type = cardData["CardType"];
        kind = cardData["CardKind"];
        race = cardData["CardRace"];
        hp = Convert.ToInt32(cardData["CardHP"].Equals("") ? "0" : cardData["CardHP"]);
        flags = cardData["CardFlags"];
        skinID = cardData["CardSkinID"];
        cost = Convert.ToInt32(cardData["CardCost"]);
        skill = cardData["CardSkill"];
        eliteSkill = cardData["CardEliteSkill"];

        GenerateCardImage();
    }

    private void GenerateCardImage()
    {
        eliteConsumeBackgroundImage.enabled = true;
        consumeBackgroundImage.enabled = true;
        backgroundL.enabled = true;
        backgroundR.enabled = true;
        cardImage.enabled = true;
        frameLImage.enabled = true;
        frameRImage.enabled = true;
        typeImage.enabled = true;
        colorBackgroundLImage.enabled = true;
        colorBackgroundRImage.enabled = true;
        colorLImage.enabled = true;
        colorRImage.enabled = true;
        nameBackgroundLImage.enabled = true;
        nameBackgroundRImage.enabled = true;
        cardNameText.enabled = true;
        costText.enabled = true;
        //nonBasicSkillShadow.enabled = false;//case 0，nonBasicSkillShadow.enabled = false会失效
        foreach (RawImage image in skillBackgroundImage)
        {
            image.enabled = true;
        }
        foreach (RawImage image in skillImage)
        {
            image.enabled = true;
        }
        foreach (Text image in skillValueText)
        {
            image.enabled = true;
        }
        foreach (RawImage image in eliteSkillBackgroundImage)
        {
            image.enabled = true;
        }
        foreach (RawImage image in eliteSkillImage)
        {
            image.enabled = true;
        }
        foreach (Text text in eliteSkillValueText)
        {
            text.enabled = true;
        }
        foreach (RawImage image in raceImage)
        {
            image.enabled = true;
        }
        armorImage.enabled = true;
        lifeImage.enabled = true;
        hPText.enabled = true;

        Dictionary<string, object> kindDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(kind);

        if (type.Equals("consume"))
        {
            List<string> flagsJArray = null;
            if (flags != null && flags != "")
            {
                flagsJArray = JsonConvert.DeserializeObject<List<string>>(flags);
            }

            //提升速度
            if (flagsJArray != null && flagsJArray.Contains("4"))
            {

                eliteConsumeBackgroundImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("EliteConsumeBackground");
                typeImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("TypeConsume");
                colorBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("ColorBackgroundLall");
                colorLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_" + kindDictionary["leftKind"]);
                nameBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NameBackgroundLall");
                colorBackgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("ColorBackgroundLall");
                colorRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_" + kindDictionary["rightKind"]);
                nameBackgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NameBackgroundRall");
                eliteConsumeBackgroundImage.enabled = true;
                consumeBackgroundImage.enabled = false;
            }
            else
            {
                consumeBackgroundImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NormalConsumeBackground");
                typeImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("TypeConsume");
                colorBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("ColorBackgroundLall");
                colorLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_all");
                nameBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NameBackgroundLall");
                nameBackgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NameBackgroundRall");
                consumeBackgroundImage.enabled = true;
                eliteConsumeBackgroundImage.enabled = false;
                colorBackgroundRImage.enabled = false;
                colorRImage.enabled = false;
            }
        }
        else
        {
            consumeBackgroundImage.enabled = false;
            eliteConsumeBackgroundImage.enabled = false;
        }
        if (type.Equals("monster") || type.Equals("equip"))
        {
            if (type.Equals("monster")) typeImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("TypeMonster");
            if (type.Equals("equip")) typeImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("TypeEquipment");

            backgroundL.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundL" + kindDictionary["leftKind"]);
            frameLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("FrameL" + kindDictionary["leftKind"]);
            colorBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("ColorBackgroundL" + kindDictionary["leftKind"]);
            colorLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_" + kindDictionary["leftKind"]);
            nameBackgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NameBackgroundL" + kindDictionary["leftKind"]);

            backgroundR.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundR" + (kindDictionary.ContainsKey("rightKind") ? kindDictionary["rightKind"] : kindDictionary["leftKind"]));
            frameRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("FrameR" + (kindDictionary.ContainsKey("rightKind") ? kindDictionary["rightKind"] : kindDictionary["leftKind"]));
            if (kindDictionary.ContainsKey("rightKind"))
            {
                colorBackgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("ColorBackgroundL" + kindDictionary["rightKind"]);
                colorRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_" + kindDictionary["rightKind"]);
            }
            else
            {
                colorBackgroundRImage.enabled = false;
                colorRImage.enabled = false;
            }
            nameBackgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NameBackgroundR" + (kindDictionary.ContainsKey("rightKind") ? kindDictionary["rightKind"] : kindDictionary["leftKind"]));
        }
        else
        {
            backgroundL.enabled = false;
            backgroundR.enabled = false;

            frameLImage.enabled = false;
            frameRImage.enabled = false;
        }

        string skinResPath = Database.cardMonster.Query("AllSkinConfig", " and SkinID='" + skinID + "'")[0]["SkinResPath"];
        string kindPath = (string)kindDictionary["leftKind"];
        if (!type.Equals("monster"))
            kindPath = "item";
        StartCoroutine(Utils.SAToRawImage(cardImage, "/CardImage/" + kindPath + "/" + skinResPath + ".png"));

        cardNameText.text = cardName;
        costText.text = cost.ToString();

        List<KeyValuePair<string, int>> skillList = new List<KeyValuePair<string, int>>();
        if (skill != null && !skill.Equals(""))
        {
            Dictionary<string, object> pD = JsonConvert.DeserializeObject<Dictionary<string, object>>(skill);
            foreach (KeyValuePair<string, object> keyValuePair in pD)
            {
                string key = keyValuePair.Key;
                long value = (long)keyValuePair.Value;

                skillList.Add(new KeyValuePair<string, int>(key, (int)value));
            }
        }

        int basicNumber = 0;
        int nonNasicNumber = 3;
        for (int i = 0; i < skillList.Count; i++)
        {
            string skillEnglishName = skillList[i].Key;
            //Debug.Log("CardForShow：" + i + " " + skillId);
            Dictionary<string, string> skillresult = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName='" + skillEnglishName + "'")[0];
            string mark = skillresult["SkillFlags"];
            if (mark.Contains("|1|"))
            {
                if (basicNumber < 3)
                {
                    skillImage[basicNumber].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillresult["SkillImageName"]);
                    skillBackgroundImage[basicNumber].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BasicSkillBackground");
                    skillValueText[basicNumber].text = skillList[i].Value == 0 ? null : skillList[i].Value.ToString();
                    basicNumber++;
                }
            }
            else
            {
                if (nonNasicNumber < 6)
                {
                    skillImage[nonNasicNumber].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillresult["SkillImageName"]);
                    skillBackgroundImage[nonNasicNumber].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NonBasicSkillBackgroundall");
                    skillValueText[nonNasicNumber].text = skillList[i].Value == 0 ? null : skillList[i].Value.ToString();
                    nonNasicNumber++;
                }
            }
        }
        for (int i = basicNumber; i < 3; i++)
        {
            skillBackgroundImage[i].enabled = false;
            skillImage[i].enabled = false;
            skillValueText[i].enabled = false;
        }
        for (int i = nonNasicNumber; i < 6; i++)
        {
            skillBackgroundImage[i].enabled = false;
            skillImage[i].enabled = false;
            skillValueText[i].enabled = false;
        }
        //牌面技能位置
        int nonNasicSkillAmount = nonNasicNumber - 3;
        switch (nonNasicSkillAmount)
        {
            case 1:
                nonBasicSkillShadow.enabled = true;
                nonBasicSkillShadow.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NonBasicSkillShadow");
                skillBackgroundImage[3].GetComponent<RectTransform>().localPosition = new Vector3(0, -319, 0);
                skillImage[3].GetComponent<RectTransform>().localPosition = new Vector3(0, -300, 0);
                skillValueText[3].GetComponent<RectTransform>().localPosition = new Vector3(0, -355, 0);
                break;
            case 2:
                nonBasicSkillShadow.enabled = true;
                nonBasicSkillShadow.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NonBasicSkillShadow");
                skillBackgroundImage[3].GetComponent<RectTransform>().localPosition = new Vector3(-67, -319, 0);
                skillImage[3].GetComponent<RectTransform>().localPosition = new Vector3(-67, -300, 0);
                skillValueText[3].GetComponent<RectTransform>().localPosition = new Vector3(-67, -355, 0);
                skillBackgroundImage[4].GetComponent<RectTransform>().localPosition = new Vector3(67, -319, 0);
                skillImage[4].GetComponent<RectTransform>().localPosition = new Vector3(67, -300, 0);
                skillValueText[4].GetComponent<RectTransform>().localPosition = new Vector3(67, -355, 0);
                break;
            case 3:
                nonBasicSkillShadow.enabled = true;
                nonBasicSkillShadow.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NonBasicSkillShadow");
                break;
            default:
                nonBasicSkillShadow.enabled = false;
                break;
        }

        if (eliteSkill != null && !eliteSkill.Equals(""))
        {
            Dictionary<string, object> ePD = JsonConvert.DeserializeObject<Dictionary<string, object>>(eliteSkill);

            if (ePD.ContainsKey("leftSkill") && (JObject)ePD["leftSkill"] != null)
            {
                JObject l = (JObject)ePD["leftSkill"];
                string nL = (string)l["name"];
                long vL = (long)l["value"];

                colorBackgroundLImage.enabled = true;
                colorLImage.enabled = true;
                eliteSkillBackgroundImage[0].enabled = true;
                eliteSkillImage[0].enabled = true;
                eliteSkillValueText[0].enabled = true;

                eliteSkillImage[0].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(nL);
                eliteSkillBackgroundImage[0].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NonBasicSkillBackgroundall");
                eliteSkillValueText[0].text = vL == 0 ? null : vL.ToString();
            }
            else
            {
                colorBackgroundLImage.enabled = false;
                colorLImage.enabled = false;
                eliteSkillBackgroundImage[0].enabled = false;
                eliteSkillImage[0].enabled = false;
                eliteSkillValueText[0].enabled = false;
            }

            if (ePD.ContainsKey("rightSkill") && (JObject)ePD["rightSkill"] != null)
            {
                JObject r = (JObject)ePD["rightSkill"];
                string nR = (string)r["name"];
                long vR = (long)r["value"];

                colorBackgroundRImage.enabled = true;
                colorRImage.enabled = true;
                eliteSkillBackgroundImage[1].enabled = true;
                eliteSkillImage[1].enabled = true;
                eliteSkillValueText[1].enabled = true;

                eliteSkillImage[1].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(nR);
                eliteSkillBackgroundImage[1].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NonBasicSkillBackgroundall");
                eliteSkillValueText[1].text = vR == 0 ? null : vR.ToString();
            }
            else
            {
                colorBackgroundRImage.enabled = false;
                colorRImage.enabled = false;
                eliteSkillBackgroundImage[1].enabled = false;
                eliteSkillImage[1].enabled = false;
                eliteSkillValueText[1].enabled = false;
            }
        }
        else
        {
            eliteSkillBackgroundImage[0].enabled = false;
            eliteSkillImage[0].enabled = false;
            eliteSkillValueText[0].enabled = false;
            eliteSkillBackgroundImage[1].enabled = false;
            eliteSkillImage[1].enabled = false;
            eliteSkillValueText[1].enabled = false;
        }

        if (race != null && race != "")
        {
            JArray raceJArray = JsonConvert.DeserializeObject<JArray>(race);
            for (int i = 0; i < raceImage.Length; i++)
            {
                if (raceJArray != null && i < raceJArray.Count)
                {
                    raceImage[i].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("Race" + raceJArray[i]);
                }
                else raceImage[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < raceImage.Length; i++)
            {
                raceImage[i].enabled = false;
            }
        }

        if (hp == 0)
        {
            hPText.text = null;
            armorImage.enabled = false;
            lifeImage.enabled = false;
        }
        else
        {
            if (type.Equals("monster"))
            {
                lifeImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("Life");
                armorImage.enabled = false;
            }
            if (type.Equals("equip"))
            {
                armorImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("Armor");
                lifeImage.enabled = false;
            }
            hPText.text = hp.ToString();
        }
    }
}