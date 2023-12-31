using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 启动时加载AssetBundle
/// </summary>
public class LoadAssetBundle : MonoBehaviour
{
    /// <summary>
    /// 预制体
    /// </summary>
    public static AssetBundle prefabAssetBundle;
    /// <summary>
    /// 卡牌
    /// </summary>
    public static AssetBundle cardAssetBundle;
    /// <summary>
    /// ui
    /// </summary>
    public static AssetBundle uiAssetBundle;
    /// <summary>
    /// 英雄
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