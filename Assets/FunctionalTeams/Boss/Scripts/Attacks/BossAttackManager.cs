/*****************************************************************************
// File Name :         BossAttacksManager.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
// Creation Date :     10/2/2024
//
// Brief Description : Controls first two boss attacks for our first playable build
*****************************************************************************/
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

/// <summary>
/// manager for the boss attacks currently, will be restructured for future builds
/// </summary>
public class BossAttackManager : MonoBehaviour
{
    public static BossAttackManager Instance;

    private bool _attackInProgress = false;

    [Tooltip("After an attack is finished how long to wait before starting next one")]
    [SerializeField] private float _timeBetweenAttacks = 5f;

    [Tooltip("this is how long before boss attacks actually begin if you want start delay")]
    [SerializeField] private float _timeAfterStartToStartAttacks;

    private Coroutine _chooseAttacksRepeatedlyCoroutine;

    #region Attack Events

    private Action[] _bossAttacks;
    public static event Action BeginRoomLockdownAttack;
    public static event Action BeginInteriorTongueAttack;

    #endregion

    /// <summary>
    /// creates instance of BossManager
    /// </summary>
    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    /// <summary>
    /// just for testing
    /// </summary>
    private void Update()
    {
        // Test the begin interior attack until attack system is setup
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            BeginAttackAct1();
        }
    }

    /// <summary>
    /// Begins attacks in the first act
    /// </summary>
    private void BeginAttackAct1()
    {
        BeginInteriorTongueAttack?.Invoke();
        BeginRoomLockdownAttack?.Invoke();
    }

    /// <summary>
    /// puts all attack actions into an array to be called randomly
    /// </summary>
    private void InitializeBossAttackList()
    {
        //add more attacks here
        _bossAttacks = new Action[]{BeginRoomLockdownAttack, BeginInteriorTongueAttack};
    }

    /// <summary>
    /// Chooses attacks randomly when there is not a current attack in progress
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChooseAttacksRepeatedly()
    {
        yield return new WaitForSeconds(_timeAfterStartToStartAttacks);

        while(true)
        {
            if(!_attackInProgress)
            {
                _attackInProgress = true;
                yield return new WaitForSeconds(_timeBetweenAttacks);
                //call random attack to start from the list of attacks
                _bossAttacks[UnityEngine.Random.Range(0, _bossAttacks.Length)]?.Invoke();
            }
            yield return null;
        }
    }

    public bool AttackInProgress
    {
        get { return _attackInProgress; }
        set { _attackInProgress = value; }
    }
}
