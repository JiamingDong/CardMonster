using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点击背景关闭切换卡组的界面
/// </summary>
public class CloseSwitchDeckPrefab : MonoBehaviour
{
    public void OnClick()
    {
        Destroy(transform.parent.parent.gameObject);
    }
}
