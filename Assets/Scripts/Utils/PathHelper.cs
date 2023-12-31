using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHelper : MonoBehaviour
{
    /// <summary>
    /// streamingAssets·��
    /// </summary>
    /// <returns></returns>
    public static string getStreamingAssetsPath()
    {
#if UNITY_ANDROID
        //��׿·��
        return Application.dataPath + "!assets";
#endif

#if UNITY_STANDALONE_WIN
    //windows·��
    return Application.streamingAssetsPath;
#endif

    }
}
