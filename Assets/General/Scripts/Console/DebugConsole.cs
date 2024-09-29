/*
// Name: DebugConsole.CS
// Author: Nabil Tagba
// Overview: Hosts one function
// which handles the debug console commands
 */
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DebugConsole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField _commandInput;

    public void RunDebugCommand()
    {
        // check for system commands
        if (_commandInput.text.Substring(0,3) == "ZUG")
        {
            //handle system commands
            if (_commandInput.text.Substring(4, _commandInput.text.Length-4) == "Quit()")
            {
                print("Attempting to Quite Game");
                Application.Quit();

            }

            if (_commandInput.text.Substring(4, _commandInput.text.Length-4) == "Reload()")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        // else just print the input value
        else
        {
            print(_commandInput.text);
        }
    }

}
