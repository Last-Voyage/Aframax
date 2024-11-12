using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEditorInternal;

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
            DisplayStoryBeatEvent(storyBeatEvents, i);
        }
    }

    private void DisplayStoryBeatEvent(List<StoryBeatEvent> storyBeatEvents, int index)
    {
        GUIStyle headerStyle = new(GUI.skin.label);
        headerStyle.fontStyle = FontStyle.Bold;

        GUILayout.Label("Event " + index, headerStyle);

        GUILayout.BeginHorizontal();
        for (int i = 0; i < 4; i++)
        {
            bool isEventType = (int)storyBeatEvents[index].EventType == i;

            Color defaultColor = GUI.backgroundColor;

            if (isEventType)
            {
                GUI.backgroundColor = new(0.5f, 0.5f, 1f, 1);
            }

            if (GUILayout.Button((StoryBeatEvent.BeatEventType)i + ""))
            {
                storyBeatEvents[index].EventType = (StoryBeatEvent.BeatEventType)i;
            }

            GUI.backgroundColor = defaultColor;
        }
        GUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        switch (storyBeatEvents[index].EventType)
        {
            case StoryBeatEvent.BeatEventType.Dialogue:

                for (int i = 0; i < storyBeatEvents[index].DialogueLines.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    storyBeatEvents[index].DialogueLines[i] = (DialogueLine)EditorGUILayout.ObjectField(storyBeatEvents[index].DialogueLines[i], typeof(DialogueLine), true);
                    if (GUILayout.Button("Delete"))
                    {
                        storyBeatEvents[index].DialogueLines.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Dialogue"))
                {
                    storyBeatEvents[index].DialogueLines.Add(null);
                }

                storyBeatEvents[index].IsShuffled = EditorGUILayout.Toggle("Shuffled", storyBeatEvents[index].IsShuffled);
                storyBeatEvents[index].IsLoop = EditorGUILayout.Toggle("Loop", storyBeatEvents[index].IsLoop);
                if (storyBeatEvents[index].IsLoop)
                {
                    storyBeatEvents[index].MinimumWaitTime = EditorGUILayout.FloatField("Min Wait Time", storyBeatEvents[index].MinimumWaitTime);
                    storyBeatEvents[index].MaximumWaitTime = EditorGUILayout.FloatField("Max Wait Time", storyBeatEvents[index].MaximumWaitTime);
                }

                break;
            case StoryBeatEvent.BeatEventType.BoatSpeed:

                storyBeatEvents[index].BoatSpeed = EditorGUILayout.FloatField("Boat Speed", storyBeatEvents[index].BoatSpeed);
                storyBeatEvents[index].SpeedChangeTime = EditorGUILayout.FloatField("Change Time", storyBeatEvents[index].SpeedChangeTime);

                break;
            case StoryBeatEvent.BeatEventType.BossAttack:
                break;
            case StoryBeatEvent.BeatEventType.Function:

                break;
        }

        EditorGUI.indentLevel--;

        GUILayout.Space(4);

        GUILayout.BeginHorizontal();

        storyBeatEvents[index].DelayTime = EditorGUILayout.FloatField("Delay Time", storyBeatEvents[index].DelayTime);
        if (GUILayout.Button("Delete Event"))
        {
            storyBeatEvents.Remove(storyBeatEvents[index]);
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
