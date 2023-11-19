using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHelper : MonoBehaviour
{
    /// <summary>
    /// streamingAssets路径
    /// </summary>
    /// <returns></returns>
    public static string getStreamingAssetsPath()
    {
#if UNITY_ANDROID
        //安卓路径
        return Application.dataPath + "!assets";
#endif

#if UNITY_STANDALONE_WIN
    //windows路径
    return Application.streamingAssetsPath;
#endif

    }
}
