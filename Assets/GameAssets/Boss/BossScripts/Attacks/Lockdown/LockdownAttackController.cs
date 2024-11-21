/*****************************************************************************
// File Name :         LockdownAttackController.cs
// Author :            Andrea Swihart-DeCoster
// Contributor :       Ryan Swanson
// Creation Date :     10/26/24
//
// Brief Description : Controls the Aggressive Room 1 Attack Behavior
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the Aggressive Room Tongue Attack.
/// </summary>
public class LockdownAttackController : BaseBossAttack
{
    [Tooltip("Prefab of patrol enemy")]
    [SerializeField] private GameObject _patrolEnemyPrefab;
    [Tooltip("The core of the patrol enemy. The part that receives damage")]
    [SerializeField] private GameObject _coreEnemyPrefab;

    [Space]
    [SerializeField] private int _numberOfRoomsToAttack;
    private int _roomCoresDefeated = 0;

    private LockdownAttackEnemyController[] _lockdownEnemyControllers;

    /// <summary>
    /// Called when the attack ends to destroy all spawned enemies
    /// </summary>
    public static UnityEvent OnForceDestroyAllEnemies {get; private set;} = new();

    #region Enable & Action/Event Subscriptions

    /// <summary>
    /// Subscribes to needed events
    /// </summary>
    private void OnEnable()
    {
        SubscribeToEvents();   
    }

    /// <summary>
    /// Unsubscribes to events on disable
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    protected override void SubscribeToEvents()
    {
        _onBeginAttack.AddListener(BeginAttack);
    }

    /// <summary>
    /// Unsubscribes to all events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        _onBeginAttack.RemoveListener(BeginAttack);
    }

    #endregion
    /// <summary>
    /// Sets up all patrol locations before gameplay
    /// </summary>
    private void Start()
    {
        _isAttackActive = false;
        InitializePatrolLocations();
    }

    /// <summary>
    /// Loops through children and initializes patrol locations
    /// </summary>
    private void InitializePatrolLocations()
    {
        _lockdownEnemyControllers = GetComponentsInChildren<LockdownAttackEnemyController>();
        
        foreach (LockdownAttackEnemyController lockdownAttackEnemyController in _lockdownEnemyControllers)
        {
            lockdownAttackEnemyController.SetUpEnemyController(_patrolEnemyPrefab,_coreEnemyPrefab);
        }
    }

    /// <summary>
    /// Begins the enemy spawning
    /// </summary>
    protected override void BeginAttack()
    {
        // This is a passive attack this ends when it's scene is over
        // This should only subscribe during its lifetime as it's waiting for it's scene to end

        base.BeginAttack();

        AttackRandomRooms(_numberOfRoomsToAttack);
    }

    /// <summary>
    /// Attacks in random rooms
    /// </summary>
    /// <param name="attackNumber"> The amount of rooms to attack in </param>
    private void AttackRandomRooms(int attackNumber)
    {
        List<LockdownAttackEnemyController> remainingAttackRooms = _lockdownEnemyControllers.ToList();

        for (int i = 0; i < attackNumber; i++)
        {
            int randomPos = UnityEngine.Random.Range(0, remainingAttackRooms.Count);

            LockdownAttackEnemyController attackController = remainingAttackRooms[randomPos];
            attackController.AttackInRoom();
            remainingAttackRooms.Remove(attackController);
            attackController.GetOnCoreDestroyed().AddListener(RoomCoreDestroyed);
        }
    }

    /// <summary>
    /// Increments the number of room cores defeated
    /// Checks if all have been defeated
    /// </summary>
    private void RoomCoreDestroyed()
    {
        _roomCoresDefeated++;
        if (_roomCoresDefeated >= _numberOfRoomsToAttack)
        {
            EndAttack();
        }
    }

    /// <summary>
    /// Kills any living attack controllers that may remain
    /// </summary>
    private void KillAnyLivingAttackControllers()
    {
        foreach(LockdownAttackEnemyController enemyController in _lockdownEnemyControllers)
        {
            enemyController.ForceDestroy();
        }
    }

    /// <summary>
    /// Destroys all outstanding enemies and calls the endAttack event
    /// </summary>
    public override void EndAttack()
    {
        // This should only unsubscribe from it's scene if it began, this isn't in unsub from event
        BossAttackActSystem.Instance.GetOnAttackCompleted().RemoveListener(EndAttack);

        KillAnyLivingAttackControllers();

        base.EndAttack();
    }
}
