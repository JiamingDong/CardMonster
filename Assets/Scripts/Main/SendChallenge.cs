using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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
        IPAddress iPAddress = IPAddress.Parse(GameObject.Find("IPAddressInputField").GetComponent<InputField>().text);
        int port = Convert.ToInt32(GameObject.Find("PortInputField").GetComponent<InputField>().text);
        SocketTool.StartClient(iPAddress, port);
        while (!SocketTool.link.Connected)
        {
            yield return new WaitForSecondsRealtime(3f);
            SocketTool.StartClient(iPAddress, port);
        }

        Debug.Log("SendChallenge.StartSocketClient：跳转战斗界面");
        SceneManager.LoadScene("BattleScene");
    }
}
