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
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _optionsButton;
    /// <summary>
    /// disables the continue button when there is no save data
    /// </summary>
    private void Awake()
    {
        //checks for gameplay save data, disables if there's none 
        _continueButton.interactable = SaveManager._hasSavedGameplayData;

        //Adjust the navigation when the continue button is disabled
        if(!SaveManager._hasSavedGameplayData )
        {
            Navigation newGameNavigation = _newGameButton.navigation;
            newGameNavigation.mode = Navigation.Mode.Explicit;
            newGameNavigation.selectOnDown = _optionsButton;
            _newGameButton.navigation = newGameNavigation;

            newGameNavigation = _optionsButton.navigation;
            newGameNavigation.mode=Navigation.Mode.Explicit;
            newGameNavigation.selectOnUp = _newGameButton;
            _optionsButton.navigation = newGameNavigation;
        }
    }
}
