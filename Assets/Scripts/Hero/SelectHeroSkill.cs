using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 给卡组选择英雄技能界面
/// </summary>
public class SelectHeroSkill : MonoBehaviour
{
    void Start()
    {
        List<Dictionary<string, string>> allHeroSKill = Database.cardMonster.Query("AllSkillConfig", " and SkillFlags like '%|2|%'");

        GameObject heroSkillBackgroundPanel = GameObject.Find("HeroSkillBackgroundPanel");

        for (int i = 0; i < allHeroSKill.Count; i++)
        {
            var heroSkill = allHeroSKill[i];
            string id = heroSkill["SkillID"];
            string name = heroSkill["SkillChineseName"];
            string description = heroSkill["SkillDescription"];

            GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("HeroSKillDetailPrefab");
            GameObject instance = Instantiate(prefab, heroSkillBackgroundPanel.transform);
            instance.GetComponent<Transform>().localPosition = new Vector3(0, (i - 1) * 250, 0);

            GameObject canvas = instance.transform.Find("Canvas").gameObject;
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            canvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            var initHeroSkillDetail = instance.GetComponent<InitHeroSkillDetail>();
            initHeroSkillDetail.Init(id, name, description);
        }
    }

}
