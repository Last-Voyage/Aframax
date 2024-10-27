/*****************************************************************************
// File Name :         TutorialObjectInteract.cs
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

    /// <summary>
    /// This makes it so that, when the object with the correct tag interacts with this, it will advance the current
    /// stage of tutorial
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(_objectTag))
        {
            EnvironmentManager.Instance.CompletedTutorial()?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
