using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterativeChunkLoad : MonoBehaviour
{
    // The entire pool of chunks that can be used
    public List<GameObject> EveryChunk = new List<GameObject>();

    // The current queue of in scene chunks
    private GameObject[] _usedChunks = new GameObject[4];

    #region Pointers
    [Tooltip("This pointer will select the next chunk in the queue")]
    private int _chunkQueuePtrPtr = 0;

    // I need to grab the READCHUNKQUEUE
    [Tooltip("The pointer for the order of chunks, taken from the text document")]
    private int[] _chunkQueuePtr;

    // Pointers for where the chunks are in the every chunk array
    // This is necessary to put chunks back into the unused chunk landing area
    private int _newFrontChunkPtr;
    private int _frontChunkPtr;
    private int _middleChunkPtr;
    private int _backChunkPtr;

    [Tooltip("A pointer to keep track of which chunk get's removed next")]
    private enum ChunkStates
    {
        newFront, front, middle, back
    }
    #endregion

    [Tooltip("The starting position for placing each of the unused chunks")] // Currently just 0,0,0
    private Vector3 _unusedChunkLandingArea = Vector3.zero;

    [Tooltip("The size of each chunk. Used to calculate the midpoint to midpoint of chunks")]
    private const float DistanceBetweenChunks = 10f;

    private void Start()
    {
        // Make sure the enviro manager is in the scene for testing
        // With gameplay manager and universal manager
        EnvironmentManager.Instance.GetSendingOverChunks().AddListener(ReceiveChunkQueue);
    }

    private void ReceiveChunkQueue(int[] chunkqueued)
    {
        _chunkQueuePtr = chunkqueued;
    }

    /// <summary>
    /// I should probably make this an event listener!
    /// That way, I don't have to keep track of other nonsense!
    /// </summary>
    private void ChunkChange()
    {
        AddChunk();

        // PLACE HOLDER LINE - Put a function here that would cleanup anything that happened in the back chunk

        _usedChunks[(int)ChunkStates.back].SetActive(false);

        SendChunksAway();
        SwapChunkStates();
    }

    /// <summary>
    /// This will get a new front chunk pointer and it will tick up the pointer to the next queue item
    /// It will take the chunk in queue and move it to the front of the chunks
    /// It will then set the frontmost chunk active
    /// Then it will add that chunk to the array that keeps track of where the chunks are
    /// </summary>
    private void AddChunk()
    {
        _newFrontChunkPtr = _chunkQueuePtr[_chunkQueuePtrPtr];

        _chunkQueuePtrPtr++;

        EveryChunk[_newFrontChunkPtr].transform.position +=
            _usedChunks[(int)ChunkStates.front].transform.position + new Vector3(DistanceBetweenChunks, 0, 0);

        EveryChunk[_newFrontChunkPtr].SetActive(true);

        _usedChunks[(int)ChunkStates.newFront] = EveryChunk[_newFrontChunkPtr];
    }

    /// <summary>
    /// This will take the, about to no longer be, back chunk
    /// And teleport it to a position that is based on where it is in the array/list
    /// It takes the chunk's array/list position and multiplies it by the distance between chunks, meaning,
    /// all of them will be properly spaced out
    /// It also bases that off of the unused chunk landing area
    /// </summary>
    private void SendChunksAway()
    {
        _usedChunks[(int)ChunkStates.back].transform.position =
            _unusedChunkLandingArea + new Vector3(DistanceBetweenChunks * _backChunkPtr, 0, 0);
    }

    /// <summary>
    /// Swaps all of the chunks to be in the new order they are in
    /// The newFront becomes (the real) front, (the old) front becomes middle, and middle becomes back
    /// It's not in that order for the sake of not removing any in use chunks
    /// </summary>
    private void SwapChunkStates()
    {
        _usedChunks[(int)ChunkStates.back] = _usedChunks[(int)ChunkStates.middle];
        _backChunkPtr = _middleChunkPtr;
        _usedChunks[(int)ChunkStates.middle] = _usedChunks[(int)ChunkStates.front];
        _middleChunkPtr = _frontChunkPtr;
        _usedChunks[(int)ChunkStates.front] = _usedChunks[(int)ChunkStates.newFront];
        _frontChunkPtr = _newFrontChunkPtr;
        _newFrontChunkPtr = 0; // This ptr is freed until a new chunk is going to be added
    }



    private void OnDisable()
    {
        EnvironmentManager.Instance.GetSendingOverChunks().RemoveListener(ReceiveChunkQueue);
    }
}


