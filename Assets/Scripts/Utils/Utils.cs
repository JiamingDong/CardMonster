using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class Utils : MonoBehaviour
{
    /// <summary>
    /// 加载StreamingAssets里的图片
    /// </summary>
    /// <param name="fileName">以"/"开头</param>
    public static IEnumerator LoadStreamingAssetsImage(Image image, string fileName)
    {
        string path = Application.streamingAssetsPath + fileName;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
        //Debug.Log(path);
        yield return request.SendWebRequest();
        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
    }

    /// <summary>
    /// 给RawImage加载StreamingAssets里的图片
    /// </summary>
    /// <param name="fileName">以"/"开头</param>
    public static IEnumerator SAToRawImage(RawImage image, string fileName)
    {
        //Debug.Log("SAToRawImage:" + fileName + "-------------开始");
        string path = Application.streamingAssetsPath + fileName;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
        //Debug.Log("LoadCardImage:" + path);
        yield return request.SendWebRequest();
        try
        {
            image.texture = DownloadHandlerTexture.GetContent(request);
        }
        catch (Exception e)
        {
            Debug.Log(fileName);
            Debug.Log(e.ToString());
        }
        //Debug.Log("SAToRawImage:" + fileName + "-------------结束");
        yield return null;
    }

    /// <summary>
    /// 加载图片Assets/Image/Card/
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Sprite LoadAssetBundleResource(string fileName)
    {
        string fileNmaePrefix = "Assets/Image/Card/";
        string fileNmaeSuffix = ".png";
        Texture2D texture = (Texture2D)LoadAssetBundle.cardAssetBundle.LoadAsset(fileNmaePrefix + fileName + fileNmaeSuffix);
        Debug.Log("LoadAssetBundleResource " + fileNmaePrefix + fileName + fileNmaeSuffix);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public static T LoadAsset<T>(string fileName) where T : UnityEngine.Object
    {
        return LoadAssetBundle.cardAssetBundle.LoadAsset<T>(fileName);
    }

    /// 运行模式下Texture转换成Texture2D
    public static Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

    /// <summary>
    /// 将streamingAssetsPath下的文件复制到persistentDataPath
    /// </summary>
    /// <param name="fileName"></param>
    public static void SAToPD(string fileName)
    {
        string src = Path.Combine(Application.streamingAssetsPath, fileName);
        string des = Path.Combine(Application.persistentDataPath, fileName);

        UnityWebRequest request = UnityWebRequest.Get(src);
        request.SendWebRequest();

        while (true)
        {
            if (request.downloadHandler.isDone)
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, fileName.Substring(0, fileName.LastIndexOf(Path.DirectorySeparatorChar))));
                FileStream fs = File.Create(des);
                fs.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
                break;
            }
        }
    }
}
