/******************************************************************************
// File Name:       VfxManager.cs
// Author:          Ryan Swanson
// Creation Date:   October 22nd, 2024
//
// Description:     Handles all vfx spawning and object pooling
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for external scripts to spawn vfx
/// </summary>
public class VfxManager : MainUniversalManagerFramework
{
    [SerializeField] private List<SpecificVisualEffect> tempList;

    public static VfxManager Instance;

    /// <summary>
    /// Goes through the list of all vfx in game and spawns all needed vfx. Sets up object pooling on each
    /// </summary>
    private void SetupAllVFX()
    {
        //Iterates through all vfx in game
        foreach(SpecificVisualEffect specificVisualEffect in tempList)
        {
            specificVisualEffect.InitializeObjectPool();
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

        //Gets the GeneralVfxFunctionality which acts as a mini manager of that set of vfx
        GeneralVfxFunctionality generalVfxFunctionality = newVfx.GetComponent<GeneralVfxFunctionality>();
        //Performs needed setup on the vfx
        generalVfxFunctionality.Setup();
        //Adds the created object to the pool
        specificVisualEffect.AddNewObjectToPool(generalVfxFunctionality);
    }

    /// <summary>
    /// Starts the process of moving the spawned vfx back to the pool
    /// Cannot be done by SpecificVisualEffect as its not a monobehavior
    /// </summary>
    /// <param name="specificVisualEffect">The specific visual effect the object belongs to</param>
    /// <param name="vfxObj">The object to be added back to the pool</param>
    public void VfxObjectSpawned(SpecificVisualEffect specificVisualEffect, GeneralVfxFunctionality vfxObj)
    {
        StartCoroutine(specificVisualEffect.MoveObjectBackToPool(vfxObj.gameObject));
    }

    /// <summary>
    /// THIS IS JUST FOR TESTING!!!!!
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            //I don't like how its currently done through index, will change later
            PlayVFXAtPoint(tempList[0], Vector3.zero);
        }
    }

    /// <summary>
    /// Plays a vfx at a location
    /// </summary>
    /// <param name="visualEffect"></param>
    /// <param name="location"></param>
    public void PlayVFXAtPoint(SpecificVisualEffect visualEffect, Vector3 location)
    {
        visualEffect.PlayNextVfxInPool(location);
    }

    /// <summary>
    /// Plays a vfx as a child of a specific object
    /// </summary>
    /// <param name="visualEffect"></param>
    /// <param name="parent"></param>
    public void PlayVFXChilded(SpecificVisualEffect visualEffect, Transform parent)
    {
        visualEffect.PlayNextVfxInPool(parent);
    }

    #region Base Manager
    /// <summary>
    /// Establishes the instance of VfxManager
    /// </summary>
    public override void SetupInstance()
    {
        base.SetupInstance();
        Instance = this;
    }

    /// <summary>
    /// Performs the needed setup for the manager including spawning and setting up VFX
    /// </summary>
    public override void SetupMainManager()
    {
        base.SetupMainManager();
        SetupAllVFX();
    }
    #endregion
}

/// <summary>
/// Acts as the owner of a specific vfx
/// Contains the object pool of that vfx
/// </summary>
[System.Serializable]
public class SpecificVisualEffect
{
    [SerializeField] private GameObject _vfxObject;
    [SerializeField] private float _spawnDuration;
    [SerializeField] private int _poolingAmount;

    private int _poolingCounter =0;

    public GeneralVfxFunctionality[] _vfxPool;

    /// <summary>
    /// Sets up the object pool by spawning all needed objects
    /// </summary>
    public void InitializeObjectPool()
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
    public GeneralVfxFunctionality PlayNextVfxInPool(Vector3 location)
    {
        int previousCounterValue = _poolingCounter;

        _vfxPool[_poolingCounter].gameObject.SetActive(true);
        _vfxPool[_poolingCounter].transform.position = location;
        _vfxPool[_poolingCounter].StartAllVfx();

        VfxManager.Instance.VfxObjectSpawned(this, _vfxPool[_poolingCounter]);
        IterateVFXPool();

        return _vfxPool[previousCounterValue];
    }

    /// <summary>
    /// Plays the next vfx in the object pool
    /// Takes in a Transform and sets it to be its parent
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GeneralVfxFunctionality PlayNextVfxInPool(Transform parent)
    {
        GeneralVfxFunctionality currentVfx = PlayNextVfxInPool(parent.position);
        currentVfx.gameObject.transform.SetParent(parent);
        
        return currentVfx;
    }

    /// <summary>
    /// Moves a specific object back to the object pool
    /// </summary>
    /// <param name="vfxObject"></param>
    /// <returns></returns>
    public IEnumerator MoveObjectBackToPool(GameObject vfxObject)
    {
        yield return new WaitForSeconds(_spawnDuration);
        HideVfx(vfxObject);
        //TO DO, REMOVE OBJECT PARENT AND SET IT TO BE THE OBJECT POOLING PARENT
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