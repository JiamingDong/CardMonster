using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 发起挑战按钮
/// </summary>
public class SendChallenge : MonoBehaviour
{
    public static bool isChallenging = false;

    public void OnClick()
    {
        Debug.Log("SendChallenge.OnClick：点击挑战按钮");

        if (AcceptChallenge.isChallenging)
        {
            return;
        }

        //关闭挑战
        if (isChallenging)
        {
            StopCoroutine(coroutine);
            SocketTool.CloseListening();

            SocketTool.acceptMessageThread?.Abort();

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
                Debug.Log("SendChallenge.StartSocketClient：已连接");

                SocketTool.acceptMessageThread = new(SocketTool.ReceiveMessage);
                SocketTool.acceptMessageThread.Start();

                while (true)
                {
                    Debug.Log("SendChallenge.StartSocketClient：发送挑战");
                    //Debug.Log("SocketTool.link.Connected=" + SocketTool.link.Connected);
                    Debug.Log("SelectMode.SelectWrite=" + SocketTool.link.Poll(1000, SelectMode.SelectWrite));
                    Debug.Log("SelectMode.SelectRead=" + SocketTool.link.Poll(1000, SelectMode.SelectRead));
                    Debug.Log("SelectMode.SelectError=" + SocketTool.link.Poll(1000, SelectMode.SelectError));

                    if (SocketTool.link.Poll(1000, SelectMode.SelectError))
                    {
                        break;
                    }

                    NetworkMessage networkMessage = new();
                    networkMessage.Type = NetworkMessageType.SendChallenge;
                    networkMessage.Parameter = new();

                    SocketTool.SendMessage(networkMessage);

                    NetworkMessage networkMessage2 = SocketTool.GetNetworkMessage();
                    if (networkMessage2 != null && networkMessage2.Type == NetworkMessageType.AcceptChallenge)
                    {
                        Debug.Log("SendChallenge.StartSocketClient：挑战被接受");
                        isChallenging = false;

                        SceneManager.LoadScene("BattleScene");

                        yield break;
                    }

                    yield return new WaitForSecondsRealtime(1);
                }

                SocketTool.acceptMessageThread.Abort();
                SocketTool.CloseListening();
            }
            yield return null;
        }
    }
}
