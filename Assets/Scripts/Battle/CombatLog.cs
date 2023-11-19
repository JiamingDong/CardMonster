using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ս�����̼�¼
/// </summary>
public class CombatLog : MonoBehaviour
{
    [SerializeField]
    public Text textView;

    [SerializeField]
    public ScrollRect scrollControl;

    public int index;

    private void Start()
    {
        index = 1;
    }

    /// <summary>
    /// ���ս��������־
    /// </summary>
    /// <param name="text"></param>
    public void AddLog(string text)
    {
        textView.text += "\n" + index + "." + text;
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollControl.verticalNormalizedPosition = 0f;
    }
}
