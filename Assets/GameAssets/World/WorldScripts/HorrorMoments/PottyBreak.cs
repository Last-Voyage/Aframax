/**********************************************************************************************************************
// File Name :          PottyBreak.cs
// Author :             Andrew Stapay
// Creation Date :      2/5/25
//
// Brief description :  Triggers the Potty Break Horror Moment when the player takes the key item.
**********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// Spawns the needed tentacle for the Potty Break Horror Moment
/// </summary>
public class PottyBreak : MonoBehaviour
{
    // The tentacle to spawn
    [SerializeField] private GameObject _tentacle;

    /// <summary>
    /// Spawns the Potty Break tentacle
    /// </summary>
    public void SpawnTentacle()
    {
        Instantiate(_tentacle, transform.position, Quaternion.identity);
        CameraManager.Instance.InvokeOnJumpscare();
    }
}