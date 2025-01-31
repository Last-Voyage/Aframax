/*****************************************************************************
// File Name :         ProceduralVine.cs
// Author :            Tommy Roberts
// Creation Date :     1/31/2025
//
// Brief Description : This script controls the procedural "room guarding movement" functionality
//                     as well as the procedural "lunge attack"
*****************************************************************************/
using System.Collections;
using UnityEngine;
using PathCreation;
using DG.Tweening;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Controls all functionality for the attack procedural animation and room movement
/// </summary>
public class ProceduralVine : MonoBehaviour
{
    [Header("Path Stuff")]
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private float speed = 5f;
    private float _distance = 0;
    private bool _isAttacking = false;

    [Header("Attack stuff")]
    [SerializeField] private Transform flowerHeadTransform;
    [SerializeField] private Transform followTransform;
    [SerializeField] private ChainIKConstraint chainIK;
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private float rearBackTime = 0.3f;
    [SerializeField] private float rearBackDistance = 1f;
    [SerializeField] private float waitAfterRearBackTime = .5f;
    [SerializeField] private float lungeToPlayerDuration = .2f;
    [SerializeField] private float moveBackAfterAttackTime = 2f;

    /// <summary>
    /// Updates the vine movement only right now
    /// </summary>
    private void Update() 
    {
        if(!_isAttacking)
        {
            //move along path
            MoveAlongPath();
        }
    }

    /// <summary>
    /// Moves the vine along the given path
    /// </summary>
    private void MoveAlongPath()
    {
        _distance += Time.deltaTime * speed;
        followTransform.position = pathCreator.path.GetPointAtDistance(_distance);
    }

    /// <summary>
    /// does the movement for the wind up and lunge attack
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    private IEnumerator Attack(Vector3 playerPosition)
    {
        _isAttacking = true;

        // Calculate the direction from the current object to the target object (X and Z only)
        Vector3 direction = (playerPosition - followTransform.position).normalized;
        followTransform.forward = direction;

        //rear back and raise up a little
        Vector3 posToRearBack = followTransform.position + -direction * rearBackDistance;
        followTransform.DOJump(posToRearBack, -.5f, 1, rearBackTime, false).SetEase(Ease.InSine);
        yield return new WaitForSeconds(rearBackTime);
        posToRearBack = followTransform.position + direction * rearBackDistance;
        followTransform.DOMove(posToRearBack, rearBackTime, false).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(rearBackTime + waitAfterRearBackTime);

        //redo direction from new position
        direction = (playerPosition - followTransform.position).normalized;
        var strikePos = followTransform.position + direction * Vector3.Distance(followTransform.position, playerPosition) + Vector3.up * .3f;

        //snaps to player
        followTransform.DOMove(strikePos, lungeToPlayerDuration, false).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(lungeToPlayerDuration);

        //move back to og position
        followTransform.DOJump(pathCreator.path.GetPointAtDistance(_distance), .2f, 1, moveBackAfterAttackTime, false).SetEase(Ease.InSine);
        yield return new WaitForSeconds(moveBackAfterAttackTime);

        // //attack done
        _isAttacking = false;
    }

    /// <summary>
    /// the original way we detected if the player was entering the room
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if(IsColliderPlayer(collider) && !_isAttacking)
        {
            //start attack
            Debug.Log(collider.transform, collider.transform);
            StartCoroutine(Attack(collider.transform.position));
        }
    }

    /// <summary>
    /// Checks for player collision script on a the gameobject of a given collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    private bool IsColliderPlayer(Collider collider)
    {
        return collider.gameObject.GetComponent<PlayerCollision>();
    }
}
