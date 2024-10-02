/*****************************************************************************
// File Name :         AttackTwoPatrolBehavior.cs
// Author :            Tommy Roberts
// Creation Date :     10/2/2024
//
// Brief Description : Controls collision behavior fr attack two
*****************************************************************************/
using UnityEngine;

public class AttackTwoPatrolBehavior : MonoBehaviour
{
    [SerializeField] private string _playerAttackTag = "Player";
    [SerializeField] private float _destoryAttackDelay = .1f;

    /// <summary>
    /// when player collides with this attack destory the attack
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("collided");
        if(other.gameObject.tag == (_playerAttackTag))
        {
            Debug.Log("collidedplayer");
            Destroy(gameObject, _destoryAttackDelay);
        }
    }
}
