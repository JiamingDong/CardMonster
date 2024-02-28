using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ø®≈∆œÍ«ÈΩÁ√Ê
/// </summary>
public class CardDetailInterface : MonoBehaviour
{
    public Text cardNameText;

    public List<RawImage> cardKindImageList;

    public List<RawImage> cardRaceImageList;

    public RectTransform CardContent;

    public RectTransform SkillContent;

    public void Init(List<Dictionary<string, string>> cardList)
    {
        CardContent.sizeDelta = new(CardContent.sizeDelta.x, (float)(334.8 * cardList.Count - 30));

        for (int i = 0; i < cardList.Count; i++)
        {
            Dictionary<string, string> card = cardList[i];

            GameObject cardForShowPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("CardForShowPrefab");

            GameObject aCard = Instantiate(cardForShowPrefab, CardContent.transform);
            aCard.GetComponent<Transform>().localPosition = new Vector3(0, (float)(-334.8 * i - 152.4), 0);
            aCard.GetComponent<CardForShow>().SetAllAttribute(card);
            CardInDetailInterfaceClick cardInDetailInterfaceClick = aCard.AddComponent<CardInDetailInterfaceClick>();
            cardInDetailInterfaceClick.cardData = card;
            aCard.AddComponent<Button>().onClick.AddListener(cardInDetailInterfaceClick.OnClick);

            GameObject cardCanvas = aCard.transform.GetChild(0).gameObject;
            cardCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(220.5f, 308);
            cardCanvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);

            Destroy(aCard.GetComponent<OpenCardDetailInterface>());
        }

        SwitchInfo(cardList[0]);
    }

    public void SwitchInfo(Dictionary<string, string> cardData)
    {
        for (int i = 0; i < cardKindImageList.Count; i++)
        {
            cardKindImageList[i].enabled = false;
        }

        for (int i = 0; i < cardRaceImageList.Count; i++)
        {
            cardRaceImageList[i].enabled = false;
        }

        for (int k = 0; k < SkillContent.transform.childCount; k++)
        {
            Destroy(SkillContent.transform.GetChild(k).gameObject);
        }

        string cardName = cardData["CardName"];
        cardNameText.text = cardName;

        string cardKind = cardData["CardKind"];
        if (!string.IsNullOrEmpty(cardKind))
        {
            Dictionary<string, string> cardKindDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardKind);
            int i = 0;
            HashSet<string> strings = new();
            foreach (var item in cardKindDic)
            {
                if (!strings.Contains(item.Value))
                {
                    strings.Add(item.Value);
                    cardKindImageList[i].enabled = true;
                    cardKindImageList[i].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("coloricon_" + item.Value);
                    i++;
                }
            }
        }

        string cardRace = cardData["CardRace"];
        if (!string.IsNullOrEmpty(cardRace))
        {
            List<string> cardRaceDic = JsonConvert.DeserializeObject<List<string>>(cardRace);
            int j = 0;
            foreach (var item in cardRaceDic)
            {
                cardRaceImageList[j].enabled = true;
                cardRaceImageList[j].texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("Race" + item);
                j++;
            }
        }

        int skillAmount = 0;

        string cardEliteSkill = cardData["CardEliteSkill"];
        if (!string.IsNullOrEmpty(cardEliteSkill))
        {
            Dictionary<string, Dictionary<string, object>> cardEliteSkillDic = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(cardEliteSkill);
            foreach (var item in cardEliteSkillDic)
            {
                string skillName = (string)item.Value["name"];
                int skillValue = Convert.ToInt32(item.Value["value"]);

                GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("SkillInCardDetailPrefab");

                GameObject aSkillPrefab = Instantiate(prefab, SkillContent.transform);
                aSkillPrefab.GetComponent<Transform>().localPosition = new Vector3(0, -200 * skillAmount - 150, 0);
                aSkillPrefab.GetComponent<SkillInCardDetail>().Init(skillName, skillValue);

                GameObject cardCanvas = aSkillPrefab.transform.GetChild(0).gameObject;
                cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                skillAmount++;
            }
        }

        string cardSkill = cardData["CardSkill"];
        if (!string.IsNullOrEmpty(cardSkill))
        {
            Dictionary<string, int> cardSkillDic = JsonConvert.DeserializeObject<Dictionary<string, int>>(cardSkill);
            foreach (var item in cardSkillDic)
            {
                string skillName = item.Key;
                int skillValue = item.Value;

                GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("SkillInCardDetailPrefab");

                GameObject aSkillPrefab = Instantiate(prefab, SkillContent.transform);
                aSkillPrefab.GetComponent<Transform>().localPosition = new Vector3(0, -200 * skillAmount - 150, 0);
                aSkillPrefab.GetComponent<SkillInCardDetail>().Init(skillName, skillValue);

                GameObject cardCanvas = aSkillPrefab.transform.GetChild(0).gameObject;
                cardCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                skillAmount++;
            }
        }

        SkillContent.sizeDelta = new(SkillContent.sizeDelta.x, skillAmount * 200);
    }
}
