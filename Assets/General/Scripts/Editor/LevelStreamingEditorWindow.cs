using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Collections;
using System;

public class LevelStreamingEditorWindow : EditorWindow
{
    [MenuItem("Window/Level Builder")]
    public static void ShowWindow()
    {
        GetWindow<LevelStreamingEditorWindow>("Level Streaming Editor");
    }

    private GameObject[] _levelChunkOptions;
    private Editor[] _levelChunkOptionEditors;
    private int _chunkSpacing;
    private List<GameObject> _levelChunkOrder;
    private List<GameObject> _previousLevelChunkOrder;
    private List<Editor> _levelChunkOrderEditors;

    private void OnGUI()
    {
        DrawLevelChunkOrderList();

        if (GUI.Button(new(0,500, 400, 75), "Add New Chunk"))
        {
            _levelChunkOrder.Add(null);
            _levelChunkOrderEditors.Add(null);
            _previousLevelChunkOrder.Add(null);
        }
        //if (GUILayout.Button("Reset Chunk List"))
        //{
        //    _levelChunkOrder = new();
        //    _levelChunkOrderEditors = new();
        //}
    }

    private void OnEnable()
    {
        _levelChunkOptions = RefreshLevelChunkOptions();
        _levelChunkOrder = new();
        _previousLevelChunkOrder = new();
        _levelChunkOrderEditors = new();
    }

    private void DrawLevelChunkOrderList()
    {
        WindowSettings settings = new();
        settings.ListItemSize = new(1000, 100);
        settings.ListItemSpacing = new(5, 5);
        settings.ButtonSize = Vector2Int.one * 25;

        for (int i = 0; i < _levelChunkOrder.Count; i++)
        {
            (_previousLevelChunkOrder[i], _levelChunkOrder[i], _levelChunkOrderEditors[i]) = DrawLevelChunkOrderListItem(_previousLevelChunkOrder[i], _levelChunkOrder[i], _levelChunkOrderEditors[i], settings, i);
        }
    }

    private (GameObject, GameObject, Editor) DrawLevelChunkOrderListItem(GameObject previousGameObject, GameObject gameObject, Editor gameObjectEditor, WindowSettings settings, int index)
    {
        Texture2D listItemTexture = new(settings.ListItemSize.x, settings.ListItemSize.y);
        Color32 fillColor = new(127, 127, 127, 255);
        Color32[] fillColorArray = listItemTexture.GetPixels32();
        for (var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = fillColor;
        }
        listItemTexture.SetPixels32(fillColorArray);
        listItemTexture.Apply();
        GUI.DrawTexture(new(settings.GetPositionByIndex(index, 1), settings.ListItemSize), listItemTexture);

        Vector2 basePosition = settings.GetPositionByIndex(index, 2);

        Vector2 upButtonPosition = basePosition + Vector2.up * (settings.ListItemSize.y / 2f - settings.ButtonSize.y);
        Rect upButtonRect = new(upButtonPosition, settings.ButtonSize);
        GUI.Button(upButtonRect, "↑");

        Vector2 downButtonPosition = basePosition + Vector2.up * (settings.ListItemSize.y / 2f);
        Rect downButtonRect = new(downButtonPosition, settings.ButtonSize);
        GUI.Button(downButtonRect, "↓");

        Vector2 plusButtonOffset = new(settings.ButtonSize.x + settings.ListItemSpacing.x, (settings.ListItemSize.y / 2f) - settings.ButtonSize.y);
        Vector2 plusButtonPosition = basePosition + plusButtonOffset;
        Rect plusButtonRect = new(plusButtonPosition, 2 * settings.ButtonSize);
        GUI.Button(plusButtonRect, "+");

        DrawWindow(ref previousGameObject, ref gameObject, ref gameObjectEditor, settings, index);

        return (previousGameObject, gameObject, gameObjectEditor);
    }

    private void DrawWindow(ref GameObject previousGameObject, ref GameObject gameObject, ref Editor gameObjectEditor, WindowSettings settings, int index)
    {
        Vector2 basePosition = settings.GetPositionByIndex(index, 2);
        GUIStyle bgColor = new GUIStyle();

        Vector2 objectFieldOffset = new(settings.ButtonSize.x * 3 + settings.ListItemSpacing.x * 2, settings.ListItemSize.y / 20f);
        Vector2 objectFieldPosition = basePosition + objectFieldOffset;
        Rect objectFieldRect = new(objectFieldPosition, new(settings.ListItemSize.y * 0.9f + 19, settings.ListItemSize.y * 0.9f));
        gameObject = (GameObject)EditorGUI.ObjectField(objectFieldRect, gameObject, typeof(GameObject), true);

        if (gameObject != null)
        {
            if (gameObjectEditor == null || previousGameObject != gameObject)
            {
                gameObjectEditor = Editor.CreateEditor(gameObject);
                previousGameObject = gameObject;
            }

            //Vector2 interactivePreviewOffset = (settings.ButtonSize.x * 5 + settings.ListItemSpacing.x * 3) * Vector2.right;
            //Vector2 interactivePreviewPosition = basePosition + interactivePreviewOffset;
            Rect interactivePreviewRect = new(objectFieldPosition, settings.ListItemSize.y * 0.9f * Vector2.one);

            gameObjectEditor.OnInteractivePreviewGUI(interactivePreviewRect, bgColor);
        }
    }

    private GameObject[] RefreshLevelChunkOptions()
    {
        return new GameObject[] { };
    }
}

public struct WindowSettings
{
    public Vector2Int Offset;
    public Vector2Int ListItemSize;
    public Vector2Int ListItemSpacing;
    public Vector2Int ButtonSize;

    public Vector2 GetPositionByIndex(int index, int horizontalItemCount)
    {
        return (index * (ListItemSize.y + ListItemSpacing.y) * Vector2.up) + Offset + (horizontalItemCount * ListItemSpacing.x * Vector2.right);
    }
}