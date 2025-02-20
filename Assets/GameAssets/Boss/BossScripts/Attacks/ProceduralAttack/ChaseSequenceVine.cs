/*****************************************************************************
// File Name :         ChaseSequenceVine.cs
// Author :            Tommy Roberts
// Creation Date :     2/19/2025
//
// Brief Description : This script controls an individual vine in the chase groups
*****************************************************************************/
using PathCreation;
using UnityEngine;

/// <summary>
/// This class controls an individual vine in a chase group
/// </summary>
public class ChaseSequenceVine : MonoBehaviour
{
    private float _chaseDistance;
    [SerializeField] private Transform _head;
    private float _chaseSpeed;
    [SerializeField] private PathCreator _chasePath;
    private bool _isChasing = false;

    /// <summary>
    /// Update calls the chase function when appropriate
    /// </summary>
    private void Update()
    {
        if( _isChasing && _chasePath.path.length > _chaseDistance + .3f) //added buffer number
        {
            Chasing();
        }
    }

    /// <summary>
    /// Follows the chase path
    /// </summary>
    private void Chasing()
    {
        _chaseDistance += Time.deltaTime * _chaseSpeed;
        _head.position = _chasePath.path.GetPointAtDistance(_chaseDistance);
        _head.right = _chasePath.path.GetDirectionAtDistance(_chaseDistance);
    }

    /// <summary>
    /// Starts the chase of this vine
    /// </summary>
    /// <param name="speed"></param>
    public void ActivateChase(float speed)
    {
        _chaseSpeed = speed;
        _isChasing = true;
    }
}
