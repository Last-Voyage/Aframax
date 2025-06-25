/**********************************************************************************************************************
// File Name :         SteamAchievements.cs
// Author :            Andrew Stapay
// Creation Date :     6/24/2025
//
// Brief Description : Keeps track of achievements to send to Steam
**********************************************************************************************************************/
using UnityEngine;
using Steamworks;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// Keeps track of achievements and communicates them with Steam
/// </summary>
public class SteamAchievements : MonoBehaviour
{
    /// <summary>
    /// Container class for all information related to an achievement
    /// VARIABLES PASSED INTO THE CONSTRUCTOR MUST MIRROR THE INFORMATION ON THE STEAMWORKS ACHIEVEMENT
    /// CONFIGURATION PAGE
    /// </summary>
    private class AchievementContainer
    {
        public Achievement AchievementId;
        public bool Achieved;

        public AchievementContainer(Achievement achievementId)
        {
            AchievementId = achievementId;
            Achieved = false;
        }
    }

    /// <summary>
    /// Enum for the *logical IDs* of each achievement
    /// </summary>
    private enum Achievement : int
    {

    }

    // List of achievements
    private AchievementContainer[] _achievements = { };

    // Steam Communication
    private CGameID _gameId;
    private Callback<UserStatsReceived_t> _userStatsReceived;

    /// <summary>
    /// Called when the gameobject is enabled
    /// Used to subscribe to events and set up communication with Steam
    /// </summary>
    private void OnEnable()
    {
        SubscribeToEvents();

        _userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
    }

    /// <summary>
    /// Called when the gameobject is disabled
    /// Used to unsubscribe to events and remove communication with Steam
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();

        _userStatsReceived = null;
    }

    /// <summary>
    /// Unlocks an achievement in Steam
    /// </summary>
    /// <param name="achievement"> The achievement to unlock, in the form of an AchievementContainer </param>
    private void UnlockAchievement(AchievementContainer achievement)
    {
        if (SteamManager.Initialized)
        {
            achievement.Achieved = true;

            SteamUserStats.SetAchievement(achievement.AchievementId.ToString());
        }
    }

    /// <summary>
    /// Called when we receive information about the user's Steam stats
    /// Used to mark already-completed achievements
    /// </summary>
    /// <param name="pCallback"> Information about the communication from Steam </param>
    private void OnUserStatsReceived(UserStatsReceived_t pCallback)
    {
        if (!SteamManager.Initialized)
            return;

        // we may get callbacks for other games' stats arriving, ignore them
        if ((ulong)_gameId == pCallback.m_nGameID)
        {
            if (EResult.k_EResultOK == pCallback.m_eResult)
            {
                // load achievements
                foreach (AchievementContainer ach in _achievements)
                {
                    SteamUserStats.GetAchievement(ach.AchievementId.ToString(), out ach.Achieved);
                }
            }
        }
    }

    /// <summary>
    /// Subscribes methods from this script to various events
    /// </summary>
    private void SubscribeToEvents()
    {

    }

    /// <summary>
    /// Unsubscribes methods from this script from various events
    /// </summary>
    private void UnsubscribeToEvents()
    {

    }
}
