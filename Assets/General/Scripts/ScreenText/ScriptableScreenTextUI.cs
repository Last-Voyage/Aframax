/*****************************************************************************
// File Name :         ScriptableScreenTextUI.cs
// Author :            Nick Rice
//                     
// Creation Date :     10/21/24
//
// Brief Description : This script handles data containers for readable UI elements in the game
*****************************************************************************/

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.UI;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

/// <summary>
/// The scriptable object for tutorial text and timing
/// </summary>
[CreateAssetMenu(menuName = "UI Data Object/TutorialData")]
public class ScriptableTutorialUI : ScriptableObject
{
    [Tooltip("Tutorial text and time variables")]
    [SerializeField]
    private TextAndTimerData _tutorialData;

    [Tooltip("How much time the tutorial takes to display")]
    public uint TotalTime { get => TotalTime; private set => TotalTime = value; }

    private void Awake()
    {
        TotalTime  = _tutorialData.GetTimeBeforeText + _tutorialData.GetTimeToDisplay;
    }

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

    [Tooltip("How much time a chain of dialogue takes to display")]
    public uint TotalTime { get => TotalTime; private set => TotalTime = value; }

    private void Awake()
    {
        for (int i = 0; i < _dialogueData.Length; i++)
        {
            TotalTime += _dialogueData[i].GetTimeBeforeText + _dialogueData[i].GetTimeToDisplay;
        }
    }
    
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

/*[CustomPropertyDrawer(typeof(TextAndTimerData))]
public sealed class DialoguePropertyDrawer : PropertyDrawer
{
    /*public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {


        EditorGUI.EndProperty();
    }#1#
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement uiContainer = new();
        Box dialogueInfoBox = new();

        PropertyField textProperty = new PropertyField(property.FindPropertyRelative("_displayedText"));
        PropertyField timeProperty = new PropertyField(property.FindPropertyRelative("_timeBeforeDisplaying"));
        

        TextField dialogueField = new();
        UnsignedIntegerField waitTimeField = new();
        
        dialogueField.label = "Dialogue";
        waitTimeField.label = "Wait Time";
        dialogueInfoBox.name = "DialogueInfoBox";

        dialogueField.value = "Write Text Here";

        textProperty.label = "Dialogue";

        dialogueField.Serialize();
        waitTimeField.Serialize();

        textProperty.style.whiteSpace = WhiteSpace.Normal;

        textProperty.style.overflow = Overflow.Hidden;
        textProperty.style.width = 100;

        dialogueField.multiline = true;
        dialogueField.style.whiteSpace = WhiteSpace.Normal;
        dialogueField.style.unityTextAlign = TextAnchor.UpperLeft;

        dialogueField.labelElement.style.color = Color.white;

        dialogueField.style.maxHeight = 60;

        dialogueField.autoCorrection = true;

        dialogueField.style.color = Color.blue;

        /*uiContainer.Add(dialogueField);
        uiContainer.Add(waitTimeField);
        uiContainer.Add(dialogueInfoBox);#1#
        
        textProperty.Add(dialogueField);
        timeProperty.Add(waitTimeField);
        dialogueInfoBox.Add(textProperty);
        dialogueInfoBox.Add(timeProperty);

        uiContainer.Add(dialogueInfoBox);
        
        return uiContainer;
    }
    
}*/