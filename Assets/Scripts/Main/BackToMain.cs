using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMain : MonoBehaviour
{
    public void OnClick()
    {
        //Debug.Log("BackToMain");
        SceneManager.LoadScene("MainScene");//�л�������CardDeckScene
    }
}
