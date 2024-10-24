/*********************************************************************************************************************
// File Name :         FmodAmbienceEvents
// Author :            Andrea Swihart-DeCoster
// Creation Date :     10/23/24
//
// Brief Description : Stores all ambient sounds.
*********************************************************************************************************************/

using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Events;

public class FmodAmbienceEvents : MonoBehaviour
{
    public static FmodAmbienceEvents Instance;

    [field: Header("Ambient Background Audio")]
    [field: SerializeField] public EventReference[] AmbientBackgroundSounds { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
