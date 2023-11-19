using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 给卡组选择英雄技能界面
/// </summary>
public class SelectHeroSkill : MonoBehaviour
{
    public Text HeroNameText;
    public Text HeroSkillNameText1;
    public Text HeroSkillDescription1;
    public Text HeroSkillNameText2;
    public Text HeroSkillDescription2;
    public Text HeroSkillNameText3;
    public Text HeroSkillDescription3;

    public Canvas SelectHeroSkillButtonCanvas1;
    public Canvas SelectHeroSkillButtonCanvas2;
    public Canvas SelectHeroSkillButtonCanvas3;

    float heroBigImageWidth = 269.6f;
    float heroBigImageHeight = 292;
    float heroBackgroundPanelWidth = 289.6f;
    float heroBackgroundPanelHeight = 312;

    string selectedHeroId;//当前展示技能的英雄

    void Start()
    {
        List<Dictionary<string, string>> allHeroConfig = Database.cardMonster.Query("AllHeroConfig", "order by HeroID asc");

        GameObject heroWindowPanel = GameObject.Find("HeroWindowPanel");

        //加载左边英雄大图片
        GameObject heroBackgroundPanel = GameObject.Find("HeroBackgroundPanel");
        heroBackgroundPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(heroBackgroundPanelWidth, allHeroConfig.Count * heroBackgroundPanelHeight);
        heroBackgroundPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, -(allHeroConfig.Count * heroBackgroundPanelHeight- heroWindowPanel.GetComponent<RectTransform>().sizeDelta.y)/2, 0);
        //heroBackgroundPanel.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

        int index = 0;
        foreach (Dictionary<string, string> aHeroConfig in allHeroConfig)
        {
            //HeroBigImagePrefab
            GameObject heroBigImagePrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("HeroBigImagePrefab");
            GameObject aHeroBigImagePrefab = Instantiate(heroBigImagePrefab, heroBackgroundPanel.transform);
            aHeroBigImagePrefab.name = "HeroBigImagePrefab" + (index + 1).ToString();
            aHeroBigImagePrefab.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
            aHeroBigImagePrefab.GetComponent<Transform>().localPosition = new Vector3(0, (float)(heroBackgroundPanelHeight * (allHeroConfig.Count/2-index-0.5)));

            GameObject aHeroBigImageCanvas = aHeroBigImagePrefab.transform.GetChild(0).gameObject;
            aHeroBigImageCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            aHeroBigImageCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(heroBigImageWidth, heroBigImageHeight);
            aHeroBigImageCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            aHeroBigImageCanvas.GetComponent<SwitchShowHero>().heroId = aHeroConfig["HeroID"];

            GameObject HeroBigImage = aHeroBigImageCanvas.transform.GetChild(0).gameObject;
            HeroBigImage.GetComponent<Image>().sprite = LoadAssetBundle.heroAssetBundle.LoadAsset<Sprite>(aHeroConfig["HeroPic"]);
            //Debug.Log("SelectHeroSkill " + "HeroBigImage" + aHeroConfig.Key);
            index++;
        }

        //加载英雄技能
        //GameObject CardDeckCardDeckWindowPanel = GameObject.Find("CardDeckCardDeckWindowPanel");
        DeckInCollection deckInCollection = GameObject.Find("CardDeckWindowPanel").GetComponent<DeckInCollection>();
        string heroId= deckInCollection.heroSkillId;
        if (heroId.Equals(""))
            heroId = Database.cardMonster.Query("AllHeroConfig", "order by HeroID asc")[0]["HeroID"];
        //else heroId = Database.CardMonster.Query("HeroSkillConfig", "and HeroSkillId='" + deckInCollection.heroSkillId + "'")[0]["BelongTo"];
        LoadHeroSkill(heroId);
    }

    public void LoadHeroSkill(string heroId)
    {
        Dictionary<string, string> heroConfig = Database.cardMonster.Query("AllHeroConfig", "and HeroID='" + heroId + "'")[0];
        string heroSkill = heroConfig["HeroSkill"];
        heroSkill = "'" + heroSkill.Substring(1, heroSkill.Length - 2).Replace("|", "','") + "'";
        List<Dictionary<string,string>> skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillEnglishName in ("+ heroSkill + ")");
        HeroNameText.text = heroConfig["HeroName"];
        HeroSkillNameText1.text = skillConfig[0]["SkillChineseName"];
        HeroSkillDescription1.text = skillConfig[0]["SkillDescription"];
        SelectHeroSkillButtonCanvas1.GetComponent<ChangeDeckHeroSkill>().heroSkillId = skillConfig[0]["SkillID"];
        HeroSkillNameText2.text = skillConfig[1]["SkillChineseName"];
        HeroSkillDescription2.text = skillConfig[1]["SkillDescription"];
        SelectHeroSkillButtonCanvas2.GetComponent<ChangeDeckHeroSkill>().heroSkillId = skillConfig[1]["SkillID"];
        HeroSkillNameText3.text = skillConfig[2]["SkillChineseName"];
        HeroSkillDescription3.text = skillConfig[2]["SkillDescription"];
        SelectHeroSkillButtonCanvas3.GetComponent<ChangeDeckHeroSkill>().heroSkillId = skillConfig[2]["SkillID"];
    }
}
