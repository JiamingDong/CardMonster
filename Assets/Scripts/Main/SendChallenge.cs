using System;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 发起挑战按钮
/// </summary>
public class SendChallenge : MonoBehaviour
{
    bool isChallenging = false;

    public void OnClick()
    {
        Debug.Log("SendChallenge.OnClick：点击挑战按钮");
        //关闭挑战
        if (isChallenging)
        {
            StopCoroutine(coroutine);
            SocketTool.CloseListening();

            GameObject launchChallengeButtonCanvas = GameObject.Find("SendChallengeButtonCanvas");

            Text launchChallengeButtonText = launchChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            launchChallengeButtonText.text = "发起挑战";

            Image launchChallengeButtonImage = launchChallengeButtonCanvas.transform.Find("ButtonBackgroundImage").GetComponent<Image>();
            Sprite sprite = LoadAssetBundle.uiAssetBundle.LoadAsset<Sprite>("OrangeButton");
            launchChallengeButtonImage.sprite = sprite;

            isChallenging = false;
        }
        //开启挑战
        else
        {
            isChallenging = true;

            GameObject launchChallengeButtonCanvas = GameObject.Find("SendChallengeButtonCanvas");

            Text launchChallengeButtonText = launchChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            launchChallengeButtonText.text = "取消挑战";

            Image launchChallengeButtonImage = launchChallengeButtonCanvas.transform.Find("ButtonBackgroundImage").GetComponent<Image>();
            Sprite sprite = LoadAssetBundle.uiAssetBundle.LoadAsset<Sprite>("RedButton");
            launchChallengeButtonImage.sprite = sprite;

            coroutine = StartCoroutine(StartSocketClient());
        }
    }

    private Coroutine coroutine;

    public IEnumerator StartSocketClient()
    {
        yield return null;
        Debug.Log("SendChallenge.StartSocketClient：进入");
        IPAddress iPAddress = IPAddress.Parse(GameObject.Find("EnemyIPInputField").GetComponent<InputField>().text);
        int port = Convert.ToInt32(GameObject.Find("EnemyPortInputField").GetComponent<InputField>().text);

        while (true)
        {
            //yield return null;
            SocketTool.StartClient(iPAddress, port);

            if (SocketTool.link.Connected)
            {
                // yield return null;
                SocketTool.acceptMessageThread.Start();
                SceneManager.LoadScene("BattleScene");
            }
            yield return null;
        }
    }
}
