using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ֽ����ȡ����ť
/// </summary>
public class ExitButton : MonoBehaviour
{
    public GameObject interfaceGameObject;

    public void OnClick()
    {
        Destroy(interfaceGameObject);
    }
}