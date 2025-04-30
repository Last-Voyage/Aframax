/******************************************************************************
// File Name:       PreviousSelectedUi.cs
// Author:          Nick Rice
// Creation Date:   April 29, 2025
//
// Description:     Causes this Ui to be selected when the player goes back to this screen
******************************************************************************/
using UnityEngine;

/// <summary>
/// Causes this Ui to be selected when the player goes back to this screen
/// </summary>
public class PreviousSelectedUi : MonoBehaviour
{
    /// <summary>
    /// This puts the ui onto the selection stack to be selected when the player goes back to this screen
    /// </summary>
    public void SelectThis()
    {
        UiManager.Instance.AddToSelectionStack(gameObject);
    }
}
