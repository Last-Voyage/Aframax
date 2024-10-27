/*****************************************************************************
// File Name :         ScriptableUI.cs
// Author :            Nick Rice
//                     
// Creation Date :     10/23/24
//
// Brief Description : This script sends a tutorial step completion event, and disables the object
*****************************************************************************/
using UnityEngine;

/// <summary>
/// Sends a tutorial step completion event, and disables the object
/// </summary>
public class TutorialObjectInteract : MonoBehaviour
{
    [SerializeField]
    string _objectTag;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(_objectTag))
        {
            EnvironmentManager.Instance.CompletedTutorial()?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
