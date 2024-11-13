/*****************************************************************************
// File Name :         ScriptableScreenTextUI.cs
// Author :            Nick Rice
//                     
// Creation Date :     10/21/24
//
// Brief Description : This script handles data containers for readable UI elements in the game
*****************************************************************************/

using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The scriptable object for tutorial text and timing
/// </summary>
[CreateAssetMenu(menuName = "UI Data Object/TutorialData")]
public class ScriptableTutorialUI : ScriptableObject
{
    [Tooltip("Tutorial text and time variables")]
    [SerializeField]
    private TextAndTimerData _tutorialData;
    
    #region Getting Total Time
    [Tooltip("How much time the tutorial takes to display")]
    public uint TotalTime { get; private set; }

    /// <summary>
    /// Checks to see if it's null, then performs total time operation
    /// </summary>
    private void Awake()
    {
        _tutorialData.GetText.NullIfEmpty();
        if (IsActive())
        {
            TotalTime  = _tutorialData.GetTimeBeforeText + _tutorialData.GetTimeToDisplay;
        }
    }
    #endregion
    
    /// <summary>
    /// Checks to see if the dialogue data is not null or unused
    /// </summary>
    public bool IsActive()
    {
        bool active = false;

        active = !_tutorialData.GetText.IsUnityNull() && _tutorialData.GetText != "";
        
        return active;
    }
    
    /// <summary>
    /// Returns a reference to tutorial data
    /// </summary>
    public TextAndTimerData GetTextAndTimer()
    {
        return _tutorialData;
    }
}

/// <summary>
/// The scriptable object for dialogue text and timing
/// </summary>
[CreateAssetMenu(menuName = "UI Data Object/DialogueData")]
public class ScriptableDialogueUI : ScriptableObject
{
    [Tooltip("Dialogue text and time variables")]
    [SerializeField]
    private TextAndTimerData[] _dialogueData;

    #region Getting Total Time
    [Tooltip("How much time a chain of dialogue takes to display")]
    public uint TotalTime { get; private set; }

    /// <summary>
    /// Checks to see if it's null, then performs total time operation
    /// </summary>
    private void Awake()
    {
        _dialogueData[0].GetText.NullIfEmpty();
        if (IsActive())
        {
            for (int i = 0; i < _dialogueData.Length; i++)
            {
                TotalTime += _dialogueData[i].GetTimeBeforeText + _dialogueData[i].GetTimeToDisplay;
            }
        }
    }
    #endregion

    /// <summary>
    /// Checks to see if the dialogue data is not null or unused
    /// </summary>
    private bool IsActive()
    {
        bool active = false;

        active = _dialogueData.Length > 0 && _dialogueData != null &&
                 !_dialogueData[0].GetText.IsUnityNull() &&
                 _dialogueData[0].GetText != "";
        
        return active;
    }
    
    /// <summary>
    /// Returns the reference to dialogue data
    /// </summary>
    public TextAndTimerData[] GetTextAndTimer2()
    {
        return _dialogueData;
    }
}

/// <summary>
/// The data class for text and time
/// </summary>
[Serializable]
public struct TextAndTimerData
{
    #region Text/Time
    #region Constructor
    [Tooltip("Data for on screen tutorial text")]
    public TextAndTimerData(string onScreenGetText, uint getTimeUntilNextWords, uint getTimeToDisplay)
    {
        _displayedText = onScreenGetText;
        _getTimeBeforeTextDisplays = getTimeUntilNextWords;
        _getTimeToDisplay = getTimeToDisplay;
    }
    #endregion

    #region Getters and Setters
    [Tooltip("The text you are choosing to display")]
    public string GetText 
    { get => _displayedText; private set => _displayedText = value; }

    [Tooltip("Timing before the text shows up")]
    public uint GetTimeBeforeText 
    { get => _getTimeBeforeTextDisplays; private set => _getTimeBeforeTextDisplays = value; }

    [Tooltip("Timing for all text to display")]
    public uint GetTimeToDisplay 
    { get => _getTimeToDisplay; private set => _getTimeToDisplay = value; }

    #endregion

    #region Private Variables
    [TextArea(1,2)]
    [SerializeField]
    string _displayedText;
    
    [Header("Time variables")]
    [Range(0, 10)]
    [SerializeField]
    uint _getTimeBeforeTextDisplays;
    
    [Range(2, 8)]
    [SerializeField]
    private uint _getTimeToDisplay;

    #endregion

    #endregion
}