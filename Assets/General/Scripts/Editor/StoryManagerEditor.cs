using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(StoryManager))]
public class StoryManagerEditor : Editor
{
    private int _openStoryBeat = 0;
    private bool _debugMode;

    StoryManager GetStoryManager => (StoryManager)target;
    List<StoryBeat> GetStoryBeats => GetStoryManager.StoryBeats;

    public override void OnInspectorGUI()
    {
        if (GetStoryBeats == null
            || GetStoryBeats.Count == 0)
        {
            GetStoryManager.StoryBeats = new();
            GetStoryBeats.Add(new());
        }

        _debugMode = EditorGUILayout.Toggle("Debug Mode", _debugMode);
        if (_debugMode)
        {
            base.OnInspectorGUI();
            return;
        }

        DisplayStoryBeatControls();

        if (GUILayout.Button("Create New Event"))
        {
            _openStoryBeat = Mathf.Clamp(_openStoryBeat, 0, GetStoryBeats.Count - 1);

            if (GetStoryBeats[_openStoryBeat].StoryBeatEvents == null)
            {
                GetStoryManager.StoryBeats[_openStoryBeat].StoryBeatEvents = new();
            }
            GetStoryManager.StoryBeats[_openStoryBeat].StoryBeatEvents.Add(new StoryBeatEvent());
        }

        DisplayStoryBeat(GetStoryBeats, _openStoryBeat);
    }

    private void DisplayStoryBeatControls()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+"))
        {
            if (GetStoryBeats[_openStoryBeat].StoryBeatEvents == null)
            {
                GetStoryManager.StoryBeats[_openStoryBeat].StoryBeatEvents = new();
            }
            GetStoryManager.StoryBeats.Insert(_openStoryBeat, new());
            _openStoryBeat++;
        }
        if (GUILayout.Button("<"))
        {
            _openStoryBeat = Mathf.Clamp(_openStoryBeat - 1, 0, GetStoryBeats.Count - 1);
        }
        GUIStyle labelStyle = new(GUI.skin.label);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label(_openStoryBeat + " - " + (GetStoryBeats.Count - 1), labelStyle);
        if (GUILayout.Button(">"))
        {
            _openStoryBeat = Mathf.Clamp(_openStoryBeat + 1, 0, GetStoryBeats.Count - 1);
        }
        if (GUILayout.Button("+"))
        {
            if (GetStoryBeats[_openStoryBeat].StoryBeatEvents == null)
            {
                GetStoryManager.StoryBeats[_openStoryBeat].StoryBeatEvents = new();
            }
            GetStoryManager.StoryBeats.Insert(_openStoryBeat + 1, new());
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(16);

        EditorGUILayout.BeginHorizontal();

        GetStoryBeats[_openStoryBeat].Name = EditorGUILayout.TextField(GetStoryBeats[_openStoryBeat].Name);
        if (GUILayout.Button("Delete Beat"))
        {
            if (GetStoryBeats.Count > 1)
            {
                GetStoryBeats.RemoveAt(_openStoryBeat);
                _openStoryBeat = Mathf.Clamp(_openStoryBeat, 0, GetStoryBeats.Count - 1);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayStoryBeat(List<StoryBeat> storyBeats, int index)
    {
        if (storyBeats == null)
        {
            return;
        }

        List<StoryBeatEvent> storyBeatEvents = storyBeats[index].StoryBeatEvents;

        if (storyBeatEvents == null)
        {
            return;
        }

        for (int i = 0; i < storyBeatEvents.Count; i++)
        {
            EditorGUILayout.Space(8);
            DrawGUILine(1);
            DisplayStoryBeatEvent(storyBeatEvents, index, i);
        }
    }

    private void DisplayStoryBeatEvent(List<StoryBeatEvent> storyBeatEvents, int beatIndex, int eventIndex)
    {
        SerializedObject serializedStoryManager = new(GetStoryManager);
        SerializedProperty storyBeatsProperty = serializedStoryManager.FindProperty("StoryBeats");

        SerializedProperty beatProperty = storyBeatsProperty.GetArrayElementAtIndex(beatIndex);
        SerializedProperty storyBeatEventsProperty = beatProperty.FindPropertyRelative("StoryBeatEvents");

        SerializedProperty storyBeatEventProperty = storyBeatEventsProperty.GetArrayElementAtIndex(eventIndex);

        GUIStyle headerStyle = new(GUI.skin.label);
        headerStyle.fontStyle = FontStyle.Bold;

        GUILayout.Label("Event " + eventIndex, headerStyle);

        GUILayout.BeginHorizontal();
        for (int i = 0; i < 4; i++)
        {
            bool isEventType = (int)storyBeatEvents[eventIndex].EventType == i;

            Color defaultColor = GUI.backgroundColor;

            if (isEventType)
            {
                GUI.backgroundColor = new(0.5f, 0.5f, 1f, 1);
            }

            if (GUILayout.Button((StoryBeatEvent.BeatEventType)i + ""))
            {
                storyBeatEvents[eventIndex].EventType = (StoryBeatEvent.BeatEventType)i;
            }

            GUI.backgroundColor = defaultColor;
        }
        GUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        switch (storyBeatEvents[eventIndex].EventType)
        {
            case StoryBeatEvent.BeatEventType.Dialogue:

                // TODO: Implement dialogue line things
                // 
                // As an example:
                storyBeatEvents[eventIndex].DialogueLine = (DialogueLine)EditorGUILayout.ObjectField(storyBeatEvents[eventIndex].DialogueLine, typeof(DialogueLine), true);

                //for (int i = 0; i < storyBeatEvents[eventIndex].DialogueLines.Count; i++)
                //{
                //    GUILayout.BeginHorizontal();

                //    storyBeatEvents[eventIndex].DialogueLines[i] = (DialogueLine)EditorGUILayout.ObjectField(storyBeatEvents[eventIndex].DialogueLines[i], typeof(DialogueLine), true);
                //    if (GUILayout.Button("Delete"))
                //    {
                //        storyBeatEvents[eventIndex].DialogueLines.RemoveAt(i);
                //    }

                //    GUILayout.EndHorizontal();
                //}

                //if (GUILayout.Button("Add Dialogue"))
                //{
                //    storyBeatEvents[eventIndex].DialogueLines.Add(null);
                //}

                //storyBeatEvents[eventIndex].IsShuffled = EditorGUILayout.Toggle("Shuffled", storyBeatEvents[eventIndex].IsShuffled);
                //storyBeatEvents[eventIndex].IsLoop = EditorGUILayout.Toggle("Loop", storyBeatEvents[eventIndex].IsLoop);
                //if (storyBeatEvents[eventIndex].IsLoop)
                //{
                //    storyBeatEvents[eventIndex].MinimumWaitTime = EditorGUILayout.FloatField("Min Wait Time", storyBeatEvents[eventIndex].MinimumWaitTime);
                //    storyBeatEvents[eventIndex].MaximumWaitTime = EditorGUILayout.FloatField("Max Wait Time", storyBeatEvents[eventIndex].MaximumWaitTime);
                //}

                break;
            case StoryBeatEvent.BeatEventType.BoatSpeed:

                storyBeatEvents[eventIndex].BoatSpeed = EditorGUILayout.FloatField("Boat Speed", storyBeatEvents[eventIndex].BoatSpeed);
                storyBeatEvents[eventIndex].SpeedChangeTime = EditorGUILayout.FloatField("Change Time", storyBeatEvents[eventIndex].SpeedChangeTime);

                break;
            case StoryBeatEvent.BeatEventType.BossAttack:

                if (storyBeatEvents[eventIndex].BossAttacks == null)
                {
                    storyBeatEvents[eventIndex].BossAttacks = new();
                }

                SerializedProperty attackListProperty = storyBeatEventProperty.FindPropertyRelative("BossAttacks");
                EditorGUILayout.PropertyField(attackListProperty);
                serializedStoryManager.ApplyModifiedProperties();

                //for (int i = 0; i < storyBeatEvents[eventIndex].BossAttacks.Count; i++)
                //{
                //    GUILayout.BeginHorizontal();

                //    storyBeatEvents[eventIndex].BossAttacks[i] = (BaseBossAttack)EditorGUILayout.ObjectField(storyBeatEvents[eventIndex].BossAttacks[i], typeof(BaseBossAttack), true);
                //    if (GUILayout.Button("Delete"))
                //    {
                //        storyBeatEvents[eventIndex].BossAttacks.RemoveAt(i);
                //    }

                //    GUILayout.EndHorizontal();
                //}

                //if (GUILayout.Button("Add Attack"))
                //{
                //    storyBeatEvents[eventIndex].BossAttacks.Add(null);
                //}

                break;
            case StoryBeatEvent.BeatEventType.Function:

                // Courtesy of #praetorblue on Discord

                SerializedProperty beatEventProperty = storyBeatEventProperty.FindPropertyRelative("BeatEvent");
                EditorGUILayout.PropertyField(beatEventProperty);
                serializedStoryManager.ApplyModifiedProperties();

                //for (int i = 0; i < storyBeatsProp.arraySize; i++)
                //{
                //    SerializedProperty beatProp = storyBeatsProp.GetArrayElementAtIndex(i);
                //    SerializedProperty beatEventsProp = beatProp.FindPropertyRelative("StoryBeatEvents");
                //    for (int j = 0; j < beatEventsProp.arraySize; j++)
                //    {
                //        SerializedProperty sbeProp = beatEventsProp.GetArrayElementAtIndex(j);
                //        SerializedProperty eventToRunProp = sbeProp.FindPropertyRelative("EventToRun");
                //        EditorGUILayout.PropertyField(eventToRunProp); // for example
                //    }
                //}



                //SerializedProperty eventProperty = serializedObject.FindProperty("UnityEvent");
                //EditorGUILayout.PropertyField(eventProperty);
                //eventObject.ApplyModifiedProperties();

                break;
        }

        EditorGUI.indentLevel--;

        GUILayout.Space(4);

        GUILayout.BeginHorizontal();

        storyBeatEvents[eventIndex].DelayTime = EditorGUILayout.FloatField("Delay Time", storyBeatEvents[eventIndex].DelayTime);
        if (GUILayout.Button("Delete Event"))
        {
            storyBeatEvents.Remove(storyBeatEvents[eventIndex]);
        }

        GUILayout.EndHorizontal();
    }

    private void DrawGUILine(int height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}