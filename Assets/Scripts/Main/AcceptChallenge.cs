using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AcceptChallenge : MonoBehaviour
{
    public static bool isChallenging = false;

    public void OnClick()
    {
        Debug.Log("AcceptChallenge.OnClick：点击接受挑战按钮");

        if (SendChallenge.isChallenging)
        {
            return;
        }

        //关闭挑战
        if (isChallenging)
        {
            StopCoroutine(coroutine);
            SocketTool.link.Close();
            SocketTool.acceptMessageThread.Abort();

            GameObject receiveChallengeButtonCanvas = GameObject.Find("ReceiveChallengeButtonCanvas");

            Text receiveChallengeButtonText = receiveChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            receiveChallengeButtonText.text = "接受挑战";

            Image receiveChallengeButtonImage = receiveChallengeButtonCanvas.transform.Find("ButtonBackgroundImage").GetComponent<Image>();
            Sprite sprite = LoadAssetBundle.uiAssetBundle.LoadAsset<Sprite>("BlueButton");
            receiveChallengeButtonImage.sprite = sprite;

            isChallenging = false;
        }
        //开启挑战
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

        int port = Convert.ToInt32(GameObject.Find("AllyPortInputField").GetComponent<InputField>().text);
        SocketTool.StartListening(IPAddress.Parse("0.0.0.0"), port);

        SocketTool.acceptMessageThread = new(SocketTool.ReceiveMessage);
        SocketTool.acceptMessageThread.Start();

        while (true)
        {
            yield return null;
            NetworkMessage networkMessage = SocketTool.GetNetworkMessage();
            if (networkMessage != null && networkMessage.Type == NetworkMessageType.SendChallenge)
            {
                isChallenging = false;

                NetworkMessage networkMessage2 = new();
                networkMessage2.Type = NetworkMessageType.AcceptChallenge;
                networkMessage2.Parameter = new();

                SocketTool.SendMessage(networkMessage2);

                SceneManager.LoadScene("BattleScene");

                yield break;
            }
        }
    }
}
