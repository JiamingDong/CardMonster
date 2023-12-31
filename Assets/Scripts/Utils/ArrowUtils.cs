using System.Collections;
using UnityEngine;

public class ArrowUtils:MonoBehaviour
{
    public static IEnumerator CreateArrow(Vector3 startPosition, Vector3 endPosition)
    {
        //Debug.Log(startPosition);
        //Debug.Log(endPosition);
        GameObject arrowPrefab = LoadAssetBundle.prefabAssetBundle.LoadAsset<GameObject>("ArrowPrefab");
        GameObject arrowInstance = Instantiate(arrowPrefab, GameObject.Find("BattleSceneCanvas").transform);
        GameObject canvas = arrowInstance.transform.GetChild(0).gameObject;
        canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        arrowInstance.GetComponent<InitArrowPrefab>().Init(startPosition, endPosition);
        yield return new WaitForSecondsRealtime(1);
    }
}