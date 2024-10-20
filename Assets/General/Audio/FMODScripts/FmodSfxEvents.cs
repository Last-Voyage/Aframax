/*********************************************************************************************************************
// File Name :         FmodSfxEvents
// Author :            Andrea Swihart-DeCoster
// Creation Date :     10/18/24
//
// Brief Description : Stores all the SFX
*********************************************************************************************************************/

using FMODUnity;
using UnityEngine;

/// <summary>
/// Stores all the FMOD SFX as properties.
/// </summary>
public class FmodSfxEvents : MonoBehaviour
{
    public static FmodSfxEvents Instance;

    [field: Header("Boss")]
    [field: SerializeField] public EventReference weakPointDestroyed { get; private set; }

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
