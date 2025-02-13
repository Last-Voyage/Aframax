/******************************************************************************
// File Name:       VfxManager.cs
// Author:          Ryan Swanson
// Creation Date:   October 22nd, 2024
//
// Description:     Handles all vfx spawning and object pooling
******************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// How the vfx duration is set
/// ParticleSystemDuration is the longest duration of the attached particle systems
/// FixedDuration is a duration set in editor
/// Infinite means it last forever
/// </summary>
public enum EVfxDurationType
{
    ParticleSystemDuration,
    FixedDuration,
    Infinite
};

/// <summary>
/// Allows for external scripts to spawn vfx
/// </summary>
public class VfxManager : MainUniversalManagerFramework
{
    public static VfxManager Instance;

    [SerializeField] private SpecificVisualEffect[] _allVfxInGame;

    //Personally don't like to have to use ids. Let me know if you have a better solution
    private const int _MUZZLE_SMOKE_ID = 0;
    private const int _ENEMY_BLOOD_ID = 1;
    private const int _ENEMY_ATTACK_WARNING_ID = 2;
    private const int _WOODEN_SPARKS_ID = 3;
    private const int _METAL_SPARKS_ID = 4;
    private const int _PLUME_SMOKE_ID = 5;

    /// <summary>
    /// Triggers the Light Shift Horror Moment
    /// </summary>
    private static readonly UnityEvent _onLightShift = new();

    /// <summary>
    /// Triggers light flickering for the Slytherin Horror Moment
    /// </summary>
    private static readonly UnityEvent _onLightFlicker = new();

    /// <summary>
    /// Sets up the object pool of all vfx
    /// </summary>
    private void SetUpAllVisualEffectsInGame()
    {
        foreach (SpecificVisualEffect vfx in _allVfxInGame)
        {
            vfx.SetupSpecificVisualEffect();
        }
    }

    /// <summary>
    /// Spawns the vfx to be added to the pool
    /// SpecificVisualEffects cannot do this as it isn't a monobehavior
    /// </summary>
    /// <param name="specificVisualEffect"> The visual effect that we are creating the pool for </param>
    public void CreateVisualEffectsInPool(SpecificVisualEffect specificVisualEffect)
    {
        //Spawn the vfx
        GameObject newVfx = Instantiate(specificVisualEffect.GetVFXObject());
        newVfx.SetActive(false);
        ObjectPoolingParent.Instance.InitiallyAddObjectToPool(newVfx);

        //Gets the GeneralVfxFunctionality which acts as a mini manager of that set of vfx
        GeneralVfxFunctionality generalVfxFunctionality = newVfx.GetComponent<GeneralVfxFunctionality>();
        
        //Performs needed setup on the vfx
        generalVfxFunctionality.SetupChildParticleSystems();
        //Adds the created object to the pool
        specificVisualEffect.AddNewObjectToPool(generalVfxFunctionality);
    }

    /// <summary>
    /// Starts the process of moving the spawned vfx back to the pool
    /// Cannot be done by SpecificVisualEffect as its not a monobehavior
    /// </summary>
    /// <param name="specificVisualEffect">The specific visual effect the object belongs to</param>
    /// <param name="vfxObj">The object to be added back to the pool</param>
    public void StartVisualEffectsDuration(SpecificVisualEffect specificVisualEffect, GeneralVfxFunctionality vfxObj)
    {
        StartCoroutine(specificVisualEffect.MoveObjectBackToPool(vfxObj.gameObject));
    }

    /// <summary>
    /// Reclaims all visual effects before a scene change happens so that they don't get destroyed on the scene change
    /// </summary>
    private void ReclaimAllVisualEffectsBeforeSceneChange()
    {
        foreach (SpecificVisualEffect vfx in _allVfxInGame)
        {
            vfx.MoveAllVfxBackToPool();
        }
    }

    #region Base Manager
    /// <summary>
    /// Establishes the instance of VfxManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Performs the needed setup for the manager including spawning and setting up VFX
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        SetUpAllVisualEffectsInGame();
    }

    /// <summary>
    /// Subscribes the reclaiming vfx to the scene change
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        AframaxSceneManager.Instance.GetOnGameplaySceneLoaded.AddListener(ReclaimAllVisualEffectsBeforeSceneChange);
    }

    /// <summary>
    /// Unsubscribes the reclaiming vfx to the scene change
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        AframaxSceneManager.Instance.GetOnGameplaySceneLoaded.RemoveListener(ReclaimAllVisualEffectsBeforeSceneChange);
    }
    #endregion

    #region Events

    /// <summary>
    /// Invokes the _onLightShift event
    /// </summary>
    public void InvokeOnLightShift()
    {
        _onLightShift?.Invoke();
    }

    public void InvokeOnLightFlicker()
    {
        _onLightFlicker?.Invoke();
    }

    #endregion

    #region Getters

    #region GetVfx
    public SpecificVisualEffect GetMuzzleSmokeVfx() => _allVfxInGame[_MUZZLE_SMOKE_ID];
    public SpecificVisualEffect GetEnemyBloodVfx() => _allVfxInGame[_ENEMY_BLOOD_ID];
    public SpecificVisualEffect GetEnemyAttackWarningVfx() => _allVfxInGame[_ENEMY_ATTACK_WARNING_ID];
    public SpecificVisualEffect GetMetalSparksVfx() => _allVfxInGame[_METAL_SPARKS_ID];
    public SpecificVisualEffect GetWoodenSparksVfx() => _allVfxInGame[_WOODEN_SPARKS_ID];
    public SpecificVisualEffect GetPlumeSmokeVfx() => _allVfxInGame[_PLUME_SMOKE_ID];
    #endregion

    /// <summary>
    /// Returns the light shift event
    /// </summary>
    public UnityEvent GetOnLightShiftEvent() => _onLightShift;

    /// <summary>
    /// Returns the light flicker event
    /// </summary>
    public UnityEvent GetOnLightFlickerEvent() => _onLightFlicker;

    #endregion
}