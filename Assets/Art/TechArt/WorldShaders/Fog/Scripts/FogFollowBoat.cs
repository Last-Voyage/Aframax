/**************************************************************************
// File Name :          FogFollowBoat.cs
// Author :             Miles Rogers
// Creation Date :      11/1/2024
//
// Brief Description :  Make the low-lying fog follow the boat object
**************************************************************************/

using UnityEngine;

/// <summary>
/// Make the low-lying fog follow the boat object.
/// </summary>
public class FogFollowBoat : MonoBehaviour
{
    /// <summary>
    /// The boat object in the scene.
    /// </summary>
    [SerializeField] private Transform _boatObject;

    // Follow the boat on the X and Z axis (maintain Y position)
    void Update()
    {
        Vector3 pos = _boatObject.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }
}
