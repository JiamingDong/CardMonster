using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ����ʱ����AssetBundle
/// </summary>
public class LoadAssetBundle : MonoBehaviour
{
    /// <summary>
    /// Ԥ����
    /// </summary>
    public static AssetBundle prefabAssetBundle;
    /// <summary>
    /// ����
    /// </summary>
    public static AssetBundle cardAssetBundle;
    /// <summary>
    /// ui
    /// </summary>
    public static AssetBundle uiAssetBundle;
    /// <summary>
    /// Ӣ��
    /// </summary>
    public static AssetBundle heroAssetBundle;

    void Awake()
    {
        if (prefabAssetBundle == null)
            prefabAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "prefab"));
        if (cardAssetBundle == null)
            cardAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "card"));
        if (uiAssetBundle == null)
            uiAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "ui"));
        if (heroAssetBundle == null)
            heroAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "hero"));
    }
}