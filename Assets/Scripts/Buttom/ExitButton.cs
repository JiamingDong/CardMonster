using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各种界面的取消按钮
/// </summary>
public class ExitButton : MonoBehaviour
{
    public GameObject interfaceGameObject;

    public void OnClick()
    {
        Destroy(interfaceGameObject);
    }
}