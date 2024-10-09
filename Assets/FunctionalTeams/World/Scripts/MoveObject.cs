/******************************************************************************
// File Name:       TheChunkChanger.cs
// Author:          Nick Rice
// Creation Date:   October 2nd, 2024
//
// Description:     This script moves the object it's attached to in the z direction
******************************************************************************/
using UnityEngine;

/// <summary>
/// Moves the object it's attached to in the z direction
/// </summary>
public class MoveObject : MonoBehaviour
{
    [SerializeField] private float _moveTheX = 0f;
    [SerializeField] private float _moveTheZ = 1f;

    /// <summary>
    /// Moves the object it's attached to in the z direction
    /// </summary>
    void Update()
    {
        gameObject.transform.position += new Vector3(_moveTheX, 0, _moveTheZ) * Time.deltaTime;
    }
}
