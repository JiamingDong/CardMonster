using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ʹ�þ�Ӣ����ʱ��������ѡ����Ӫ�Ľ���Ĺرհ�ť�ϹҵĽű�
/// </summary>
public class CloseSelectEliteKindButton : MonoBehaviour
{
    public void OnClick()
    {
        GameObject selectEliteKindPrefabInstantiation = GameObject.Find("SelectEliteKindPrefabInstantiation");
        Destroy(selectEliteKindPrefabInstantiation);
    }
}
