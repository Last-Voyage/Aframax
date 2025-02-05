/**********************************************************************************************************************
// File Name :         GeneratorInteractable.cs
// Author :            Ryan Swanson
// Contributors:       Nick Rice, Charlie Polonus
// Creation Date :     11/14/24
// 
// Brief Description : Controls the functionality for the generator
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the functionality for the generator
/// </summary>
public class GeneratorInteractable : InventoryInteractableTrigger
{
    [Tooltip("Moves the sparks and smoke forwards, or elsewhere")]
    private readonly Vector3 _vfxDisplacement = Vector3.back;
    
    /// <summary>
    /// This function spawns in the smoke for the generator
    /// </summary>
    public void GeneratorStartsSmoking()
    {
        VfxManager.Instance.GetPlumeSmokeVfx().PlayNextVfxInPool
            (transform.position + _vfxDisplacement, transform.rotation);
    }

    /// <summary>
    /// This functions spawns in the sparks for the generator
    /// </summary>
    public void GeneratorSparks()
    {
        VfxManager.Instance.GetMetalSparksVfx().PlayNextVfxInPool
            (transform.position + _vfxDisplacement, transform.rotation);
    }

    /// <summary>
    /// This function plays the audio when repaired
    /// </summary>
    public void GeneratorRepair()
    {
        RuntimeSfxManager.APlayOneShotSfxAttached?.Invoke(FmodSfxEvents.Instance.GeneratorFixed, gameObject);
    }

    /// Note for future engineers: The method OnSoundChange() still exists, it just runs the base script
    /// InventoryInteractableTrigger, if you want to override it you still can
}
