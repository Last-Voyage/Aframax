/******************************************************************************
// File Name:       ReadChunkQueue.cs
// Author:          Nick Rice & Charlie Polonus
// Creation Date:   September 29, 2024
//
// Description:     This script takes the .txt file with the order of all the chunks, 
//                  and converts it into an array of ints, and then sends that over to
//                  the script that needs it through an event.
******************************************************************************/
using UnityEngine;
using System.IO;

/// <summary>
/// This script takes the .txt file with the order of all the chunks, 
/// and converts it into an array of ints, and then sends that over to
/// the script that needs it through an event.
/// </summary>
public class ReadChunkQueue : MonoBehaviour
{
    private const string THE_LINE_OF_CHUNKS = "/ChunkQueue.txt";

    private int[] _realChunks;

    private void OnEnable()
    {
        GetChunkQueue();
    }

    /// <summary>
    /// Grabs an array full of strings and sends it to a string to int function
    /// </summary>
    private void GetChunkQueue()
    {
        string[] allChunkers = File.ReadAllLines(Application.streamingAssetsPath + THE_LINE_OF_CHUNKS);
        TurnQueueToInt(allChunkers);
    }

    /// <summary>
    /// Turns the array of strings into an array of ints, and then sends it off to the actual chunk loading script
    /// </summary>
    /// <param name="stringChunkers"> all chunks to parse </param>
    private void TurnQueueToInt(string[] stringChunkers)
    {
        string realStringOfChunk = stringChunkers[0];
        string[] chunked = realStringOfChunk.Split(",");
        _realChunks = new int[chunked.Length];

        for(int i = 0; i < chunked.Length; i++)
        {
            _realChunks[i] = int.Parse(chunked[i]);
        }

        EnvironmentManager.Instance.SendOutChunks( _realChunks );
    }
}
