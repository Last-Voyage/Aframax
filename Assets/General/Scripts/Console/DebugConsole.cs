/***********************************************************************************************************************
// Name: DebugConsole.CS
// Author: Nabil Tagba
// Overview: Hosts one function
// which handles the debug console commands
***********************************************************************************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//only works in engine or development builds
#if DEVELOPMENT_BUILD || UNITY_EDITOR

/// <summary>
/// Contains the functionality for the 
/// Debug commands
/// </summary>
public class DebugConsole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField _commandInput;

    /// <summary>
    /// checks the debug commands and runs the 
    /// right code depending on the command
    /// </summary>
    public void RunDebugCommand()
    {
        // check for system commands
        if (_commandInput.text.Substring(0, 3) == "ZUG")
        {
            //handle system commands
            if (_commandInput.text.Substring(4, _commandInput.text.Length - 4) == "Quit()")
            {
                Application.Quit();
            }
            else if (_commandInput.text.Substring(4, _commandInput.text.Length - 4) == "Reload()")
            {
                AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(SceneManager.GetActiveScene().buildIndex, 0);
            }
            else if (_commandInput.text.Substring(4, _commandInput.text.Length - 4) == "DrawColliders()")
            {
                print("Functionality not implemented yet");
            }
        }
        // else just print the input value
        else
        {
            print(_commandInput.text);
        }
    }
}


#endif

