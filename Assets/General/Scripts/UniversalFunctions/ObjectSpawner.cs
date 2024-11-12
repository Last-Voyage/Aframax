/**********************************************************************************************************************
// File Name :         ObjectSpawner.cs
// Author :            Alex Kalscheur
// Creation Date :     11/12/24
// 
// Brief Description : Used as a way for designers to access prefab instantiation from the editor
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Used as a way for designers to access prefab instantiation from the editor
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Tooltip("The prefab you want to spawn in")]
    [SerializeField] GameObject _prefab;
    [Tooltip("The position you want to spawn the object in.  " +
             "Will be in relation to the parent if defined.  " +
             "Otherwise will spawn in worldspace")]
    [SerializeField] Vector3 _position;
    [Tooltip("The rotation the object will spawn in at" +
             "Will be in relation to the parent if defined.  " +
             "Otherwise will spawn in worldspace")]
    [SerializeField] Quaternion _rotation;
    [Tooltip("(optional) Object you want the prefab instance childed to")]
    [SerializeField] Transform _parent;
    
    /// <summary>
    /// Default function that uses the defined serialized objects and variables
    /// </summary>
    public void SpawnObject()
    {
        if(_parent == null)
        {
            Instantiate(_prefab, _position, _rotation);
        }
        else
        {
            Instantiate(_prefab, _position, _rotation, _parent);
        }        
    }

    /// <summary>
    /// Override function for designers' ease.  Used for spawning in worldspace
    /// </summary>
    /// <param name="prefab">The prefab you want to spawn in</param>
    /// <param name="position">The position in worldspace you want to spawn at</param>
    /// <param name="rotation">Rotation you want to spawn at</param>
    public void SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Instantiate(prefab, position, rotation);
    }

    /// <summary>
    /// Override function for designers' ease.  Used for spawning as a child
    /// </summary>
    /// <param name="prefab">The prefab you want to spawn in</param>
    /// <param name="position">The position in worldspace you want to spawn at</param>
    /// <param name="rotation">Rotation you want to spawn at</param>
    /// <param name="parent">The object you want the prefab instance to be childed to</param>
    public void SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        Instantiate(prefab, position, rotation, parent);
    }
}
