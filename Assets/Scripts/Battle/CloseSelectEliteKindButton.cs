using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用精英怪兽时，弹出的选择阵营的界面的关闭按钮上挂的脚本
/// </summary>
public class CloseSelectEliteKindButton : MonoBehaviour
{
    public void OnClick()
    {
        GameObject selectEliteKindPrefabInstantiation = GameObject.Find("SelectEliteKindPrefabInstantiation");
        Destroy(selectEliteKindPrefabInstantiation);
    }
}
