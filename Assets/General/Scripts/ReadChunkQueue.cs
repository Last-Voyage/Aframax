using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class ReadChunkQueue : MonoBehaviour
{
    const string _theLineOfChunks = "/ChunkQueue.txt";

    int[] _realChunks;

    /// <summary>
    /// Need to do!
    /// Array full of strings
    /// Parse the first string, and put each individual string into a new array
    /// Then convert all of those strings into ints, and put all of those numbers into a new array
    /// </summary>
    private void GetChunkQueue()
    {
        string[] allChunkers = File.ReadAllLines(Application.streamingAssetsPath + _theLineOfChunks);
        TurnQueueToInt(allChunkers);
    }

    private void TurnQueueToInt(string[] stringChunkers)
    {
        string realStringOfChunk = stringChunkers[0];
        string[] CHUNKED = realStringOfChunk.Split(",");
        _realChunks = new int[CHUNKED.Length];

        for(int i = 0; i < CHUNKED.Length; i++)
        {
            _realChunks[i] = int.Parse(CHUNKED[i]);
        }

        EnvironmentManager.Instance.GetSendingOverChunks()?.Invoke(_realChunks);
    }
}
