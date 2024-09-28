using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterativeChunkLoad : MonoBehaviour
{
    public List<GameObject> EveryChunk = new List<GameObject>();

    private GameObject[] _usedChunks = new GameObject[4];

    // This will be a reference to the object that will be used to determine if the player is far enough away
    // from the back chunk (where they can't see it anymore), so that the chunk swapping
    // process can begin
    private GameObject _chunkSwapObj;


    #region Pointers
    [Tooltip("This variable will be used as a pointer for selecting the newFront chunk")]
    private int _whateverChunkYouWantPtr = 0;

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

    [Tooltip("The starting position for placing each of the unused chunks")]
    private Vector3 _unusedChunkLandingArea = Vector3.zero;

    [Tooltip("The size of each chunk, that will be used to calculate the midpoint to midpoint of chunks")]
    private const float DistanceBetweenChunks = 5f;

    private void ChunkChange()
    {
        _newFrontChunkPtr = _whateverChunkYouWantPtr;

        EveryChunk[_whateverChunkYouWantPtr].transform.position += 
            _usedChunks[(int)ChunkStates.front].transform.position + new Vector3(DistanceBetweenChunks,0,0);

        EveryChunk[_whateverChunkYouWantPtr].SetActive(true);

        _usedChunks[(int)ChunkStates.newFront] = EveryChunk[_whateverChunkYouWantPtr];

        // PLACE HOLDER LINE - Put a function here that would cleanup anything that happened in the back chunk

        _usedChunks[(int)ChunkStates.back].SetActive(false);

        SendChunksAway();
        SwapChunkStates();
    }

    /// <summary>
    /// This will take the, about to no longer be, back chunk
    /// And teleport it to a position that is based on where it is in the array/list
    /// It takes the chunk's array/list position and multiplies it by the distance between chunks, meaning,
    /// all of them will be properly spaced out
    /// It also bases that off of the unused chunk landing area
    /// (Will this be needed?) - idk
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == _chunkSwapObj)
        {
            // Get a variable from the collision.gameObjects that will say what the newest loaded chunk will be
            ChunkChange();
        }
    }
}
