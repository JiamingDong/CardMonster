using UnityEngine;

/// <summary>
/// 逆天改命，创建界面
/// </summary>
public class ChangeDestinyStart : HeroSkillStart
{
    override public void OnClick()
    {
        //Debug.Log("逆天改命");

        GameObject prefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("ChangeDestinyPrefab");
        GameObject battleSceneCanvas = GameObject.Find("BattleSceneCanvas");
        GameObject instance = Instantiate(prefab, battleSceneCanvas.transform);
        instance.name = "ChangeDestinyPrefabbInstantiation";
        instance.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);

        GameObject canvas = instance.transform.Find("Canvas").gameObject;
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        canvas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
}
