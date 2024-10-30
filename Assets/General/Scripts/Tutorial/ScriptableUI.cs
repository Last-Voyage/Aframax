/*****************************************************************************
// File Name :         ScriptableUI.cs
// Author :            Nick Rice
//                     
// Creation Date :     10/21/24
//
// Brief Description : This is a data container for UI elements that are to be included in the game
*****************************************************************************/

using UnityEngine;

/// <summary>
/// The scriptable object for tutorial dialogue and time
/// </summary>
[CreateAssetMenu(menuName = "UI Data Object/TextAndTimeData")]
public class ScriptableUI : ScriptableObject
{
    [Tooltip("The displayed text and the time it takes to display")]
    [SerializeField]
    private TextAndTimerData _textAndTimer;

    public TextAndTimerData GetTextAndTimer()
    {
        return _textAndTimer;
    }
}
