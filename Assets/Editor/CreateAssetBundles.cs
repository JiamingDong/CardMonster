using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

#if UNITY_STANDALONE_WIN
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
#endif
#if UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
#endif
    }
}