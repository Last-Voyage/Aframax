/******************************************************************************
// File Name:       SaveManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Contains the functionality to set up and get access to save data
******************************************************************************/

using UnityEngine;
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// Provides the system by which the saving is set up and
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
        //Checks to see if we're in editor, if so use data path instead of persistent data path
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
    /// Fills the save data with its initial values when the file is first created as needed
    /// You could use this to populate a dictionary on start up for example
    /// For simpler variable types just set them in the Game Save Data class
    /// </summary>
    private void StartingValues()
    {
        
    }

    /// <summary>
    /// Writes all variables in the Game Save Data class into Json
    /// </summary>
    private void SaveText()
    {
        //Converts the Game Save Data class into a string
        var convertedJson = JsonConvert.SerializeObject(_gameSaveData);
        //Saves the string into the text file
        File.WriteAllText(_path + "Data.json", convertedJson);
    }

    /// <summary>
    /// Loads the data from a file
    /// </summary>
    private void Load()
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

    /// <summary>
    /// Resets all vars in the saved data
    /// </summary>
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
    /// <summary>
    /// Establishes the instance for the save manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Sets up the main manager by establishing the path to the Json file and loading the data
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        EstablishPath();
        Load();
    }
    #endregion

    #region Getters
    public GameSaveData GetGameSaveData() => _gameSaveData;
    #endregion
}

/// <summary>
/// Holds the data which is being saved
/// Save data is read from text file and stored into Game Save Data
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public bool TempSaveBool;

    #region Getters
    
    public bool GetTempBool() => TempSaveBool;
    
    #endregion

    #region Setters
    
    public void SetTempBool(bool newTemp)
    {
        TempSaveBool = newTemp;
    }
    
    #endregion
}