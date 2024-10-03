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
    /// <summary>
    /// Moves the object it's attached to in the z direction
    /// </summary>
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, 1f) * Time.deltaTime;
    }
}
