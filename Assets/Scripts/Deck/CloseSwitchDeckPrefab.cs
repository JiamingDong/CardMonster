using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������ر��л�����Ľ���
/// </summary>
public class CloseSwitchDeckPrefab : MonoBehaviour
{
    public void OnClick()
    {
        Destroy(transform.parent.parent.gameObject);
    }
}
