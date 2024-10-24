/*****************************************************************************
// File Name :         ObjectPoolingParent.cs
// Author :            Ryan Swanson
// Creation Date :     10/22/24
//
// Brief Description : Acts as the parent to all objects being pooled. Cleans up the hierarchy
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains the cleanliness of the hierarchy by childing all objects being pooled
/// </summary>
public class ObjectPoolingParent : MonoBehaviour
{
    public static ObjectPoolingParent Instance;

    /// <summary>
    /// Establishes the instances
    /// </summary>
    public void SetupInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Adds a specific child
    /// </summary>
    /// <param name="newChild"> The child to add</param>
    public void AddObjectAsChild(GameObject newChild)
    {
        newChild.transform.SetParent(transform);
    }

    /// <summary>
    /// Removes a specific child
    /// </summary>
    /// <param name="child"> The child to remove </param>
    public void RemoveObjectAsChild(GameObject child)
    {
        child.transform.SetParent(null);
    }
}
