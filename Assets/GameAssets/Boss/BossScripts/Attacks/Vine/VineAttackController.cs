/*****************************************************************************
// File Name :         VineAttackController.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
//                     Ryan Swanson
// Creation Date :     10/9/2024
//
// Brief Description : Controls the functionality of handling all vine attacks
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Contains functionality to spawn and control multiple vine attacks
/// </summary>
public class VineAttackController : BaseBossAttack
{
    /// <summary>
    /// Unity doesn't seem to like Serializing an array of arrays but it likes this
    /// </summary>
    [SerializeField] private VineAttackGroup[] _attackOrder;
    [SerializeField] private float _attackInterval;
    [Tooltip("rate at which the limp idle sfx should loop")]
    [SerializeField] private float _loopRate;
    private VineAttack[] _vineAttackRooms;

    private List<VineAttack> _currentAttackingRooms;

    private Coroutine _attackProcessCoroutine;

    WaitForSeconds _attackIntervalDelay;

    private void Start()
    {
        _isAttackActive = false;
        _attackIntervalDelay = new WaitForSeconds(_attackInterval);
        GetAllRooms();
    }

    /// <summary>
    /// Gets all rooms that are available to attack in
    /// </summary>
    private void GetAllRooms()
    {
        _vineAttackRooms = GetComponentsInChildren<VineAttack>();

        //Tells each vine to set initial values and subscribe to events
        //Not done in start in order make sure events are established first
        foreach(VineAttack vine in _vineAttackRooms)
        {
            vine.PerformSetUp(this);
        }
    }

    /// <summary>
    /// Begins the attack functionality
    /// </summary>
    protected override void BeginAttack()
    {
        base.BeginAttack();
        //Determines which rooms are being attacked

        SpawnVinesInRooms();
        _attackProcessCoroutine = StartCoroutine(AttackProcess());
    }

    /// <summary>
    /// Functionality to randomly pick attack rooms
    /// Not currently in use as design wants to pick the attacks but I believe this functionality should remain
    /// </summary>
    /// <param name="roomCount"> The number of rooms to attack in</param>
    private void DetermineRandomAttackRooms(int roomCount)
    {
        //Check that the number of rooms to attack doesn't exceed the amount of rooms that exist
        if (roomCount >= _vineAttackRooms.Length)
        {
            Debug.LogWarning("More vines spawned than available rooms. Please enter a lower number");
        }

        _currentAttackingRooms = new(roomCount);
        List<VineAttack> availableRooms = new();

        availableRooms.AddRange(_vineAttackRooms);

        //Randomly picks rooms without duplicates
        for (int i = 0; i < roomCount; i++)
        {
            int randomNum = UnityEngine.Random.Range(0, availableRooms.Count);

            _currentAttackingRooms.Add(availableRooms[randomNum]);
            availableRooms.Remove(_currentAttackingRooms[i]);
        }
    }

    /// <summary>
    /// Attacks in a random active room
    /// </summary>
    private void AttackInRandomCurrentRoom()
    {
        int randomVine = UnityEngine.Random.Range(0, _currentAttackingRooms.Count);
        VineAttack vineAttack = _currentAttackingRooms[randomVine];
        vineAttack.StartAttackProcess();
    }

    /// <summary>
    /// Spawns vines in the rooms that are being attacked
    /// </summary>
    /// <param name="roomCount"></param>
    private void SpawnVinesInRooms()
    {
        foreach(VineAttack vine in _vineAttackRooms)
        {
            vine.SpawnVine();
            //Sfx for limb idle
            base.PlayIdleLoop(_loopRate);
        }
    }

    /// <summary>
    /// Attacks until cancelled
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackProcess()
    {
        foreach(VineAttackGroup attackOrder in _attackOrder)
        {
            UseAllAttacksInAttackGroup(attackOrder);
            yield return _attackIntervalDelay;
        }
        //For SFX of limb idle
        base.DestroyIdleLoop();
        EndAttack();
    }

    /// <summary>
    /// Uses all attacks in the current group of attacks
    /// </summary>
    /// <param name="attackOrder"></param>
    private void UseAllAttacksInAttackGroup(VineAttackGroup attackOrder)
    {
        foreach (VineAttack vineAttack in attackOrder.VineAttacksThisGroup)
        {
            vineAttack.StartAttackProcess();
        }
    }

    /// <summary>
    /// Ends the attack
    /// </summary>
    protected override void EndAttack()
    {
        base.EndAttack();
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
}

/// <summary>
/// Contains an array of vine attacks
/// Can contain additional functionality as needed
/// </summary>
[System.Serializable]
public class VineAttackGroup
{
    [field:SerializeField] public VineAttack[] VineAttacksThisGroup { get; private set; }
}
