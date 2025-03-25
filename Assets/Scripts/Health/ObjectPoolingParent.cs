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
    
    [Tooltip("Contains a list of pooled objects even if they are not currently childed to the pooling parent")]
    private List<GameObject> _allPooledObjects = new();

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
    /// Adds an object 
    /// </summary>
    /// <param name="newObject"></param>
    public void InitiallyAddObjectToPool(GameObject newObject)
    {
        AddObjectAsChild(newObject);
        _allPooledObjects.Add(newObject);
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

    /// <summary>
    /// Reclaims all objects in any object pool
    /// </summary>
    private void ReclaimAllObjects()
    {
        foreach(GameObject pooledObject in _allPooledObjects)
        {
            pooledObject.SetActive(false);
            AddObjectAsChild(pooledObject);
        }
    }

    /// <summary>
    /// Subscribes to all events
    /// </summary>
    public void SubscribeToEvents()
    {
        AframaxSceneManager.Instance.GetOnBeforeSceneChanged.AddListener(ReclaimAllObjects);
    }

    /// <summary>
    /// Unsubscribes to all events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        AframaxSceneManager.Instance.GetOnBeforeSceneChanged.RemoveListener(ReclaimAllObjects);
    }

    /// <summary>
    /// Unsubscribes events on destruction
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
}
