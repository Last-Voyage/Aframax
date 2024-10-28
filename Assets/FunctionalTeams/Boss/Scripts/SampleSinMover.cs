/*****************************************************************************
// File Name :         SampleSinMover.cs
// Author :            Tommy Roberts
// Creation Date :     10/27/2024
//
// Brief Description : creates basic back and forth motion for procedural animation
*****************************************************************************/
using UnityEngine;

/// <summary>
/// Just moves a transform back and forth between two points
/// </summary>
public class SampleSinMover : MonoBehaviour
{
    [SerializeField] private Vector3 _direction;
    [SerializeField] private Vector3 _start;

    /// <summary>
    /// set start pos
    /// </summary>
    private void Awake()
    {
        _start = transform.localPosition;
    }

    /// <summary>
    /// move the transform
    /// </summary>
    void Update()
    {
        //just move the object from a to b and back
        transform.localPosition = _start + _direction * Mathf.Sin(Time.timeSinceLevelLoad);
    }
}
