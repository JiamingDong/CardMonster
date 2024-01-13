using UnityEngine;
using UnityEngine.UI;

public class InitHeroSkillDetail : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;
    public Canvas buttonCanvas;

    public void Init(string id, string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;

        ChangeDeckHeroSkill changeDeckHeroSkill = buttonCanvas.GetComponent<ChangeDeckHeroSkill>();
        changeDeckHeroSkill.heroSkillId = id;
    }
}
