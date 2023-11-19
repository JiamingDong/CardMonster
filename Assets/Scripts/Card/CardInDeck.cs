using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class CardInDeck : MonoBehaviour
{
    public string id;
    public string skinID;
    public string kind;
    public string type;
    public int cost;
    public string flags;

    public RawImage backgroundLImage;
    public RawImage backgroundRImage;
    public RawImage eliteConsumeBackgroundImage;
    public RawImage consumeBackgroundImage;
    public RawImage cardImage;
    public Text costText;
    // Start is called before the first frame update
    void Start()
    {
        backgroundLImage.enabled = true;
        backgroundRImage.enabled = true;
        cardImage.enabled = true;
        costText.enabled = true;
    }

    void OnDestroy()
    {
        Destroy(cardImage.texture);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="skinID"></param>
    /// <param name="kind">传递参数时，用两边带竖线的形式</param>
    /// <param name="type"></param>
    /// <param name="cost"></param>
    /// <param name="flags">传递参数时，用两边带竖线的形式</param>
    public void changeAllAttribute(Dictionary<string, string> cardData)
    {
        id = cardData["CardID"];
        skinID = cardData["CardSkinID"];
        kind = cardData["CardKind"];
        type = cardData["CardType"];
        cost = Convert.ToInt32(cardData["CardCost"]);
        flags = cardData["CardFlags"];
        generateCardImage();
    }

    private void generateCardImage()
    {

        Dictionary<string, object> kindDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(kind);

        //Debug.Log("CardInDeck.generateCardImage:colorArray[0]="+ colorArrayPlus[0]+ ",colorArray[1]="+ colorArrayPlus[1]);

        backgroundLImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundL"+ kindDictionary["leftKind"]);
        backgroundRImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("BackgroundR"+ (kindDictionary.ContainsKey("rightKind") ? kindDictionary["rightKind"] : kindDictionary["leftKind"]));

        if (type.Equals("consume"))
        {
            if (flags.Contains("|4|"))
            {
                eliteConsumeBackgroundImage.enabled = true;
                consumeBackgroundImage.enabled = false;
                eliteConsumeBackgroundImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("EliteConsumeBackgroundInDeck");
            }
            else
            {
                eliteConsumeBackgroundImage.enabled = false;
                consumeBackgroundImage.enabled = true;
                consumeBackgroundImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>("NormalConsumeBackgroundInDeck");
            }
        }
        else
        {
            eliteConsumeBackgroundImage.enabled = false;
            consumeBackgroundImage.enabled = false;
        }

        string skinResPath = Database.cardMonster.Query("AllSkinConfig", " and SkinID='" + skinID + "'")[0]["SkinResPath"];
        string kindPath = (string)kindDictionary["leftKind"];
        if (!type.Equals("monster"))
            kindPath = "item";
        StartCoroutine(Utils.SAToRawImage(cardImage, "/CardImage/" + kindPath + "/" + skinResPath + ".png"));

        //string cardImageFileType = type.Equals("0") ? ".jpg" : ".png";
        //StartCoroutine(Tool.LoadCardImage(cardImage, "/CardImage/" + skinID + cardImageFileType));
        costText.text = cost.ToString();
    }
}
