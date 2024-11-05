/******************************************************************************
// File Name:       VfxManager.cs
// Author:          Ryan Swanson
// Creation Date:   October 22nd, 2024
//
// Description:     Handles all vfx spawning and object pooling
******************************************************************************/

using System.Collections;
using UnityEngine;

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
    private const int MUZZLE_SMOKE_ID = 0;
    private const int ENEMY_BLOOD_ID = 1;
    private const int ENEMY_ATTACK_WARNING_ID = 2;

    /// <summary>
    /// Sets up the object pool of all vfx
    /// </summary>
    private void SetUpAllVfxInGame()
    {
        foreach (SpecificVisualEffect vfx in _allVfxInGame)
        {
            vfx.SetupSpecificVisualEffect();
        }
    }

    /// <summary>
    /// Reclaims all vfx from all vfx in game back into the game
    /// </summary>
    private void ReclaimAllVFXBeforeSceneChange()
    {
        foreach (SpecificVisualEffect vfx in _allVfxInGame)
        {
            vfx.MoveAllVfxBackToPool();
        }
    }

    /// <summary>
    /// Spawns the vfx to be added to the pool
    /// SpecificVisualEffects cannot do this as it isn't a monobehavior
    /// </summary>
    /// <param name="specificVisualEffect"></param>
    public void CreateVFXInPool(SpecificVisualEffect specificVisualEffect)
    {
        //Spawn the vfx
        GameObject newVfx = Instantiate(specificVisualEffect.GetVFXObject());
        newVfx.SetActive(false);
        ObjectPoolingParent.Instance.AddObjectAsChild(newVfx);

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
    public void StartVFXDuration(SpecificVisualEffect specificVisualEffect, GeneralVfxFunctionality vfxObj)
    {
        StartCoroutine(specificVisualEffect.MoveObjectBackToPool(vfxObj.gameObject));
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
        SetUpAllVfxInGame();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        AframaxSceneManager.Instance.GetOnBeforeSceneChanged.AddListener(ReclaimAllVFXBeforeSceneChange);
    }

    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        AframaxSceneManager.Instance.GetOnBeforeSceneChanged.RemoveListener(ReclaimAllVFXBeforeSceneChange);
    }
    #endregion

    #region Getters

    #region GetVfx
    public SpecificVisualEffect GetMuzzleSmokeVfx() => _allVfxInGame[MUZZLE_SMOKE_ID];
    public SpecificVisualEffect GetEnemyBloodVfx() => _allVfxInGame[ENEMY_BLOOD_ID];
    public SpecificVisualEffect GetEnemyAttackWarningVfx() => _allVfxInGame[ENEMY_ATTACK_WARNING_ID];
    #endregion

    #endregion
}

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

    private int _poolingCounter =0;

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
            VfxManager.Instance.CreateVFXInPool(this);
            _poolingCounter++;
        }
        _poolingCounter = 0;
    }

    /// <summary>
    /// Determines how long the duration of the particle system should last before being reclaimed by the pool
    /// </summary>
    private void CalculateParticleDuration()
    {
        switch(_vfxDurationType)
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
        if(_vfxDurationType != EVfxDurationType.Infinite)
        {
            VfxManager.Instance.StartVFXDuration(this, _vfxPool[_poolingCounter]);
        }

        IterateVFXPool();

        return _vfxPool[previousCounterValue];
    }

    /// <summary>
    /// Plays the next vfx in the object pool
    /// Takes in a Transform and sets it to be its parent
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GeneralVfxFunctionality PlayNextVfxInPool(Transform parent, Vector3 location, Quaternion rotation)
    {
        GeneralVfxFunctionality currentVfx = PlayNextVfxInPool(location,rotation);
        currentVfx.gameObject.transform.SetParent(parent);
        currentVfx.transform.position = location;
        
        return currentVfx;
    }

    /// <summary>
    /// Moves a specific object back to the object pool
    /// </summary>
    /// <param name="vfxObject"></param>
    /// <returns></returns>
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
        foreach(GeneralVfxFunctionality vfx in _vfxPool)
        {
            ObjectPoolingParent.Instance.AddObjectAsChild(vfx.gameObject);
        }
    }

    /// <summary>
    /// Iterates through the object pool counter
    /// </summary>
    private void IterateVFXPool()
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
    /// <param name="objectToHide"></param>
    private void HideVfx(GameObject objectToHide)
    {
        objectToHide.SetActive(false);
    }

    #region Getters
    public GameObject GetVFXObject() => _vfxObject;
    #endregion
}