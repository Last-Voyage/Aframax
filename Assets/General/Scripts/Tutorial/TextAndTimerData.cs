/*****************************************************************************
// File Name :         TextAndTimerData.cs
// Author :            Nick Rice
//                     
// Creation Date :     10/23/24
//
// Brief Description : The data class for text and time
*****************************************************************************/
using System;
using UnityEngine;

/// <summary>
/// The data class for text and time
/// </summary>
[Serializable]
public class TextAndTimerData
{
    #region Constructor
    [Tooltip("Object for on screen text and timing until the words are displayed")]
    public TextAndTimerData(string onScreenText, int timeUntilNextWords)
    {
        _displayedText = onScreenText;
        _timeBeforeDisplaying = timeUntilNextWords;
    }
    #endregion

    #region Getters and Setters
    [Tooltip("The text you are choosing to display")]
    public string Text { get => _displayedText; private set => _displayedText = value; }

    [Tooltip("The time value before the text shows up")]
    public int Time { get => _timeBeforeDisplaying; private set => _timeBeforeDisplaying = value; }
    #endregion

    #region Private Variables

    [Space]
    [TextArea(1,2)]
    [SerializeField]
    string _displayedText;

    [Range(0, 20)]
    [SerializeField]
    int _timeBeforeDisplaying;
    #endregion
}
