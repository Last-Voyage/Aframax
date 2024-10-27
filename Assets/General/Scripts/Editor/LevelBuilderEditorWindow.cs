using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class LevelBuilderEditorWindow : EditorWindow
{
    [MenuItem("Window/Level Builder 2")]
    public static void ShowWindow()
    {
        GetWindow<LevelBuilderEditorWindow>("Level Builder");
    }

    private GameObject[] _levelChunkOptions;
    private Editor[] _levelChunkOptionEditors;

    private EditorWindowSettings _settings;
    private string _previousTextAreaText;
    private string _textAreaText;
    private int[] _lastThreeValues;

    private Vector2 GetWindowSize => position.size;

    private void OnEnable()
    {
        _textAreaText = "";
        _settings = new();
        _settings.EdgeSpacing = new(10, 10);

        _lastThreeValues = new int[3]
        {
            -1, -1, -1
        };

        RefreshLevelChunkOptions();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _levelChunkOptionEditors.Length; i++)
        {
            _levelChunkOptionEditors[i].DiscardChanges();
        }
    }

    private void OnGUI()
    {
        if (!FindObjectOfType<IterativeChunkLoad>())
        {
            GUILayout.Label("\nUnable to be used here, try adding an object with the IterativeChunkLoad script");
            return;
        }

        if (_textAreaText != _previousTextAreaText)
        {
            RefreshLastValues();
            _previousTextAreaText = _textAreaText;
        }

        float displacement = DisplayTextFileArea();
        DisplayChunkOptionButtons(displacement);
    }

    private float DisplayTextFileArea()
    {
        Vector2 buttonSpacing = new(5, 5);

        Vector2 textAreaSize = new();
        textAreaSize.x = (GetWindowSize.x - (_settings.EdgeSpacing.x * 2) - buttonSpacing.x) * 0.75f;
        textAreaSize.y = 200f;

        Vector2 textAreaPosition = _settings.EdgeSpacing;

        Rect textAreaRect = new(textAreaPosition, textAreaSize);
        _textAreaText = GUI.TextArea(textAreaRect, _textAreaText);

        Vector2 sideButtonSize;
        sideButtonSize.x = (GetWindowSize.x - (_settings.EdgeSpacing.x * 2) - buttonSpacing.x) * 0.25f;
        sideButtonSize.y = (textAreaSize.y - (2 * buttonSpacing.y)) / 3f;

        Vector2 buttonOffset = new();
        buttonOffset.x = textAreaSize.x + buttonSpacing.x;
        buttonOffset.y = 0;
        Rect buttonRect = new(_settings.EdgeSpacing + buttonOffset, sideButtonSize);
        if (GUI.Button(buttonRect, "Load Level from\nSave File"))
        {
            _textAreaText = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/ChunkQueue.txt");
        }

        buttonOffset.y = sideButtonSize.y + buttonSpacing.y;
        buttonRect = new(_settings.EdgeSpacing + buttonOffset, sideButtonSize);
        bool canWork = ValidTextArea();
        GUIStyle style = new("button");
        style.richText = true;
        string buttonName = "Save Current Level\nOrder to File" + (!canWork ? "\n<color=red>Warning! Will not work!</color>" : "");
        if (GUI.Button(buttonRect, buttonName, style))
        {
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/ChunkQueue.txt", _textAreaText);
        }

        buttonOffset.y = (sideButtonSize.y + buttonSpacing.y) * 2;
        sideButtonSize.y = ((textAreaSize.y - (3 * buttonSpacing.y)) / 3f) * 0.7f;
        buttonRect = new(_settings.EdgeSpacing + buttonOffset, sideButtonSize);

        if (GUI.Button(buttonRect, "Refresh Chunk List\n(Below)", style))
        {
            RefreshLevelChunkOptions();
        }

        float chunkDistance = FindObjectOfType<IterativeChunkLoad>().DistanceBetweenChunks;
        buttonOffset.y += buttonSpacing.y + sideButtonSize.y;
        buttonOffset.x += sideButtonSize.x * 0.75f;
        sideButtonSize.y *= 0.4f;
        sideButtonSize.x /= 4f;
        buttonRect = new(_settings.EdgeSpacing + buttonOffset, sideButtonSize);
        chunkDistance = EditorGUI.FloatField(buttonRect, chunkDistance);
        FindObjectOfType<IterativeChunkLoad>().SetChunkDistance(chunkDistance);

        buttonOffset.x -= sideButtonSize.x * 3;
        sideButtonSize.x *= 3f;
        buttonRect = new(_settings.EdgeSpacing + buttonOffset, sideButtonSize);
        EditorGUI.LabelField(buttonRect, "Chunk Size");

        return textAreaSize.y + (2 * buttonSpacing.y);
    }

    private void DisplayChunkOptionButtons(float extraAboveSpace)
    {
        if (_levelChunkOptions == null
            || _levelChunkOptions.Length == 0)
        {
            return;
        }

        Vector2 buttonSize = new(75, 90);
        Vector2 buttonSpacing = new(5, 5);

        int buttonCount = _levelChunkOptions.Length;
        int buttonsPerLine = buttonCount;

        for (int i = 0; i < buttonCount + 1; i++)
        {
            float currentTotalWidth = (i * buttonSize.x) + ((i - 1) * buttonSpacing.x) + (_settings.EdgeSpacing.x * 2);

            if (currentTotalWidth > GetWindowSize.x)
            {
                buttonsPerLine = i - 1;
                break;
            }
        }

        for (int i = 0; i < buttonCount; i++)
        {
            Vector2 buttonPosition = new();
            buttonPosition.x = _settings.EdgeSpacing.x + ((buttonSize.x + buttonSpacing.x) * (i % buttonsPerLine));
            buttonPosition.y = _settings.EdgeSpacing.y + ((buttonSize.y + buttonSpacing.y) * Mathf.Floor(i / buttonsPerLine)) + extraAboveSpace;

            Rect buttonRect = new(buttonPosition, buttonSize);

            GUIStyle style = new("button");
            style.alignment = TextAnchor.LowerCenter;

            bool cantUse = _lastThreeValues.Contains(i);

            if (cantUse)
            {
                style.normal.background = ColoredTexture(buttonSize, new(0.75f,0.15f,0.15f));
            }

            if (GUI.Button(buttonRect, i + "", style) && !cantUse)
            {
                AppendTextArea(i + "");
            }

            Rect interactivePreviewRect = buttonRect;
            interactivePreviewRect.height = interactivePreviewRect.width;
            interactivePreviewRect.size *= 0.9f;
            interactivePreviewRect.position += Vector2.one * 0.05f * buttonSize.x;
            GUIStyle bgColor = new();

            try
            {
                _levelChunkOptionEditors[i].OnInteractivePreviewGUI(interactivePreviewRect, bgColor);
            } catch
            {
                RefreshLevelChunkOptions();
                return;
            }
        }

        static Texture2D ColoredTexture(Vector2 size, Color color)
        {
            Color32[] pixels = new Color32[Mathf.CeilToInt(size.x) * Mathf.CeilToInt(size.y)];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }
    }

    private void RefreshLevelChunkOptions()
    {
        if (!FindObjectOfType<IterativeChunkLoad>())
        {
            return;
        }

        _levelChunkOptions = FindObjectOfType<IterativeChunkLoad>().EveryChunk;

        if (_levelChunkOptionEditors != null)
        {
            for (int i = 0; i < _levelChunkOptionEditors.Length; i++)
            {
                if (_levelChunkOptionEditors[i] != null)
                {
                    _levelChunkOptionEditors[i].DiscardChanges();
                    DestroyImmediate(_levelChunkOptionEditors[i]);
                }
            }
        }

        _levelChunkOptionEditors = new Editor[_levelChunkOptions.Length];
        for (int i = 0; i < _levelChunkOptions.Length; i++)
        {
            _levelChunkOptionEditors[i] = Editor.CreateEditor(_levelChunkOptions[i]);
        }
    }

    private void RefreshLastValues()
    {
        _lastThreeValues = new int[]
        {
            -1, -1, -1
        };

        string[] lastValueStrings = _textAreaText.Split(",");

        if (lastValueStrings.Length == 0)
        {
            return;
        }

        int index = lastValueStrings.Length - 1;
        for (int i = 0; i < 3; i++)
        {
            while (!int.TryParse(lastValueStrings[index], out _))
            {
                if (index <= 0)
                {
                    return;
                }
                index--;
            }
            _lastThreeValues[i] = int.Parse(lastValueStrings[index]);
            if (index <= 0)
            {
                return;
            }
            index--;
        }
    }

    private void AppendTextArea(string text)
    {
        if (_textAreaText == "" || _textAreaText[^1] == ',')
        {
            _textAreaText += text;
        }
        else
        {
            _textAreaText += "," + text;
        }

        RefreshLastValues();
    }

    private bool ValidTextArea()
    {
        if (_textAreaText == ""
            || _textAreaText.Split(",").Length == 0
            || _textAreaText.Split(",")[^1] == ""
            || _textAreaText.Contains(",,"))
        {
            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(_textAreaText.Replace(",", ""), "^[0-9]*$"))
        {
            return false;
        }

        foreach (string curString in _textAreaText.Split(",").Distinct())
        {
            if (int.Parse(curString) >= _levelChunkOptions.Length)
            {
                return false;
            }
        }

        return true;
    }
}

public struct EditorWindowSettings
{
    public Vector2 EdgeSpacing;
}