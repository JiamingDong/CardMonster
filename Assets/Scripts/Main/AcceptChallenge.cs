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
        //关闭挑战
        if (isChallenging)
        {
            StopCoroutine(coroutine);
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

        SocketTool.acceptMessageThread.Start();

        while (true)
        {
            yield return null;
            NetworkMessage networkMessage = SocketTool.GetNetworkMessage();
            //Debug.Log(networkMessage != null ? networkMessage.Type + "----" + (networkMessage.Type == NetworkMessageType.GameStart) : "null");
            if (networkMessage != null && networkMessage.Type == NetworkMessageType.GameStart)
            {
                Debug.Log("AcceptChallenge.StartSocketClient：跳转战斗界面");
                SceneManager.LoadScene("BattleScene");
                Debug.Log("AcceptChallenge.StartSocketClient：跳转战斗界面2");
                yield return null;
                Debug.Log("AcceptChallenge.StartSocketClient：跳转战斗界面3");
                yield return null;
                yield break;
            }
        }
    }
}
