/*****************************************************************************
// Name :           StoryBeat.cs
// Author :         Charlie Polonus
// Contributer :    Ryan Swanson
// Created :        11/6/2024
// Description :    Handles the story beats
*****************************************************************************/

using System.Collections.Generic;
/// <summary>
/// An individual story beat
/// </summary>
[System.Serializable]
public class StoryBeat
{
    // The name and description of the story beat
    public string BeatName;
    public string BeatDescription;

    // Whether or not the beat should trigger as soon as the game starts
    public bool TriggerOnStart;

    // Whether or not the beat is outdated
    public bool Outdated = false;

    // The list of story beat events in the beat
    public List<StoryBeatEvent> StoryBeatEvents;
}
