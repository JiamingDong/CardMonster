using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ��Ϸ�������棬ȷ�ϰ�ť����������
/// </summary>
public class GameOverConfirmButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}
