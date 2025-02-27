/******************************************************************************
// File Name:       PlayerSpawnPoint.cs
// Author:          Ryan Swanson
// Creation Date:   November 2, 2024
//
// Description:     Spawns in the player at the start of the scene
******************************************************************************/

using UnityEngine;

/// <summary>
/// Spawns the player as a child of this object
/// </summary>
public class PlayerSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public bool CanSpawnWithMovement = true;

    [Space]
    [SerializeField] private GameObject _playerPrefab;

    public static PlayerSpawnPoint Instance;

    /// <summary>
    /// Performs any needed set up of the spawn point
    /// </summary>
    public void SetUp()
    {
        Instance = this;
        SpawnPlayer();
    }

    /// <summary>
    /// Spawns in the player as a child of this
    /// </summary>
    private void SpawnPlayer()
    {
        Instantiate(_playerPrefab, transform);
    }
}
