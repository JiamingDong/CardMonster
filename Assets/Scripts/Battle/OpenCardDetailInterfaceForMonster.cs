using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenCardDetailInterfaceForMonster : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    float unscaledTime;

    void OpenInterface()
    {
        MonsterInBattle monsterInBattle = gameObject.GetComponent<MonsterInBattle>();
        Dictionary<string, string> equipment = monsterInBattle.equipment;
        Dictionary<string, string> cardData = monsterInBattle.cardData;
        List<SkillInBattle> skillList = monsterInBattle.skillList;

        Dictionary<string, int> cardSkill = new();
        foreach (SkillInBattle skillInBattle in skillList)
        {
            string skillClassName = skillInBattle.GetType().Name;

            var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillClassName='" + skillClassName + "'")[0];
            var skillEnglishName = skillConfig["SkillEnglishName"];

            cardSkill.Add(skillEnglishName, skillInBattle.GetSkillValue());
        }

        cardData["CardSkill"] = JsonConvert.SerializeObject(cardSkill);

        List<Dictionary<string, string>> cardList = new();
        cardList.Add(cardData);
        if (equipment != null)
        {
            cardList.Add(equipment);
        }

        GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardDetailInterfacePrefab");
        GameObject instance = Instantiate(prefab);
        instance.name = "CardDetailInterfacePrefab";
        instance.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        instance.GetComponent<CardDetailInterface>().Init(cardList);

        GameObject canvas = instance.transform.Find("Canvas").gameObject;
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        canvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OpenInterface();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        unscaledTime = Time.unscaledTime;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.unscaledTime - unscaledTime > 0.5)
        {
            OpenInterface();
        }
    }
}
