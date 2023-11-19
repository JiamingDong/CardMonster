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
        if (isChallenging)
        {
            StopCoroutine(coroutine);

            GameObject receiveChallengeButtonCanvas = GameObject.Find("ReceiveChallengeButtonCanvas");

            Text receiveChallengeButtonText = receiveChallengeButtonCanvas.transform.Find("ButtonText").GetComponent<Text>();
            receiveChallengeButtonText.text = "������ս";

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

        int port = Convert.ToInt32(GameObject.Find("PortInputField").GetComponent<InputField>().text);
        SocketTool.StartListening(IPAddress.Parse("127.0.0.1"), port);

        while (true)
        {
            if (SocketTool.link.Connected)
            {
                Debug.Log("AcceptChallenge.StartSocketClient����תս������");
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
