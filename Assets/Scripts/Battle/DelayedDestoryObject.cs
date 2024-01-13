using System.Collections;
using UnityEngine;

public class DelayedDestoryObject : MonoBehaviour
{
    public Object o;

    void Start()
    {
        StartCoroutine(Clear());
    }

    private IEnumerator Clear()
    {
        Destroy(o, 0.5f);
        yield return null;
    }
}
