using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.CompilerServices;

public class Utils : MonoBehaviour
{
    static string GetMembername(object caller, [CallerMemberName] string memberName = "")
    {
        return caller.GetType().FullName + "." + memberName;
    }

    /// <summary>
    /// ���dictionary2����dictionary1������key��ÿ��key��Ӧvalue����ȣ���==����Equals��������true�����򷵻�false
    /// </summary>
    /// <param name="dictionary1"></param>
    /// <param name="dictionary2"></param>
    /// <returns></returns>
    public static bool CompareOpportunity(Dictionary<string, object> dictionary1, Dictionary<string, object> dictionary2)
    {
        foreach (KeyValuePair<string, object> kvp in dictionary1)
        {
            string key = kvp.Key;
            object value = kvp.Value;

            if (!dictionary2.ContainsKey(key) || (dictionary2[key] != value && dictionary2[key].Equals(value)))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ����StreamingAssets���ͼƬ
    /// </summary>
    /// <param name="fileName">��"/"��ͷ</param>
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
    /// ��RawImage����StreamingAssets���ͼƬ
    /// </summary>
    /// <param name="fileName">��"/"��ͷ</param>
    public static IEnumerator SAToRawImage(RawImage image, string fileName)
    {
        Debug.Log("SAToRawImage:" + fileName + "-------------��ʼ");
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
            Debug.Log(e.ToString());
        }
        Debug.Log("SAToRawImage:" + fileName + "-------------����");
        yield return null;
    }

    /// <summary>
    /// ����ͼƬAssets/Image/Card/
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

    /// ����ģʽ��Textureת����Texture2D
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
    /// ��streamingAssetsPath�µ��ļ����Ƶ�persistentDataPath
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
