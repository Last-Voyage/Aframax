/*********************************************************************************************************************
// File Name :         FmodSfxEvents
// Author :            Andrea Swihart-DeCoster
// Contributers :      Charlie Polonus
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
    [field: SerializeField] public EventReference LimbMove { get; private set; }

    [field: Tooltip("Time to fade in the limb movement")]
    [field: SerializeField] public float LimbMoveFadeInTime { get; private set; }

    [field: Tooltip("Time to fade out the limb movement")]
    [field: SerializeField] public float LimbMoveFadeOutTime { get; private set; }

    [field: SerializeField] public EventReference ChaseSequenceStart { get; private set; }
    [field: SerializeField] public EventReference ChaseSequenceLoop { get; private set; }

    [field: Tooltip("Time to fade in the limb movement")]
    [field: SerializeField] public float ChaseLoopFadeInTime { get; private set; }

    [field: Tooltip("Time to fade out the limb movement")]
    [field: SerializeField] public float ChaseLoopFadeOutTime { get; private set; }

    #endregion Boss

    #region Interactables

    [field: Header("Interactable")]
    
    [field: SerializeField] public EventReference RadioSong { get; private set; }
    [field: SerializeField] private EventReference[] _pickupSoundEffects;

    #endregion
    
    #region Player

    [field: Header("Player")]
    [field: SerializeField] public EventReference AboveDeckWalking { get; private set; }
    [field: SerializeField] public EventReference BelowDeckWalking { get; private set; }
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
    [field: Header("Harpoon Collisions")]
    [field: SerializeField] public EventReference HarpoonHitGeneral { get; private set; }
    [field: SerializeField] public EventReference HarpoonHitWood { get; private set; }
    [field: SerializeField] public EventReference HarpoonHitMetal { get; private set; }
    [field: SerializeField] public EventReference HarpoonWaterSplash { get; private set; }
    [field: Header("Harpoon Misc.")]
    [field: SerializeField] public EventReference HarpoonShiftingMovement { get; private set; }
    [field: Tooltip("Time to fade in the harpoon movement")]
    [field: SerializeField] public float HarpoonShiftFadeInTime { get; private set; }

    [field: Tooltip("Time to fade out the harpoon movement")]
    [field: SerializeField] public float HarpoonShiftFadeOutTime { get; private set; }

    #endregion Harpoon

    #endregion Player

    #region Environment
    [field: Header("Environment")]
    [field: SerializeField] public EventReference GeneratorFixed { get; private set; }
    #endregion

    /// <summary>
    /// Gets the item pickup sound based on the id of the sound effect
    /// </summary>
    /// <param name="pickupSoundId"> The id of the sound effect </param>
    /// <returns> The sound effect recieved from the id </returns>
    public EventReference GetItemPickupSound(int pickupSoundId)
    {
        // Edge case: the id is too high or the list doesn't exist
        if (_pickupSoundEffects == null || pickupSoundId >= _pickupSoundEffects.Length)
        {
            return new();
        }

        // Edge case: the sound effect doesn't exist
        if (_pickupSoundEffects[pickupSoundId].IsNull)
        {
            return new();
        }

        // Return the sound effect
        return _pickupSoundEffects[pickupSoundId];
    }

    public void SetUpInstance()
    {
        Instance = this;
    }
}
