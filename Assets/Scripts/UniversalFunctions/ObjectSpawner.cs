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
    /// <summary>
    /// Spawns an object based on the given parameters.  Used for spawning in worldspace
    /// </summary>
    /// <param name="prefab">The prefab you want to spawn in</param>
    /// <param name="position">The position in worldspace you want to spawn at</param>
    /// <param name="rotation">Rotation you want to spawn at</param>
    public void SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Instantiate(prefab, position, rotation);
    }

    /// <summary>
    /// Spawns an object based on the given parameters.  Used for spawning as a child
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
