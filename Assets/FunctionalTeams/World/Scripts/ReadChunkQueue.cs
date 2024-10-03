/******************************************************************************
// File Name:       ReadChunkQueue.cs
// Author:          Nick Rice & Charlie Polonus
// Creation Date:   September 29, 2024
//
// Description:     This script takes the .txt file with the order of all of the chunks, 
//                  and converts it into an array of ints, and then sends that over to
//                  the script that needs it through an event.
******************************************************************************/
using UnityEngine;
using System.IO;

/// <summary>
/// This script takes the .txt file with the order of all of the chunks, 
/// and converts it into an array of ints, and then sends that over to
/// the script that needs it through an event.
/// </summary>
public class ReadChunkQueue : MonoBehaviour
{
    const string _THE_LINE_OF_CHUNKS = "/ChunkQueue.txt";

    int[] _realChunks;

    private void OnEnable()
    {
        GetChunkQueue();
    }

    /// <summary>
    /// Grabs an array full of strings and sends it to a string to int function
    /// </summary>
    private void GetChunkQueue()
    {
        string[] allChunkers = File.ReadAllLines(Application.streamingAssetsPath + _THE_LINE_OF_CHUNKS);
        TurnQueueToInt(allChunkers);
    }

    /// <summary>
    /// Turns the array of strings into an array of ints, and then sends it off to the actual chunk loading script
    /// </summary>
    /// <param name="stringChunkers"></param>
    private void TurnQueueToInt(string[] stringChunkers)
    {
        string realStringOfChunk = stringChunkers[0];
        string[] CHUNKED = realStringOfChunk.Split(",");
        _realChunks = new int[CHUNKED.Length];

        for(int i = 0; i < CHUNKED.Length; i++)
        {
            _realChunks[i] = int.Parse(CHUNKED[i]);
        }

        EnvironmentManager.Instance.SendOutChunks( _realChunks );
    }
}
