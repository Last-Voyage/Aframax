/**********************************************************************************************************************
// File Name :          ColorBlindnessEditorUtility.cs
// Author :             Miles Rogers
// Creation Date :      4/15/2025
//
// Brief description :  Custom editor window for managing color blindness simulation in-game
**********************************************************************************************************************/

#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Custom editor window for managing color blindness simulation in-game
/// </summary>
public class ColorBlindnessEditorUtility : EditorWindow
{
    /// <summary>
    /// Creates the ColorBlindnessEditorUtility
    /// </summary>
    [MenuItem("Tools/Color Blindness")]
    public static void OpenColorblindnessSettings()
    {
        var wnd = GetWindow<ColorBlindnessEditorUtility>();
        wnd.titleContent = new GUIContent("Color Blindness Settings");
    }

    /// <summary>
    /// Creates the Unity editor layout for the ColorBlindnessEditorUtility
    /// </summary>
    public void CreateGUI()
    {
        // Set margins
        rootVisualElement.style.marginTop = 10;
        rootVisualElement.style.marginBottom = 10;
        rootVisualElement.style.marginLeft = 10;
        rootVisualElement.style.marginRight = 10;
        
        // Create label
        var label = new Label("Select color blindness mode:");
        rootVisualElement.Add(label);

        // Get enum names to use as options
        var options = System.Enum.GetNames(typeof(GlobalColorFilterManager.ColorBlindnessMode));

        // Default dropdown field
        var dropdown = new DropdownField("Mode", options.ToList(), 0);
        dropdown.style.marginTop = 10;
        
        // Value change lambda
        dropdown.RegisterValueChangedCallback(evt =>
        {
            if (System.Enum.TryParse(evt.newValue, out GlobalColorFilterManager.ColorBlindnessMode selectedMode))
            {
                // Check if the game is running
                if (!Application.isPlaying)
                {
                    // Display an error
                    EditorUtility.DisplayDialog(
                        "Colorblindness Filter Error",
                        "You may only change the colorblindness filter setting in play mode!",
                        "Ok"
                    );
                    return;
                }
                
                // Set the filter in the global color filter manager
                GlobalColorFilterManager.SetColorblindnessFilter(selectedMode);
            }
        });
        
        // Add dropdown to window
        rootVisualElement.Add(dropdown);
    }
}

#endif // UNITY_EDITOR
