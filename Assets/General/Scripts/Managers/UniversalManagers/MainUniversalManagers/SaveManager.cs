/******************************************************************************
// File Name:       SaveManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Contains the functionality to setup and get access to save data
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// Provides the system by which the saving is setup and
/// </summary>
public class SaveManager : MainUniversalManagerFramework
{
    //Game Save Data variable MUST viewable in editor for Json, so either public or serialized
    [SerializeField] private GameSaveData _gameSaveData;
    private string _path;

    public static SaveManager Instance;

    /// <summary>
    /// Sets the path to create the save file
    /// </summary>
    private void EstablishPath()
    {
        //Checks to see if we're in editor, if so use datapath instead of persistent datapath
        //Needed so that saving works both in editor and build
        _path = Application.isEditor ? Application.dataPath : Application.persistentDataPath;
        //Append /SaveData/ to said path
        //SaveData is the file is assets containing the save data
        _path = _path + "/SaveData/";
        //Check if we're in a build, check if the directory exists, if not
        if (!Application.isEditor && !Directory.Exists(_path)) 
        {
            //Creates the directory
            Directory.CreateDirectory(_path); 
        }
    }

    /// <summary>
    /// Fills the save data with it's initial values when the file is first created as needed
    /// You could use this to populate a dictionary on start up for example
    /// For simpler variable types just set them in the Game Save Data class
    /// </summary>
    private void StartingValues()
    {
        
    }

    /// <summary>
    /// Writes all variables in the Game Save Data class into Json
    /// </summary>
    public void SaveText()
    {
        //Converts the Game Save Data class into a string
        var convertedJson = JsonConvert.SerializeObject(_gameSaveData);
        //Saves the string into the text file
        File.WriteAllText(_path + "Data.json", convertedJson);
    }

    public void Load()
    {
        //Loads all variables in Json into the Game Save Data class
        if (File.Exists(_path + "Data.json"))
        {
            //Converts the text file into a string
            var json = File.ReadAllText(_path + "Data.json");
            //Converts the string into the Game Save Data class
            _gameSaveData = JsonConvert.DeserializeObject<GameSaveData>(json);
        }
        else
        {
            //Sets the initial values
            StartingValues();
            //Saves the initial values
            SaveText();
        }
    }

    public void ResetSaveData()
    {
        //Fully resets all variables in the Game Save Data
        _gameSaveData = new GameSaveData();

        //Sets the initial values
        StartingValues();

        //Saves the changes into the text file
        SaveText();
    }

    #region BaseManager
    public override void SetupMainManager()
    {
        base.SetupMainManager();
        Instance = this;
        EstablishPath();
        Load();
    }
    #endregion

    #region Getters
    public GameSaveData GetGameSaveData() => _gameSaveData;
    #endregion

    #region Setters

    #endregion
}

/// <summary>
/// Holds the data which is being saved
/// Save data is read from text file and stored into Game Save Data
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public bool tempSaveBool = false;

    #region Getters
    public bool GetTempBool() => tempSaveBool;
    #endregion

    #region Setters
    public void SetTempBool(bool newTemp)
    {
        tempSaveBool = newTemp;
    }
    #endregion
}