using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ӣѡ��Ӫ�����ȡ����ť
/// </summary>
public class CancelEliteKindSelect : MonoBehaviour
{
    public void OnClick()
    {
        Destroy(GameObject.Find("SelectEliteKindPrefabInstantiation"));
    }
}