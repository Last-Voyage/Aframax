/*****************************************************************************
// File Name :         DisableContinueBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     4/30/25
//
// Brief Description : disables the continue button when there is no save data
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// disables the continue button when there is no save data
/// </summary>
public class DisableContinueBehaviour : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    /// <summary>
    /// disables the continue button when there is no save data
    /// </summary>
    private void Awake()
    {
        //checks for gameplay save data, disables if there's none 
        _continueButton.interactable = SaveManager._hasSavedGameplayData;
    }
}
