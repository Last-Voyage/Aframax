/**********************************************************************************************************************
// File Name :         SteamAchievements.cs
// Author :            Andrew Stapay
// Creation Date :     7/9/2025
//
// Brief Description : Helper for achievements that require the player to shoot an object
**********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// Helper script for achievements that require the player to shoot an object
/// </summary>
public class ShootingAchievement : MonoBehaviour
{
    /// <summary>
    /// An enum for the different types of shooting achievements
    /// </summary>
    public enum ShootingAchievementType
    {
        HEADSHOT,
        DONT_BREAK_IT,
        FISH
    }

    // The achievement that shooting this object will unlock
    [SerializeField] private ShootingAchievementType _achievementType;

    private static int _nextId = 0;
    private int _id = 0;

    private void Awake()
    {
        if (_achievementType == ShootingAchievementType.HEADSHOT)
        {
            _id = _nextId;
            _nextId++;
        }
    }

    /// <summary>
    /// Called when a collider triggers this object
    /// </summary>
    /// <param name="other"> Information about the other collider in the collision </param>
    private void OnTriggerEnter(Collider other)
    {
        // See if this was hit by a harpoon
        if (other.TryGetComponent<HarpoonDamage>(out HarpoonDamage damage))
        {
            SteamAchievements.Instance.AchievementShot(_achievementType, _id);
        }
    }
}
