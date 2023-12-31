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
        Debug.Log("AcceptChallenge.OnClick�����������ս��ť");
        //�ر���ս
        if (isChallenging)
        {
            StopCoroutine(coroutine);
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

        SocketTool.acceptMessageThread.Start();

        while (true)
        {
            yield return null;
            NetworkMessage networkMessage = SocketTool.GetNetworkMessage();
            //Debug.Log(networkMessage != null ? networkMessage.Type + "----" + (networkMessage.Type == NetworkMessageType.GameStart) : "null");
            if (networkMessage != null && networkMessage.Type == NetworkMessageType.GameStart)
            {
                Debug.Log("AcceptChallenge.StartSocketClient����תս������");
                SceneManager.LoadScene("BattleScene");
                Debug.Log("AcceptChallenge.StartSocketClient����תս������2");
                yield return null;
                Debug.Log("AcceptChallenge.StartSocketClient����תս������3");
                yield return null;
                yield break;
            }
        }
    }
}
