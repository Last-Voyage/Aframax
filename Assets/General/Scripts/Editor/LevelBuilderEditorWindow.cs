/*********************************************************************************************************************
// File Name :         LevelBuilderEditorWindow.cs
// Author :            Charlie Polonus
// Creation Date :     10/24/2024
// Brief Description : The script behind the editor window that allows chunk-based level building 
*********************************************************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class LevelBuilderEditorWindow : EditorWindow
{
    // Create the window if there isn't one already
    [MenuItem("Window/Level Builder")]
    public static void ShowWindow()
    {
        GetWindow<LevelBuilderEditorWindow>("Level Builder");
    }

    // The arrays containing the chunk objects and their respective preview windows
    private GameObject[] _levelChunkOptions;
    private Editor[] _levelChunkOptionEditors;

    // Various settings to ensure that the window functions correctly
    private string _previousTextAreaText;
    private string _textAreaText;
    private int[] _lastThreeValues;

    // Static UI element sizes
    private static Vector2 _buttonSpacing = new(5, 5);
    private static Vector2 _edgeSpacing = new(10, 10);
    private static float _textAreaHeight = 200f;

    // Static grid UI element sizes
    private static Vector2 _gridButtonSize = new(75, 90);
    private static Vector2 _gridButtonSpacing = new(5, 5);

    /// <summary>
    /// A getter for the current window size
    /// </summary>
    /// <returns>The current editor window's size</returns>
    private Vector2 GetWindowSize() => position.size;

    /// <summary>
    /// Once the window is made, set all the variables to default values
    /// </summary>
    private void OnEnable()
    {
        _textAreaText = "";

        // Make sure that it doesn't restrict any chunks from being selected
        _lastThreeValues = new int[3]
        {
            -1, -1, -1
        };

        RefreshLevelChunkOptions();
    }

    /// <summary>
    /// Once the window is destroyed, essentially "clear the cache" of the preview scenes
    /// </summary>
    private void OnDestroy()
    {
        // Edge case: There's nothing to destroy
        if (_levelChunkOptionEditors == null)
        {
            return;
        }

        // Delete the items in the list
        for (int i = 0; i < _levelChunkOptionEditors.Length; i++)
        {
            _levelChunkOptionEditors[i].DiscardChanges();
        }
    }

    /// <summary>
    /// When rendering the window
    /// </summary>
    private void OnGUI()
    {
        // If there's no iterative chunk loader, don't even try to render, just show a warning
        if (!FindObjectOfType<IterativeChunkLoad>())
        {
            GUILayout.Label("\nUnable to be used here, try adding an object with the IterativeChunkLoad script");
            return;
        }

        // If the current text is different, update the last three number spaces, and update the text
        if (_textAreaText != _previousTextAreaText)
        {
            RefreshLastValues();
            _previousTextAreaText = _textAreaText;
        }

        // Display the text area and buttons, displacing the buttons by however much space the text area takes up
        float displacement = DisplayTextFileArea();
        DisplayChunkOptionButtons(displacement);
    }

    /// <summary>
    /// Displays the text area and necessary buttons
    /// </summary>
    /// <returns>The amount of vertical space needed to display something under it</returns>
    private float DisplayTextFileArea()
    {
        // Define the text area's size
        Vector2 textAreaSize = new();
        textAreaSize.x = (GetWindowSize().x - (_edgeSpacing.x * 2) - _buttonSpacing.x) * 0.75f;
        textAreaSize.y = _textAreaHeight;

        Vector2 textAreaPosition = _edgeSpacing;

        // Convert everything into a rect for the text area, and display it
        Rect textAreaRect = new(textAreaPosition, textAreaSize);
        _textAreaText = GUI.TextArea(textAreaRect, _textAreaText);

        // ----- Button 1: Load Level -----
        // Set default values for the side buttons' size
        Vector2 sideButtonSize;
        sideButtonSize.x = (GetWindowSize().x - (_edgeSpacing.x * 2) - _buttonSpacing.x) * 0.25f;
        sideButtonSize.y = (textAreaSize.y - (2 * _buttonSpacing.y)) / 3f;

        // Set default values for the side buttons' position
        Vector2 buttonOffset = new();
        buttonOffset.x = textAreaSize.x + _buttonSpacing.x;
        buttonOffset.y = 0;

        // Display the button
        Rect buttonRect = new(_edgeSpacing + buttonOffset, sideButtonSize);
        if (GUI.Button(buttonRect, "Load Level from\nSave File"))
        {
            _textAreaText = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/ChunkQueue.txt");
        }

        // ----- Button 2: Save Level -----
        // Update the position
        buttonOffset.y = sideButtonSize.y + _buttonSpacing.y;
        buttonRect = new(_edgeSpacing + buttonOffset, sideButtonSize);

        // Change the text on the button if unable to save safely
        bool isValidText = ValidTextArea();
        GUIStyle buttonStyle = new("button");
        buttonStyle.richText = true;
        string buttonName = "Save Current Level\nOrder to File" + (!isValidText ? "\n<color=red>Warning! Will not work!</color>" : "");

        // Display the button
        if (GUI.Button(buttonRect, buttonName, buttonStyle))
        {
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/ChunkQueue.txt", _textAreaText);
        }

        // ----- Button 3: Refresh Chunk List -----
        // Update the rect
        buttonOffset.y = (sideButtonSize.y + _buttonSpacing.y) * 2;
        sideButtonSize.y = ((textAreaSize.y - (3 * _buttonSpacing.y)) / 3f) * 0.7f;
        buttonRect = new(_edgeSpacing + buttonOffset, sideButtonSize);

        // Display the button
        if (GUI.Button(buttonRect, "Refresh Chunk List\n(Below)", buttonStyle))
        {
            RefreshLevelChunkOptions();
        }

        // ----- Float Field: Chunk Size Field -----
        // Update the field's position
        buttonOffset.y += _buttonSpacing.y + sideButtonSize.y;
        buttonOffset.x += sideButtonSize.x * 0.75f;

        // Updating field size
        sideButtonSize.y *= 0.4f;
        sideButtonSize.x /= 4f;

        // Setting the rect
        buttonRect = new(_edgeSpacing + buttonOffset, sideButtonSize);

        // Display the field and update the iterative chunk loader with the new chunk size
        float chunkDistance = FindObjectOfType<IterativeChunkLoad>().DistanceBetweenChunks;
        chunkDistance = EditorGUI.FloatField(buttonRect, chunkDistance);
        FindObjectOfType<IterativeChunkLoad>().SetChunkDistance(chunkDistance);

        // ----- Field Label: Chunk Size Field -----
        // Update the label's rect
        buttonOffset.x -= sideButtonSize.x * 3;
        sideButtonSize.x *= 3f;
        buttonRect = new(_edgeSpacing + buttonOffset, sideButtonSize);

        // Display the label
        EditorGUI.LabelField(buttonRect, "Chunk Size");

        // Return the y value that was taken up by the text field and buttons
        return textAreaSize.y + (2 * _buttonSpacing.y);
    }

    /// <summary>
    /// Display the buttons for defining the level
    /// </summary>
    /// <param name="extraAboveSpace">How much vertical space to add above the buttons</param>
    private void DisplayChunkOptionButtons(float extraAboveSpace)
    {
        // Edge cases in case certain things aren't set yet
        if (_levelChunkOptions == null
            || _levelChunkOptions.Length == 0)
        {
            return;
        }

        // Define the max number of buttons can exist in one row
        int buttonCount = _levelChunkOptions.Length;
        int buttonsPerLine = buttonCount;

        // Iterate through each button to find the max value
        for (int i = 0; i < buttonCount + 1; i++)
        {
            float currentTotalWidth = (i * _gridButtonSize.x) + ((i - 1) * _buttonSpacing.x) + (_edgeSpacing.x * 2);

            // If the buttons can't fit, don't let them get set that long per row
            if (currentTotalWidth > GetWindowSize().x)
            {
                buttonsPerLine = i - 1;
                break;
            }
        }

        // Display the buttons in a grid
        for (int i = 0; i < buttonCount; i++)
        {
            // Set the button's positions based on their index
            Vector2 buttonPosition = new();
            buttonPosition.x = _edgeSpacing.x + ((_gridButtonSize.x + _buttonSpacing.x) * (i % buttonsPerLine));
            buttonPosition.y = _edgeSpacing.y + ((_gridButtonSize.y + _buttonSpacing.y) * Mathf.Floor(i / buttonsPerLine)) + extraAboveSpace;

            // Update the button's rect with the next position and size
            Rect buttonRect = new(buttonPosition, _gridButtonSize);

            // Create a style for the buttons and make the text at the bottom middle
            GUIStyle style = new("button");
            style.alignment = TextAnchor.LowerCenter;

            // Check if the current chunk can be used, if not set it red
            bool isUnusableChunk = _lastThreeValues.Contains(i);
            if (isUnusableChunk)
            {
                style.normal.background = ColoredTexture(_gridButtonSize, new(0.75f,0.15f,0.15f));
            }

            // Update the text if clicked, and if able to add the chunk to the list
            if (GUI.Button(buttonRect, i + "", style) && !isUnusableChunk)
            {
                AppendTextArea(i + "");
            }

            // Initialize the rect for the scene preview
            Rect interactivePreviewRect = buttonRect;
            interactivePreviewRect.height = interactivePreviewRect.width;
            interactivePreviewRect.size *= 0.9f;
            interactivePreviewRect.position += 0.05f * _gridButtonSize.x * Vector2.one;
            GUIStyle bgColor = new();

            // Try to place the scene preview, if it can't, refresh the list of chunks to see if it's possible to place it
            try
            {
                _levelChunkOptionEditors[i].OnInteractivePreviewGUI(interactivePreviewRect, bgColor);
            }
            catch
            {
                RefreshLevelChunkOptions();
                return;
            }
        }

        // A static method that returns a colored texture of a given size
        static Texture2D ColoredTexture(Vector2 size, Color color)
        {
            // Creates the texture
            Color32[] pixels = new Color32[Mathf.CeilToInt(size.x) * Mathf.CeilToInt(size.y)];

            // Sets all the pixels to the right color
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            // Creates a rexture and returns it
            Texture2D texture = new(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }
    }

    /// <summary>
    /// Update the list of chunks with any new chunks in the iterative chunk loader
    /// </summary>
    private void RefreshLevelChunkOptions()
    {
        // Edge case if there isn't an iterative chunk loader, or if the list of chunks doesn't exist
        if (!FindObjectOfType<IterativeChunkLoad>()
            || FindObjectOfType<IterativeChunkLoad>().EveryChunk == null)
        {
            return;
        }

        // Get all the chunks that are in the list of the iterative chunk loader
        _levelChunkOptions = FindObjectOfType<IterativeChunkLoad>().EveryChunk;

        // If the list of editors exists, delete and "uncache" the scene previews
        if (_levelChunkOptionEditors != null)
        {
            for (int i = 0; i < _levelChunkOptionEditors.Length; i++)
            {
                if (_levelChunkOptionEditors[i] != null)
                {
                    // Removes the scene preview from the list
                    _levelChunkOptionEditors[i].DiscardChanges();
                    DestroyImmediate(_levelChunkOptionEditors[i]);
                }
            }
        }

        // Creates a new list of editors for the new chunk objects
        _levelChunkOptionEditors = new Editor[_levelChunkOptions.Length];
        for (int i = 0; i < _levelChunkOptions.Length; i++)
        {
            _levelChunkOptionEditors[i] = Editor.CreateEditor(_levelChunkOptions[i]);
        }
    }

    /// <summary>
    /// Get the last three chunk used, in order to keep them from being used again
    /// </summary>
    private void RefreshLastValues()
    {
        // Default case: All chunks can be used
        _lastThreeValues = new int[]
        {
            -1, -1, -1
        };

        // Split the string into its individual numbers
        string[] lastValueStrings = _textAreaText.Split(",");

        // Edge case: there are no chunks at all chosen, return the default
        if (lastValueStrings.Length == 0)
        {
            return;
        }

        // Iterate through the last items in the list until a chunk is found
        int index = lastValueStrings.Length - 1;
        for (int i = 0; i < 3; i++)
        {
            // If the currently selected thing isn't a number, keep checking
            while (!int.TryParse(lastValueStrings[index], out _))
            {
                // If reached the end of the list, stop checking
                if (index <= 0)
                {
                    return;
                }
                index--;
            }

            // Add the number to the list of chunks that can't be used
            _lastThreeValues[i] = int.Parse(lastValueStrings[index]);

            // If reached the end of the list, stop checking
            if (index <= 0)
            {
                return;
            }
            index--;
        }
    }

    /// <summary>
    /// Add text to the end of the text area
    /// </summary>
    /// <param name="text">The text to be added</param>
    private void AppendTextArea(string text)
    {
        // If there's nothing in the list, or the area already ends in a comma, just add the number
        if (_textAreaText == "" || _textAreaText[^1] == ',')
        {
            _textAreaText += text;
        }
        // Otherwise, add the comma along with the number
        else
        {
            _textAreaText += "," + text;
        }
    }

    /// <summary>
    /// Tests if the text area can't be generated in some way with the chunks
    /// </summary>
    /// <returns>Whether or not the list can be converted into chunks</returns>
    private bool ValidTextArea()
    {
        // Edge cases: The list contains any amount of nonsensical strings, can't be read
        if (_textAreaText == ""
            || _textAreaText.Split(",").Length == 0
            || _textAreaText.Split(",")[^1] == ""
            || _textAreaText.Contains(",,"))
        {
            return false;
        }

        // Edge case: The list contains non-numbers, can't be read
        if (!System.Text.RegularExpressions.Regex.IsMatch(_textAreaText.Replace(",", ""), "^[0-9]*$"))
        {
            return false;
        }

        // Edge case: The numbers in the list are more than the available chunks, can't be read
        foreach (string curString in _textAreaText.Split(",").Distinct())
        {
            if (int.Parse(curString) >= _levelChunkOptions.Length)
            {
                return false;
            }
        }

        // Everything should be safe to read
        return true;
    }
}