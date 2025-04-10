/*****************************************************************************
// File Name :         WallCeilingAttack.cs
// Author :            Tommy Roberts
// Creation Date :     4/8/2025
//
// Brief Description : Activates the wall and ceiling attacks for chase sequence area
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// holds function to start animaitons and damage player
/// </summary>
public class WallCeilingAttack : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private float _damageToPlayer = 25f;

    /// <summary>
    /// gets animator
    /// </summary>
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// starts attack animation, called in PlayerCollision.cs
    /// </summary>
    public void ActivateAttack()
    {
        _animator.SetTrigger("attack");
    }

    /// <summary>
    /// damages player, called in PlayerCollision.cs
    /// </summary>
    public void DamagePlayer()
    {
        PlayerFunctionalityCore.Instance.GetPlayerHealth().TakeDamage(_damageToPlayer, null);
    }
}
