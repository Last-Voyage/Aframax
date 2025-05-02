/*****************************************************************************
// Name :           StoryManagerEditor.cs
// Author :         Charlie Polonus
// Created :        11/7/2024
// Description :    The custom editor for the StoryManager script
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

/// <summary>
/// The custom editor script for the story manager, makes everything much easier to read/edit
/// </summary>
[CustomEditor(typeof(StoryManager))]
public class StoryManagerEditor : Editor
{
    // Settings for the editor specifically
    private bool _isInDebugMode;
    private static Color _DEFAULT_COLOR;

    // Getters for the StoryManager and the StoryBeats list
    private StoryManager GetStoryManager() => (StoryManager)target;
    private List<StoryBeat> GetStoryBeats() => GetStoryManager().StoryBeats;
    public int OpenStoryBeat
    {
        get { return GetStoryManager().OpenStoryBeat; }
        set { GetStoryManager().OpenStoryBeat = value; }
    }

    /// <summary>
    /// Initialize variables
    /// </summary>
    private void OnEnable()
    {
        _DEFAULT_COLOR = GUI.backgroundColor;
    }

    /// <summary>
    /// Display the custom editor
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Edge cases if the list of story beats doesn't exist
        if (GetStoryBeats() == null
            || GetStoryBeats().Count == 0)
        {
            GetStoryManager().StoryBeats = new();
            GetStoryBeats().Add(new());
        }

        // Debug mode option for those without fear
        _isInDebugMode = EditorGUILayout.Toggle("Debug Mode", _isInDebugMode);
        if (_isInDebugMode)
        {
            base.OnInspectorGUI();
            return;
        }

        // Display the controls to move through the individual story beats
        DisplayStoryBeatControls();

        // Show the currently selected story beat
        DisplayStoryBeat(GetStoryBeats(), OpenStoryBeat);

        // Editor spacing
        EditorGUILayout.Space(8);
        DrawGUILine(1);
        EditorGUILayout.Space(8);

        // Buttons for creating a new event to add on to the end of the existing list of event
        GUI.backgroundColor = new(0.25f, 1f, 0.25f, 1);
        if (GUILayout.Button("Create New Event"))
        {
            // Change the currently open story beat, in case it got moved around
            OpenStoryBeat = Mathf.Clamp(OpenStoryBeat, 0, GetStoryBeats().Count - 1);

            // Edge case if the story beat events list doesn't exist
            if (GetStoryBeats()[OpenStoryBeat].StoryBeatEvents == null)
            {
                GetStoryManager().StoryBeats[OpenStoryBeat].StoryBeatEvents = new();
            }

            // Add a new story beat event to the end of all the events
            GetStoryManager().StoryBeats[OpenStoryBeat].StoryBeatEvents.Add(new StoryBeatEvent());
        }
        GUI.backgroundColor = _DEFAULT_COLOR;

        // Make sure the changes actually saved
        if (GUI.changed && !Application.isPlaying)
        {
            EditorUtility.SetDirty(GetStoryManager());
            EditorSceneManager.MarkSceneDirty(GetStoryManager().gameObject.scene);
        }
    }

    /// <summary>
    /// Display controls for moving around between individual story beats
    /// </summary>
    private void DisplayStoryBeatControls()
    {
        // Edge case if the current open story beat doesn't exist
        if (GetStoryBeats()[OpenStoryBeat].StoryBeatEvents == null)
        {
            GetStoryManager().StoryBeats[OpenStoryBeat].StoryBeatEvents = new();
        }

        EditorGUILayout.BeginHorizontal();
        // The button to add a beat before the current one
        GUI.backgroundColor = new(0.25f, 1f, 0.25f, 1);
        if (GUILayout.Button("+"))
        {
            // Add a story beat before the current and select the new one
            GetStoryManager().StoryBeats.Insert(OpenStoryBeat, new());
        }
        GUI.backgroundColor = _DEFAULT_COLOR;

        // The button to move to the previous story beat
        if (GUILayout.Button("<"))
        {
            OpenStoryBeat = Mathf.Clamp(OpenStoryBeat - 1, 0, GetStoryBeats().Count - 1);
        }

        // Draw the label for the currently active story beat
        GUIStyle labelStyle = new(GUI.skin.label);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label(OpenStoryBeat + " / " + (GetStoryBeats().Count - 1), labelStyle);

        // The button to move to the next story beat
        if (GUILayout.Button(">"))
        {
            OpenStoryBeat = Mathf.Clamp(OpenStoryBeat + 1, 0, GetStoryBeats().Count - 1);
        }

        // The button to add a beat after the current one
        GUI.backgroundColor = new(0.25f, 1f, 0.25f, 1);
        if (GUILayout.Button("+"))
        {
            // Add a story beat after the current and select the new one
            GetStoryManager().StoryBeats.Insert(OpenStoryBeat + 1, new());
            OpenStoryBeat++;
        }
        GUI.backgroundColor = _DEFAULT_COLOR;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(16);

        EditorGUILayout.BeginHorizontal();
        // Display the name of the currently selected beat
        GetStoryBeats()[OpenStoryBeat].BeatName = EditorGUILayout.TextField(GetStoryBeats()[OpenStoryBeat].BeatName);

        // Allow the current beat to be deleted
        GUI.backgroundColor = new(1f, 0.25f, 0.25f, 1);
        if (GUILayout.Button("Delete Beat"))
        {
            // Make sure there are actually beats to delete
            if (GetStoryBeats().Count > 1)
            {
                // Fix variables to correctly match the new range
                GetStoryBeats().RemoveAt(OpenStoryBeat);
                OpenStoryBeat = Mathf.Clamp(OpenStoryBeat, 0, GetStoryBeats().Count - 1);
            }
        }
        GUI.backgroundColor = _DEFAULT_COLOR;
        EditorGUILayout.EndHorizontal();

        // Allow the description to be word wrapped
        GUIStyle textAreaStyle = new(GUI.skin.textArea);
        textAreaStyle.wordWrap = true;

        // Display the description of the currently selected beat
        GetStoryBeats()[OpenStoryBeat].BeatDescription
            = EditorGUILayout.TextArea(GetStoryBeats()[OpenStoryBeat].BeatDescription, textAreaStyle);

        // Settings for triggering a story beat in the inspector
        EditorGUILayout.BeginHorizontal();
        GetStoryBeats()[OpenStoryBeat].TriggerOnStart
            = EditorGUILayout.Toggle("Trigger on Start", GetStoryBeats()[OpenStoryBeat].TriggerOnStart);
        if (GUILayout.Button("Trigger Story Beat") && Application.isPlaying)
        {
            StoryManager.Instance.TriggerStoryBeat(GetStoryBeats()[OpenStoryBeat]);
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Display the current story beat
    /// </summary>
    /// <param name="storyBeats">The full list of all story beats</param>
    /// <param name="index">The index of the currently active story beat</param>
    private void DisplayStoryBeat(List<StoryBeat> storyBeats, int index)
    {
        // Edge case if the story beat list doesn't exist
        if (storyBeats == null)
        {
            return;
        }

        // Get the list of all events
        List<StoryBeatEvent> storyBeatEvents = storyBeats[index].StoryBeatEvents;

        // Edge case if the list of events doesn't exist
        if (storyBeatEvents == null)
        {
            return;
        }

        // Display each individual story beat event
        for (int i = 0; i < storyBeatEvents.Count; i++)
        {
            // Editor spacing
            EditorGUILayout.Space(8);
            DrawGUILine(1);

            // Display the story beat event
            DisplayStoryBeatEvent(storyBeatEvents, index, i);
        }
    }

    /// <summary>
    /// Displays the individual story beat event
    /// </summary>
    /// <param name="storyBeatEvents">The full list of all story beats</param>
    /// <param name="beatIndex">The index of the beat</param>
    /// <param name="eventIndex">The index of the event</param>
    private void DisplayStoryBeatEvent(List<StoryBeatEvent> storyBeatEvents, int beatIndex, int eventIndex)
    {
        // Setting up the serialized objects/properties for the story manager
        SerializedObject serializedStoryManager = new(GetStoryManager());
        SerializedProperty storyBeatsProperty = serializedStoryManager.FindProperty("StoryBeats");

        // Setting up the serialized objects/properties for the selected beat
        SerializedProperty beatProperty = storyBeatsProperty.GetArrayElementAtIndex(beatIndex);
        SerializedProperty storyBeatEventsProperty = beatProperty.FindPropertyRelative("StoryBeatEvents");

        // Setting up the serialized objects/properties for the selected event
        SerializedProperty storyBeatEventProperty = storyBeatEventsProperty.GetArrayElementAtIndex(eventIndex);

        // Bold header style
        GUIStyle headerStyle = new(GUI.skin.label);
        headerStyle.fontStyle = FontStyle.Bold;

        GUILayout.BeginHorizontal();
        // Figure out the title to display based on the event name
        string eventName = storyBeatEvents[eventIndex].EventName;
        if (eventName == "" || eventName == null)
        {
            // Either set it to its actual name, or a placeholder
            eventName = "Event " + eventIndex + ": " + storyBeatEvents[eventIndex].EventType.ToString();
        }

        // Display the minimize/maximize button
        if (GUILayout.Button(storyBeatEvents[eventIndex].IsMinimized ? "+" : "-"))
        {
            storyBeatEvents[eventIndex].IsMinimized = !storyBeatEvents[eventIndex].IsMinimized;
        }

        // Display the name, or display the name text field
        if (storyBeatEvents[eventIndex].IsMinimized)
        {
            EditorGUILayout.LabelField(eventName, headerStyle);
        }
        else
        {
            // If it's not minimized, allow the user to edit the name
            storyBeatEvents[eventIndex].EventName = EditorGUILayout.TextField(storyBeatEvents[eventIndex].EventName);
        }

        // Display the "move back in the list" button
        if (GUILayout.Button("↑"))
        {
            MoveItem(storyBeatEvents, eventIndex, eventIndex - 1);
        }

        // Display the "move forward in the list" button
        if (GUILayout.Button("↓"))
        {
            MoveItem(storyBeatEvents, eventIndex, eventIndex + 1);
        }

        // Display the delete event button
        GUI.backgroundColor = new(1f, 0.25f, 0.25f, 1);
        if (GUILayout.Button("-"))
        {
            // Delete the event
            storyBeatEvents.Remove(storyBeatEvents[eventIndex]);
            // If suddenly the events need to stop displaying, cancel the temporary GUI changes
            if (storyBeatEvents.Count == 0)
            {
                // Restting editor settings
                GUI.backgroundColor = _DEFAULT_COLOR;
                GUILayout.EndHorizontal();
                return;
            }
        }
        GUI.backgroundColor = _DEFAULT_COLOR;
        GUILayout.EndHorizontal();

        // If the event is minimized or doesn't exist anymore, don't display the event
        if (eventIndex >= storyBeatEvents.Count ||
            storyBeatEvents[eventIndex].IsMinimized)
        {
            return;
        }

        GUILayout.BeginHorizontal();
        // Display the event type buttons
        for (int i = 0; i < 4; i++)
        {
            // Check if the current event type is correct, and change the color accordingly
            bool isEventType = (int)storyBeatEvents[eventIndex].EventType == i;
            if (isEventType)
            {
                GUI.backgroundColor = new(0.5f, 0.5f, 1f, 1);
            }

            // Display the event type buttons
            if (GUILayout.Button((StoryBeatEvent.EBeatEventType)i + ""))
            {
                storyBeatEvents[eventIndex].EventType = (StoryBeatEvent.EBeatEventType)i;
            }

            GUI.backgroundColor = _DEFAULT_COLOR;
        }
        GUILayout.EndHorizontal();

        // Show the individual editor event settings
        EditorGUI.indentLevel++;
        switch (storyBeatEvents[eventIndex].EventType)
        {
            // If it's dialogue, show the dialogue settings
            case StoryBeatEvent.EBeatEventType.Dialogue:
                // TODO: Implement dialogue line things
                storyBeatEvents[eventIndex].Dialogue
                    = (ScriptableDialogueUi)EditorGUILayout.ObjectField(
                        storyBeatEvents[eventIndex].Dialogue,
                        typeof(ScriptableDialogueUi),
                        true);
                break;

            // If it's boat speed change, show the boat speed settings
            case StoryBeatEvent.EBeatEventType.BoatSpeed:
                storyBeatEvents[eventIndex].BoatSpeed
                    = EditorGUILayout.FloatField("Boat Speed", storyBeatEvents[eventIndex].BoatSpeed);
                storyBeatEvents[eventIndex].SpeedChangeTime
                    = EditorGUILayout.FloatField("Change Time", storyBeatEvents[eventIndex].SpeedChangeTime);
                break;

            // If it's a boss attack, show the boss attack settings
            case StoryBeatEvent.EBeatEventType.BossAttack:
                // Edge case if the boss attacks don't exist
                if (storyBeatEvents[eventIndex].BossAttacks == null)
                {
                    storyBeatEvents[eventIndex].BossAttacks = new();
                }

                // Display the list of boss attacks
                SerializedProperty attackListProperty = storyBeatEventProperty.FindPropertyRelative("BossAttacks");
                EditorGUILayout.PropertyField(attackListProperty);
                serializedStoryManager.ApplyModifiedProperties();
                break;

            // If it's a UnityEvent, display the event
            case StoryBeatEvent.EBeatEventType.Function:
                // Display the function properties
                SerializedProperty beatEventProperty = storyBeatEventProperty.FindPropertyRelative("OnBeatEvent");
                EditorGUILayout.PropertyField(beatEventProperty);
                serializedStoryManager.ApplyModifiedProperties();
                break;
        }

        EditorGUI.indentLevel--;

        GUILayout.Space(4);

        GUILayout.BeginHorizontal();
        // Display the settings for the delay on the beat event
        storyBeatEvents[eventIndex].DelayTime
            = EditorGUILayout.FloatField("Delay Time", storyBeatEvents[eventIndex].DelayTime);

        if (storyBeatEvents[eventIndex].EventType == StoryBeatEvent.EBeatEventType.Dialogue)
        {
            if (GUILayout.Button("Set Auto Delay"))
            {
                storyBeatEvents[eventIndex].DelayTime = storyBeatEvents[eventIndex].Dialogue.TotalTime;
            }
        }

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw a horizontal line across the inspector
    /// </summary>
    /// <param name="height">The height of the line</param>
    private void DrawGUILine(int height = 1)
    {
        // Draw the line at the current position
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    /// <summary>
    /// Move an item in a list to a new index
    /// </summary>
    /// <typeparam name="T">The list type</typeparam>
    /// <param name="list">The list object</param>
    /// <param name="oldIndex">The old index of the item</param>
    /// <param name="newIndex">The new index to move the item to</param>
    private void MoveItem<T>(List<T> targetList, int oldIndex, int newIndex)
    {
        // Clamp the index to the size of the targetList
        oldIndex = Mathf.Clamp(oldIndex, 0, targetList.Count - 1);
        newIndex = Mathf.Clamp(newIndex, 0, targetList.Count - 1);

        // Move the item
        T item = targetList[oldIndex];
        targetList.RemoveAt(oldIndex);
        targetList.Insert(newIndex, item);
    }
}