/*********************************************************************************************************************
// File Name :         FmodAmbienceEvents
// Author :            Andrea Swihart-DeCoster
// Contributors :      Ryan Swanson
// Creation Date :     10/23/24
//
// Brief Description : Stores all ambient sounds.
*********************************************************************************************************************/

using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Stores all FMOD ambient / background audio events
/// </summary>
public class FmodAmbienceEvents : MonoBehaviour
{
    public static FmodAmbienceEvents Instance;

    [field: Header("Ambient Background Audio")]
    [field: Tooltip("Any audio added here will play throughout the full game")]
    [field: SerializeField] public EventReference[] AmbientGameBackgroundSounds { get; private set; }

    /// <summary>
    /// Creates the instance of the FmodAmbienceEvents
    /// </summary>
    public void SetUpInstance()
    {
        Instance = this;
    }
}
