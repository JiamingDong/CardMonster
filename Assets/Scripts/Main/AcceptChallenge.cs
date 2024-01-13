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
        Debug.Log("AcceptChallenge.OnClick�����������ս��ť");

        if (SendChallenge.isChallenging)
        {
            return;
        }

        //�ر���ս
        if (isChallenging)
        {
            StopCoroutine(coroutine);
            SocketTool.link.Close();
            SocketTool.acceptMessageThread.Abort();

            GameObject receiveChallengeButtonCanvas = GameObject.Find("ReceiveChallengeButtonCanvas");

            Text receiveChallengeButtonText = receiveChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            receiveChallengeButtonText.text = "������ս";

            Image receiveChallengeButtonImage = receiveChallengeButtonCanvas.transform.Find("ButtonBackgroundImage").GetComponent<Image>();
            Sprite sprite = LoadAssetBundle.uiAssetBundle.LoadAsset<Sprite>("BlueButton");
            receiveChallengeButtonImage.sprite = sprite;

            isChallenging = false;
        }
        //������ս
        else
        {
            isChallenging = true;

            GameObject receiveChallengeButtonCanvas = GameObject.Find("ReceiveChallengeButtonCanvas");

            Text receiveChallengeButtonText = receiveChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            receiveChallengeButtonText.text = "ȡ����ս";

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
        Debug.Log("AcceptChallenge.StartSocketClient������");

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
