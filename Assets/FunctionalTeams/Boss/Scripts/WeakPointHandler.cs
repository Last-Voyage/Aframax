/******************************************************************************
// File Name:       WeakPointHandler.cs
// Author:          Ryan Swanson
// Contributors:    Andrea Swihart-DeCoster
// Creation Date:   September 22, 2024
//
// Description:     Spawns the weakpoints on some part of the boss (tentacles,etc)
******************************************************************************/

using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Spawns the weak points on the associated object
/// </summary>
public class WeakPointHandler : MonoBehaviour
{
    [Header("Spawning Options")]
    [Tooltip("The time before the first weak point spawns")]
    [SerializeField] private float _delayFirstSpawn;
    [Tooltip("The time between spawning weakpoints")]
    [SerializeField] private float _spawnInterval;

    [Header("Weak Point Options")]
    [SerializeField] private GameObject _weakPointPrefab;

    [Tooltip("The number of weak points you need to kill to destroy this object")]
    [SerializeField] private float _numNeededToDestroy;

    private List<Transform> _possibleSpawnLocations;
    private List<WeakPoint> _spawnedWeakpoints = new List<WeakPoint>();
    private GameObject _spawnedWeakPointsParent;

    private GameObject _parentGameObject;

    private float _weakPointSpawnCounter = 0;
    private float _weakPointDestructionCounter = 0;

    private Coroutine _weakPointSpawnProcessCoroutine;

    private UnityEvent<WeakPointHandler> _allWeakPointsDestroyedEvent = new();

    private void Awake()
    {
        _parentGameObject = transform.parent.gameObject;
        InitializeSpawnLocations();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartWeakPointSpawning(); 
    }

    /// <summary>
    /// Initializes the list of spawn locations.
    /// </summary>
    private void InitializeSpawnLocations()
    {
        GameObject spawnLocationsObject = transform.GetChild(0).gameObject;

        _possibleSpawnLocations = new List<Transform>();
        spawnLocationsObject.GetComponentsInChildren(_possibleSpawnLocations);
        _possibleSpawnLocations.RemoveAt(0);

        // Creates a parent object for the spawned prefabs to nest under at runtime
        _spawnedWeakPointsParent = new GameObject("Spawned Weak Points");
        _spawnedWeakPointsParent.transform.parent = transform;
    }

    #region WeakPointSpawning

    /// <summary>
    /// Starts the process of spawning weak points
    /// </summary>
    private void StartWeakPointSpawning()
    {
        if (_weakPointSpawnProcessCoroutine == null)
        {
            _weakPointSpawnProcessCoroutine = StartCoroutine(WeakPointSpawnProcess());
        }
    }

    /// <summary>
    /// Spawns the weak points with a delay between each
    /// </summary>
    /// <returns></returns>
    private IEnumerator WeakPointSpawnProcess()
    {
        //The time before the first weak point spawns
        yield return new WaitForSeconds(_delayFirstSpawn);

        //Spawns weak points so long as we haven't reached the max amount allowed
        while (_weakPointSpawnCounter < _numNeededToDestroy)
        {
            SpawnWeakPoint();
            //Waits for the delay between spawning weak points
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    /// <summary>
    /// Spawns a weak point at one of the possible spawn locations
    /// </summary>
    private void SpawnWeakPoint()
    {
        Transform weakPointSpawnLoc = DetermineWeakPointSpawnLocation();

        /* The weak point destroyed function is added as a listener to the spawns  weak points death event so the 
         * Handler can properly track its lifespan.
        */
        WeakPoint spawnedWeakPoint = Instantiate(_weakPointPrefab, weakPointSpawnLoc.position,
            weakPointSpawnLoc.rotation).GetComponentInChildren<WeakPoint>();
        _spawnedWeakpoints.Add(spawnedWeakPoint);

        spawnedWeakPoint.transform.parent = _spawnedWeakPointsParent.transform;
           
        _weakPointSpawnCounter++;
        spawnedWeakPoint.GetWeakPointDeathEvent().AddListener(WeakPointDestroyed);

        //Removes the option to spawn successive weak points at the same location.
        _possibleSpawnLocations.Remove(weakPointSpawnLoc);
    }

    /// <summary>
    /// Determines the transform to spawn the weak point at
    /// </summary>
    /// <returns></returns>
    private Transform DetermineWeakPointSpawnLocation()
    {
        return _possibleSpawnLocations[UnityEngine.Random.Range(0, _possibleSpawnLocations.Count)];
    }

    #endregion

    #region Weak Point Destruction

    /// <summary>
    /// Called when a weak point has been destroyed
    /// Listens to the weak point event for destruction
    /// </summary>
    private void WeakPointDestroyed()
    {
        //Adds to the counter of weak points destroyed
        _weakPointDestructionCounter++;

        CheckForMaxWeakPointsDestroyed();
    }

    /// <summary>
    /// Checks if the amount of weak points destroys is equal to or exceeds the amount needed to destroy 
    /// </summary>
    private void CheckForMaxWeakPointsDestroyed()
    {
        //Checks if destruction counter is or exceeds needed amount destroyed
        if(_weakPointDestructionCounter >= _numNeededToDestroy)
        {
            MaxWeakPointsDestroyed();
        }    
    }

    /// <summary>
    /// Destroys the object when the max amount of weak points have been destroyed
    /// </summary>
    private void MaxWeakPointsDestroyed()
    {
        InvokeAllWeakPointsDestroyedEvent();
        RuntimeSfxManager.APlayOneShotSFX?.Invoke(FmodSfxEvents.Instance.LimbDestroyed, _parentGameObject.transform.position);

        Destroy(_parentGameObject);
    }

    #endregion

    #region Events
    /// <summary>
    /// Calls the weak destroyed event
    /// </summary>
    private void InvokeAllWeakPointsDestroyedEvent()
    {
        _allWeakPointsDestroyedEvent?.Invoke(this);
    }
    #endregion

    #region Getters

    public UnityEvent<WeakPointHandler> GetAllWeakPointsDestroyedEvent() => _allWeakPointsDestroyedEvent;

    #endregion
}
