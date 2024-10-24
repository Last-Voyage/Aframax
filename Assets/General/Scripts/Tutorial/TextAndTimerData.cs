// created 10/23/24
using System;
using UnityEngine;

[Serializable]
public class TextAndTimerData
{
    #region Constructor
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
