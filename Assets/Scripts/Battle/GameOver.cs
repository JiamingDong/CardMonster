using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏结束，停止所有协程
/// </summary>
public class GameOver : MonoBehaviour
{
    void Start()
    {
        StopAllCoroutines();
    }
}
