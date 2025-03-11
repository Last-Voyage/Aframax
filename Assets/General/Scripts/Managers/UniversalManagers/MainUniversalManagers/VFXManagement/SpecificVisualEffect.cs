/******************************************************************************
// File Name:       SpecificVisualEffect.cs
// Author:          Ryan Swanson
// Creation Date:   November 19th, 2024
//
// Description:     Acts as the owner of a specific visual effect pool
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as the owner of a specific vfx
/// Contains the object pool of that vfx
/// </summary>
[System.Serializable]
public class SpecificVisualEffect
{
    [Tooltip("Name of the visual effect")]
    [SerializeField] private string _vfxName;

    [Tooltip("The vfx that is created")]
    [SerializeField] private GameObject _vfxObject;
    [Tooltip("How large the object pool is for the object")]
    [SerializeField] private int _poolingAmount;

    [Space]
    [Tooltip("The type of duration that the vfx has")]
    [SerializeField] private EVfxDurationType _vfxDurationType;
    [Tooltip("The fixed duration of the vfx if set to that duration type")]
    [SerializeField] private float _fixedDuration;
    private float _particleDuration;

    private int _poolingCounter = 0;

    private GeneralVfxFunctionality[] _vfxPool;

    /// <summary>
    /// Performs all needed setup for the specific vfx
    /// </summary>
    public void SetupSpecificVisualEffect()
    {
        InitializeObjectPool();
        CalculateParticleDuration();
    }

    /// <summary>
    /// Sets up the object pool by spawning all needed objects
    /// </summary>
    private void InitializeObjectPool()
    {
        //Initializes the array
        _vfxPool = new GeneralVfxFunctionality[_poolingAmount];
        for (int i = 0; i < _poolingAmount; i++)
        {
            //Spawns the vfx, disables it, and adds it to the pool
            VfxManager.Instance.CreateVisualEffectsInPool(this);
            _poolingCounter++;
        }
        _poolingCounter = 0;
    }

    /// <summary>
    /// Determines how long the duration of the particle system should last before being reclaimed by the pool
    /// </summary>
    private void CalculateParticleDuration()
    {
        switch (_vfxDurationType)
        {
            case (EVfxDurationType.ParticleSystemDuration):
                if (_vfxPool.Length != 0)
                {
                    _particleDuration = _vfxPool[0].GetLongestParticleSystemDuration();
                }
                return;
            case (EVfxDurationType.FixedDuration):
                _particleDuration = _fixedDuration;
                return;
            case (EVfxDurationType.Infinite):
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// Adds the created vfx object to the pool
    /// </summary>
    /// <param name="generalVfxFunctionality">Which object is being added</param>
    public void AddNewObjectToPool(GeneralVfxFunctionality generalVfxFunctionality)
    {
        _vfxPool[_poolingCounter] = generalVfxFunctionality;
    }

    /// <summary>
    /// Plays the next vfx in the object pool
    /// </summary>
    /// <param name="location">Where to place the object</param>
    /// <returns></returns>
    public GeneralVfxFunctionality PlayNextVfxInPool(Vector3 location, Quaternion rotation)
    {
        int previousCounterValue = _poolingCounter;

        _vfxPool[_poolingCounter].gameObject.SetActive(true);
        _vfxPool[_poolingCounter].transform.position = location;
        _vfxPool[_poolingCounter].transform.rotation = rotation;
        _vfxPool[_poolingCounter].StartAllVfx();

        //Starts the duration if the vfx has one
        if (_vfxDurationType != EVfxDurationType.Infinite)
        {
            VfxManager.Instance.StartVisualEffectsDuration(this, _vfxPool[_poolingCounter]);
        }

        IterateVfxPool();

        return _vfxPool[previousCounterValue];
    }

    /// <summary>
    /// Plays the next vfx in the object pool
    /// Takes in a Transform and sets it to be its parent
    /// </summary>
    /// <param name="parent"> The parent that we are setting the vfx to </param>
    /// <param name="location"> The location that we are creating the vfx at </param>
    /// <param name="rotation"> The rotation that we are creating the vfx at </param>
    /// <returns> The vfx that we played </returns>
    public GeneralVfxFunctionality PlayNextVfxInPool(Transform parent, Vector3 location, Quaternion rotation)
    {
        GeneralVfxFunctionality currentVfx = PlayNextVfxInPool(location, rotation);
        currentVfx.gameObject.transform.SetParent(parent);
        currentVfx.transform.position = location;

        return currentVfx;
    }

    /// <summary>
    /// Moves a specific object back to the object pool
    /// </summary>
    /// <param name="vfxObject"> The object to move back to the pool </param>
    /// <returns> The delay between iteration </returns>
    public IEnumerator MoveObjectBackToPool(GameObject vfxObject)
    {
        yield return new WaitForSeconds(_particleDuration);
        HideVfx(vfxObject);
        ObjectPoolingParent.Instance.AddObjectAsChild(vfxObject);
    }

    /// <summary>
    /// Moves all vfx back to being in the object pool
    /// </summary>
    public void MoveAllVfxBackToPool()
    {
        foreach (GeneralVfxFunctionality vfx in _vfxPool)
        {
            ObjectPoolingParent.Instance.AddObjectAsChild(vfx.gameObject);
        }
    }

    /// <summary>
    /// Iterates through the object pool counter
    /// </summary>
    private void IterateVfxPool()
    {
        _poolingCounter++;
        if (_poolingCounter >= _poolingAmount)
        {
            _poolingCounter = 0;
        }
    }

    /// <summary>
    /// Disables the vfx object after adding it back to the pool
    /// </summary>
    /// <param name="objectToHide"> The vfx gameobject to set disabled </param>
    private void HideVfx(GameObject objectToHide)
    {
        objectToHide.SetActive(false);
    }

    #region Getters
    public string GetVfxName() => _vfxName;
    public GameObject GetVfxObject() => _vfxObject;
    #endregion
}
