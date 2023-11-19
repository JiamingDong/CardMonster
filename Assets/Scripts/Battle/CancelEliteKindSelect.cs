using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 精英选阵营界面的取消按钮
/// </summary>
public class CancelEliteKindSelect : MonoBehaviour
{
    public void OnClick()
    {
        Destroy(GameObject.Find("SelectEliteKindPrefabInstantiation"));
    }
}