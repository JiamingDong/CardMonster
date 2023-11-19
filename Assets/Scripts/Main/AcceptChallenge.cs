using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AcceptChallenge : MonoBehaviour
{
    bool isChallenging = false;

    public void OnClick()
    {
        Debug.Log("AcceptChallenge.OnClick：点击接受挑战按钮");
        if (isChallenging)
        {
            StopCoroutine(coroutine);

            GameObject receiveChallengeButtonCanvas = GameObject.Find("ReceiveChallengeButtonCanvas");

            Text receiveChallengeButtonText = receiveChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            receiveChallengeButtonText.text = "接受挑战";

            Image receiveChallengeButtonImage = receiveChallengeButtonCanvas.transform.Find("ButtonBackgroundImage").GetComponent<Image>();
            Sprite sprite = LoadAssetBundle.uiAssetBundle.LoadAsset<Sprite>("BlueButton");
            receiveChallengeButtonImage.sprite = sprite;

            isChallenging = false;
        }
        else
        {
            isChallenging = true;

            GameObject receiveChallengeButtonCanvas = GameObject.Find("ReceiveChallengeButtonCanvas");

            Text receiveChallengeButtonText = receiveChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            receiveChallengeButtonText.text = "取消挑战";

            Image receiveChallengeButtonImage = receiveChallengeButtonCanvas.transform.Find("ButtonBackgroundImage").GetComponent<Image>();
            Sprite sprite = LoadAssetBundle.uiAssetBundle.LoadAsset<Sprite>("RedButton");
            receiveChallengeButtonImage.sprite = sprite;

            coroutine = StartCoroutine(StartSocketClient());
        }
    }

    private Coroutine coroutine;

    public IEnumerator StartSocketClient()
    {
        yield return null;
        Debug.Log("AcceptChallenge.StartSocketClient：进入");

        int port = Convert.ToInt32(GameObject.Find("PortInputField").GetComponent<InputField>().text);
        SocketTool.StartListening(IPAddress.Parse("127.0.0.1"), port);

        while (true)
        {
            if (SocketTool.link.Connected)
            {
                Debug.Log("AcceptChallenge.StartSocketClient：跳转战斗界面");
                SceneManager.LoadScene("BattleScene");
                yield return null;
            }
            else
            {
                yield return new WaitForSecondsRealtime(3f);

            }
        }


    }
}
