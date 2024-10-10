/*****************************************************************************
// File Name :         BossAttacksManager.cs
// Author :            Tommy Roberts
// Creation Date :     10/2/2024
//
// Brief Description : Controls first two boss attacks for our first playable build
*****************************************************************************/
using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// manager for the boss attacks currently, will be restructured for future builds
/// </summary>
public class BossAttacksManager : MonoBehaviour
{
    //general
    private bool _attackInProgress = false;
    [Tooltip("After an attack is finished how long to wait before starting next one")]
    [SerializeField] private float _timeBetweenAttacks = 5f;
    [Tooltip("this is how long before boss attacks actually begin if you want start delay")]
    [SerializeField] private float _timeAfterStartToStartAttacks;
    private Coroutine _chooseAttacksRepeatedlyCoroutine;

    public static BossAttacksManager Instance;

    //the attack events, add more as more attacks are made
    private Action[] _bossAttacks;
    public static event Action FullRoomAttack;
    public static event Action PatrolRoomAttack;

    /// <summary>
    /// creates instance of BossManager
    /// </summary>
    private void Awake() {
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
    /// Starts random attacks at the set interval
    /// </summary>
    void Start()
    {
        InitializeBossAttackList();
        _chooseAttacksRepeatedlyCoroutine = StartCoroutine(ChooseAttacksRepeatedly());
    }

    /// <summary>
    /// puts all attack actions into an array to be called randomly
    /// </summary>
    private void InitializeBossAttackList()
    {
        //add more attacks here
        _bossAttacks = new Action[]{FullRoomAttack, PatrolRoomAttack};
    }

    /// <summary>
    /// Chooses attakcs randomly when there is not a current attack in progress
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChooseAttacksRepeatedly()
    {
        Debug.Log("attack?");
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
