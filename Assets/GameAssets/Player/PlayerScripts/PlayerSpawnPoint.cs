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
    [SerializeField] private GameObject _playerPrefab;

    // Start is called before the first frame update
    void Awake()
    {
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
