/*****************************************************************************
// File Name :         ChaseSequenceVine.cs
// Author :            Tommy Roberts
// Contributor:        Ryan Swanson
// Creation Date :     2/19/2025
//
// Brief Description : This script controls an individual vine in the chase groups
*****************************************************************************/
using PathCreation;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// This class controls an individual vine in a chase group
/// </summary>
public class ChaseSequenceVine : MonoBehaviour
{
    private float _chaseDistance;
    public Transform Head;
    private float _chaseSpeed;
    [SerializeField] private PathCreator _chasePath;
    private bool _isChasing = false;

    internal UnityEvent OnChaseEnd = new();

    /// <summary>
    /// Starts the chase of this vine
    /// </summary>
    /// <param name="speed"></param>
    public void ActivateChase(float speed)
    {
        _chaseSpeed = speed;
        _isChasing = true;

        StartCoroutine(MoveProcess());
    }

    /// <summary>
    /// Coroutine that handles the process of the movement during the chase sequence
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProcess()
    {
        while (_isChasing && _chasePath.path.length > _chaseDistance + .3f) //added buffer number
        {
            Chasing();
            yield return null;
        }
        ChaseEnd();
    }

    /// <summary>
    /// Follows the chase path
    /// </summary>
    private void Chasing()
    {
        _chaseDistance += Time.deltaTime * _chaseSpeed;
        Head.position = _chasePath.path.GetPointAtDistance(_chaseDistance);
        Head.right = _chasePath.path.GetDirectionAtDistance(_chaseDistance);
    }

    /// <summary>
    /// Function called when the chase sequence reaches its end point
    /// </summary>
    private void ChaseEnd()
    {
        OnChaseEnd?.Invoke();
    }
}
