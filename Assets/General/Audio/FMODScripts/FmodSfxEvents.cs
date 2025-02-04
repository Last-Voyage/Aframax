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

    #region ui

    [field: Header("UI")]
    [field: SerializeField] public EventReference MasterVolumeSettingsChanged { get; private set; }
    [field: SerializeField] public EventReference SfxVolumeSettingsChanged { get; private set; }
    [field: SerializeField] public EventReference AmbienceVolumeSettingsChanged { get; private set; }

    #endregion

    #region Boss

    [field: Header("Boss")]
    [field: SerializeField] public EventReference WeakPointDestroyed { get; private set; }
    [field: SerializeField] public EventReference LimbDestroyed { get; private set; }
    [field: SerializeField] public EventReference LimbAttack { get; private set; }
    [field: SerializeField] public EventReference SceneStart { get; private set; }
    [field: SerializeField] public EventReference LimbSpawn { get; private set; }

    #endregion Boss

    #region Player

    [field: Header("Player")]
    [field: SerializeField] public EventReference HardSurfaceWalking { get; private set; }
    [field: Tooltip("Time between each footstep")]
    [field: SerializeField] public float FootstepDelay { get; private set; } = 0.3f;
    [field: Tooltip("Time between each footstep")]
    [field: SerializeField] public float FirstFootstepDelay { get; private set; } = 0.1f;

    [field: Space]
    [field: SerializeField] public EventReference PlayerTookDamage { get; private set; }
    [field: SerializeField] public EventReference PlayerHeartBeat { get; private set; }

    #region Harpoon

    [field: Header("Harpoon")]
    [field: SerializeField] public EventReference HarpoonShot { get; private set; }
    [field: SerializeField] public EventReference HarpoonReload { get; private set; }
    [field: SerializeField] public EventReference HarpoonHitBoat { get; private set; }
    [field: SerializeField] public EventReference HarpoonWaterSplash { get; private set; }
    

    #endregion Harpoon

    #endregion Player

    public void SetUpInstance()
    {
        Instance = this;
    }
}
