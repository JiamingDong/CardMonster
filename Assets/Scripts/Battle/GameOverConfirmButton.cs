using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏结束界面，确认按钮返回主界面
/// </summary>
public class GameOverConfirmButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}
