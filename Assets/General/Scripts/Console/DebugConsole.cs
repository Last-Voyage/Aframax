using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugConsole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField _commandInput;

    public void RunDebugCommand()
    {
        // check for system commands
        if (_commandInput.text.Substring(0,3) == "ZUG")
        {
            print("No current system commands implemented");
        }
        // else just print the input value
        else
        {
            print(_commandInput.text);
        }
    }

}
