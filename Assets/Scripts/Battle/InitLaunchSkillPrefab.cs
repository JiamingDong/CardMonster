using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ڼ��ܷ������ͼ���ϣ�������������
/// </summary>
public class InitLaunchSkillPrefab : MonoBehaviour
{
    public RawImage skillImage;

    public Text skillValueText;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillTypeName">��������</param>
    /// <param name="skillValue"></param>
    public void Init(string skillTypeName, int skillValue)
    {
        var skillConfig = Database.cardMonster.Query("AllSkillConfig", "and SkillClassName='" + skillTypeName + "'")[0];
        var skillImageName = skillConfig["SkillImageName"];
        var skillEnglishName = skillConfig["SkillEnglishName"];

        var skillTypeConfig = Database.cardMonster.Query("SkillTypeConfig", "and SkillName='" + skillEnglishName + "'")[0];
        var skillType = skillTypeConfig["SkillType"];

        skillImage.texture = LoadAssetBundle.cardAssetBundle.LoadAsset<Texture>(skillImageName);
        switch (skillType)
        {
            case "state":
                skillValueText.enabled = false;
                Vector3 vector3 = skillImage.rectTransform.localPosition;
                skillImage.rectTransform.localPosition = new(0, vector3.y, vector3.z);
                break;
            case "value":
                skillValueText.text = skillValue.ToString();
                break;
        }
    }
}
