/*****************************************************************************
// File Name :         VineAttackController.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
//                     Ryan Swanson
// Creation Date :     10/9/2024
//
// Brief Description : Controls the functionality for the bosses vine attack
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// contains functionality for full room attack
/// </summary>
public class VineAttackController : BaseBossAttack
{
    public static VineAttackController Instance;

    [SerializeField] private int _amountOfRoomsToAttack;
    [SerializeField] private float _attackInterval;

    private VineAttack[] _vineAttackRooms;

    private List<VineAttack> _currentAttackingRooms;

    private Coroutine _attackProcessCoroutine;


    private void Start()
    {
        EstablishInstance();

        _isAttackActive = false;
        GetAllRooms();
    }

    /// <summary>
    /// Creates the instance of the attack
    /// </summary>
    private void EstablishInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
            vine.PerformSetUp();
        }
    }

    /// <summary>
    /// Begins the attack functionality
    /// </summary>
    protected override void BeginAttack()
    {
        base.BeginAttack();
        //Determines which rooms are being attacked
        DetermineAttackRooms(_amountOfRoomsToAttack);
        SpawnVinesInRooms();
        _attackProcessCoroutine = StartCoroutine(AttackProcess());
    }

    /// <summary>
    /// Determines which rooms are being attacked
    /// </summary>
    /// <param name="roomCount"> The number of rooms to attack in</param>
    private void DetermineAttackRooms(int roomCount)
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
    /// Spawns vines in the rooms that are being attacked
    /// </summary>
    /// <param name="roomCount"></param>
    private void SpawnVinesInRooms()
    {
        foreach(VineAttack vine in _currentAttackingRooms)
        {
            vine.SpawnVine();
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
    /// Attacks until cancelled
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackProcess()
    {
        while(_isAttackActive)
        {
            AttackInRandomCurrentRoom();
            yield return new WaitForSeconds(_attackInterval);
        }
    }

    /// <summary>
    /// Forcibly stops the attack process if needed
    /// </summary>
    public void StopAttackProcess()
    {
        if(_attackProcessCoroutine != null)
        {
            StopCoroutine(_attackProcessCoroutine);
            _attackProcessCoroutine = null;
        }
        EndAttack();
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
