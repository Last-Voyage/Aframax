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
using Unity.VisualScripting;
using System.Collections.Generic;

/// <summary>
/// Keeps track of achievements and communicates them with Steam
/// </summary>
public class SteamAchievements : MonoBehaviour
{
    /// <summary>
    /// Enum for the *logical IDs* of each achievement
    /// </summary>
    private enum Achievement : int
    {
        LORE_KEEPER,
        WEED_WHACKER,
        HEADSHOT,
        TREE_HUGGER,
        MINIMALIST,
        YOU_DO_CARE,
        DONT_BREAK_IT,
        FISH
    }

    public static SteamAchievements Instance;

    // Tracking variables
    private HashSet<string> _foundNotes = new HashSet<string>();
    private int _monstersKilled = 0;
    private HashSet<int> _statuesShot = new HashSet<int>();
    private bool _hasUsedResource = false;

    // Achievement target values
    [SerializeField] private const int _MAX_NOTES = 8;
    [SerializeField] private const int _MAX_ENEMIES = 10;
    [SerializeField] private const int _MAX_STATUES = 5;

    /// <summary>
    /// Called when the gameobject is enabled
    /// Used to subscribe to events and set up communication with Steam
    /// </summary>
    private void OnEnable()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        if (Instance.IsUnityNull())
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Unlocks an achievement in Steam
    /// </summary>
    /// <param name="achievement"> The achievement to unlock, in the form of an AchievementContainer </param>
    private void UnlockAchievement(Achievement ach)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetAchievement(ach.ToString());
        }
    }

    /// <summary>
    /// Tracks when a resource is used
    /// </summary>
    public void ResourceUsed()
    {
        _hasUsedResource = true;
    }

    /// <summary>
    /// Tracks when a note is found and unlocks the respective achievement when a certain number are found
    /// </summary>
    /// <param name="obj"> The GameObject of the found note for tracking </param>
    public void NoteFound(GameObject obj)
    {
        _foundNotes.Add(obj.name);

        if (_foundNotes.Count >= _MAX_NOTES)
        {
            UnlockAchievement(Achievement.LORE_KEEPER);
        }
    }

    /// <summary>
    /// Tracks when a monster is killed and unlocks the respective acheivement when a certain number are killed
    /// </summary>
    public void MonsterKilled()
    {
        _monstersKilled++;

        if (_monstersKilled >= _MAX_ENEMIES)
        {
            UnlockAchievement(Achievement.WEED_WHACKER);
        }
    }

    /// <summary>
    /// Checks to see if 0 monsters have been killed
    /// </summary>
    public void CheckForNoKilledMonsters()
    {
        if (_monstersKilled == 0)
        {
            UnlockAchievement(Achievement.TREE_HUGGER);
        }
    }

    /// <summary>
    /// Checks to see if no resources have been used in the game
    /// </summary>
    public void CheckForNoResources()
    {
        if (!_hasUsedResource)
        {
            UnlockAchievement(Achievement.MINIMALIST);
        }
    }

    /// <summary>
    /// Unlocks the achievement for completing the credits
    /// </summary>
    public void CompleteCredits()
    {
        UnlockAchievement(Achievement.YOU_DO_CARE);
    }

    /// <summary>
    /// Unlocks an achievement that requires something being shot
    /// </summary>
    /// <param name="type"> The achievement in question to unlock </param>
    /// <param name="obj"> The GameObject shot for tracking </param>
    public void AchievementShot(ShootingAchievement.ShootingAchievementType type, int id)
    {
        switch (type)
        {
            case ShootingAchievement.ShootingAchievementType.HEADSHOT:
                StatueShot(id);
                break;
            case ShootingAchievement.ShootingAchievementType.DONT_BREAK_IT:
                GeneratorShot();
                break;
            case ShootingAchievement.ShootingAchievementType.FISH:
                BullseyeShot();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Tracks the number of statues shot and unlocks the respective achievement when a certain number is reached
    /// </summary>
    /// <param name="obj"> The GameObject shot for tracking </param>
    private void StatueShot(int id)
    {
        _statuesShot.Add(id);

        if (_statuesShot.Count >= _MAX_STATUES)
        {
            UnlockAchievement(Achievement.HEADSHOT);
        }
    }

    /// <summary>
    /// Unlocks the achievement for shooting the generator
    /// </summary>
    private void GeneratorShot()
    {
        UnlockAchievement(Achievement.DONT_BREAK_IT);
    }

    /// <summary>
    /// Unlocks the achievement for shooting the practice bullseye
    /// </summary>
    private void BullseyeShot()
    {
        UnlockAchievement(Achievement.FISH);
    }

    /// <summary>
    /// Resets the achievement data attached to this script and saves it
    /// </summary>
    public void ResetAcheivementData()
    {
        _foundNotes.Clear();
        _monstersKilled = 0;
        _statuesShot.Clear();
        _hasUsedResource = false;

        SaveAchievementData();
    }

    /// <summary>
    /// Saves the achievement data to the GameSaveData
    /// </summary>
    public void SaveAchievementData()
    {
        GameSaveData data = SaveManager.Instance.GetGameSaveData();

        data.FoundNotes = _foundNotes;
        data.MonstersKilled = _monstersKilled;
        data.StatuesShot = _statuesShot;
        data.HasUsedResource = _hasUsedResource;
    }

    /// <summary>
    /// Loads achievement data from the GameSaveData
    /// </summary>
    public void LoadAchievementData()
    {
        GameSaveData data = SaveManager.Instance.GetGameSaveData();

        _foundNotes = data.FoundNotes;
        _monstersKilled = data.MonstersKilled;
        _statuesShot = data.StatuesShot;
        _hasUsedResource = data.HasUsedResource;
    }
}
