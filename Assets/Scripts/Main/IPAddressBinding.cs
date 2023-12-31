using UnityEngine;
using UnityEngine.UI;

public class IPAddressBinding : MonoBehaviour
{
    public InputField mainInputField;

    // Checks if there is anything entered into the input field.
    void LockInput(InputField input)
    {
        Debug.Log(input.text);
        if (input.text.Length > 0)
        {
            Debug.Log("Text has been entered");
        }
        else if (input.text.Length == 0)
        {
            Debug.Log("Main Input Empty");
        }
    }

    public void Start()
    {
        //Adds a listener that invokes the "LockInput" method when the player finishes editing the main input field.
        //Passes the main input field into the method when "LockInput" is invoked
        mainInputField.onEndEdit.AddListener(delegate { LockInput(mainInputField); });
    }
}
