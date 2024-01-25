using UnityEngine;

public class SceneInit : MonoBehaviour
{
    void Start()
    {
#if UNITY_STANDALONE_WIN
        int h = Screen.height;
        int w = Screen.width;

        //Debug.Log("SceneInit:w=" + w);
        //Debug.Log("SceneInit:h=" + h);

        if (h > 1080 || w > 1920)
        {
            Screen.fullScreen = false;
            Screen.SetResolution(1920, 1080, false);
        }
        else
        {
            Screen.fullScreen = true;
            Screen.SetResolution(1920, 1080, true);
        }
#endif
    }
}
