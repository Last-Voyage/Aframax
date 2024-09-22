/******************************************************************************
// File Name:       WeakPointSpawner.cs
// Author:          Ryan Swanson
// Creation Date:   September 22, 2024
//
// Description:     Spawns the weakpoints on some part of the boss (tentacles,etc)
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Spawns the weak points on the associated object
/// </summary>
public class WeakPointSpawner : MonoBehaviour
{
    [Tooltip("The time between spawing weakpoints")]
    [SerializeField] private float _weakPointSpawnInterval;
    [Tooltip("The number of weak points you need to kill to destroy this object")]
    [SerializeField] private float _weakPointsNeededToDestroy;

    private float _weakPointSpawnCounter = 0;
    private float _weakPointDestructionCounter = 0;

    [Space]
    [SerializeField] private GameObject _weakPointPrefab;
    private List<WeakPoint> _spawnedWeakpoints = new List<WeakPoint>();

    [Space]
    [Tooltip("The transforms on where weak points can be spawned")]
    [SerializeField] private List<Transform> _weakPointSpawnLocations;

    private UnityEvent<WeakPointSpawner> _allWeakPointsDestroyedEvent = new();

    private Coroutine _weakPointSpawnProcessCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        //Currently being handled in start but can be moved as needed
        StartWeakPointSpawning();
    }

    #region WeakPointSpawning
    /// <summary>
    /// Starts the process of spawning weak points
    /// </summary>
    private void StartWeakPointSpawning()
    {
        if (_weakPointSpawnProcessCoroutine != null)
        {
            return;
        }

        _weakPointSpawnProcessCoroutine = StartCoroutine(WeakPointSpawnProcess());
    }

    /// <summary>
    /// Spawns the weak points with a delay between each
    /// </summary>
    /// <returns></returns>
    private IEnumerator WeakPointSpawnProcess()
    {
        //Spawns weak points so long as we haven't reached the max amount allowed
        while (_weakPointSpawnCounter < _weakPointsNeededToDestroy)
        {
            SpawnWeakPoint();
            //Waits for the delay between spawning weak points
            yield return new WaitForSeconds(_weakPointSpawnInterval);
        }
    }

    /// <summary>
    /// Creates the weak point
    /// </summary>
    private void SpawnWeakPoint()
    {
        //Adds to the counter of weak points spawned
        _weakPointSpawnCounter++;

        //Determines the position
        Transform weakPointSpawnLoc = DeterminesWeakPointSpawnLocation();

        //Spawns the weak point childed to the spawn location.
        //If we need to make the weak point not be childed swap out weakPointSpawnLoc with
        //  weakPointSpawnLoc.transform , weakPointSpawnLoc.rotation
        GameObject newestWeakPoint = Instantiate(_weakPointPrefab, weakPointSpawnLoc);

        //Converts the weak point object into its associated script
        WeakPoint weakPointFunc = newestWeakPoint.GetComponent<WeakPoint>();

        //Tells this script to listen for the weak point dying
        weakPointFunc.GetWeakPointDeathEvent().AddListener(WeakPointDestroyed);

        //Adds the weakpoint to the list of spawned weak points
        _spawnedWeakpoints.Add(weakPointFunc);

        //Removes the option to spawn a weak point at the location
        _weakPointSpawnLocations.Remove(weakPointSpawnLoc);
    }

    /// <summary>
    /// Determines the transform to spawn the weak point at
    /// </summary>
    /// <returns></returns>
    private Transform DeterminesWeakPointSpawnLocation()
    {
        return _weakPointSpawnLocations[Random.Range(0, _weakPointSpawnLocations.Count)];
    }
    #endregion


    #region Weak Point Destruction
    /// <summary>
    /// Called when a weak point has been destroyed
    /// Listens to the weak point event for destruction
    /// </summary>
    /// <param name="weakPointDestroyed"></param>
    private void WeakPointDestroyed(WeakPoint weakPointDestroyed)
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
        //Checks if destruction counter is or exceeds needed amount destoyed
        if(_weakPointDestructionCounter >= _weakPointsNeededToDestroy)
        {
            MaxWeakPointsDestroyed();
        }    
    }

    /// <summary>
    /// Destroys the object when the max amount of weak points have been destoyed
    /// </summary>
    private void MaxWeakPointsDestroyed()
    {
        InvokeAllWeakPointsDestoyedEvent();
        //At some point we will probably swap this out with playing an animation
        Destroy(gameObject);
    }

    #endregion

    #region Events
    private void InvokeAllWeakPointsDestoyedEvent()
    {
        _allWeakPointsDestroyedEvent?.Invoke(this);
    }
    #endregion

    #region Getters
    public UnityEvent<WeakPointSpawner> GetAllWeakPointsDestroyedEvent() => _allWeakPointsDestroyedEvent;
    #endregion
}
