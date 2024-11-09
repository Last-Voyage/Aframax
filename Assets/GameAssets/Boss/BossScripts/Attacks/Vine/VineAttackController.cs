/*****************************************************************************
// File Name :         VineAttackController.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
//                     Ryan Swanson
// Creation Date :     10/9/2024
//
// Brief Description : Controls the functionality for the bosses vine attack
*****************************************************************************/

using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// contains functionality for full room attack
/// </summary>
public class VineAttackController : BaseBossAttack
{
    [SerializeField] private int _roomsToAttack;

    private VineAttack[] _vineAttackRooms;

    private int _numTentaclesDestroyed;
    private int _numTentaclesSpawned;

    private void Start()
    {
        _isAttackActive = false;
        GetAllRooms();
    }

    private void GetAllRooms()
    {
        _vineAttackRooms = GetComponentsInChildren<VineAttack>();
    }

    /// <summary>
    /// links attack to boss attack manager
    /// </summary>
    private void OnEnable() 
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        _onBeginAttack.AddListener(BeginAttack);
    }

    /// <summary>
    /// Unsubscribes to any needed events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        _onBeginAttack.RemoveListener(BeginAttack);
    }

    /// <summary>
    /// unlinks attack script from boss manager
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Begins the attack functionality
    /// </summary>
    protected override void BeginAttack()
    {
        base.BeginAttack();
        _numTentaclesDestroyed = 0;

        AttackInRooms(_roomsToAttack);
    }

    private void AttackInRooms(int roomCount)
    {
        if(roomCount >= _vineAttackRooms.Length)
        {
            Debug.LogWarning("More vines spawned than available rooms. Please enter a lower number");
        }
        
  
        VineAttack[] randomRooms = new VineAttack[roomCount];
        List<VineAttack> availableRooms = new();

        availableRooms.AddRange(_vineAttackRooms);
        
        for(int i = 0; i >= roomCount; i++)
        {
            randomRooms[i] = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];
            availableRooms.Remove(randomRooms[i]);
            randomRooms[i].StartVineAttack();
        }
    }


    /// <summary>
    /// Adds a listener to the tentacle destroyed event
    /// </summary>
    /// <param name="weakPointHandler"></param>
    private void AddTentacleDestroyedListener(WeakPointHandler weakPointHandler)
    {
        weakPointHandler.GetOnAllWeakPointsDestroyedEvent().AddListener(OnTentacleDestroyed);
    }

    /// <summary>
    /// Called when a tentacle is destroyed to track this attacks lifetime
    /// </summary>
    /// <param name="weakPointHandler"></param>
    private void OnTentacleDestroyed(WeakPointHandler weakPointHandler)
    {
        _numTentaclesDestroyed++;

        if (_numTentaclesDestroyed == _numTentaclesSpawned)
        {
            EndAttack();
        }
    }
}
