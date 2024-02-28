using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卡牌详情界面的技能
/// </summary>
public class SkillInCardDetail : MonoBehaviour
{
    public RawImage skillImage;
    public Text skillNameText;
    public Text skillValueText;
    public Text skillDescriptionText;

    public void Init(string skillEnglishName, int skillValue)
    {
        var skillConfig = Database.cardMonster.Query("AllSkillConfig", " and SkillEnglishName='" + skillEnglishName + "'")[0];

        var skillImageName = skillConfig["SkillImageName"];
        skillImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillImageName);

        var skillChineseName = skillConfig["SkillChineseName"];
        skillNameText.text = skillChineseName;

        var typeInBattle = skillConfig["TypeInBattle"];
        if (typeInBattle == "state")
        {
            skillValueText.enabled = false;
        }
        skillValueText.text = skillValue.ToString();

        var skillDescription = skillConfig["SkillDescription"];
        skillDescriptionText.text = skillDescription;
    }
}
