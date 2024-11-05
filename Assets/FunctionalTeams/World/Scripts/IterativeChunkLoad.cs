/******************************************************************************
// File Name:       IterativeChunkLoad.cs
// Author:          Nick Rice, Alex Kalscheur
// Contributor:     Charlie Polonus
// Creation Date:   September 27, 2024
//
// Description:     This script takes in the list of all the chunks and the queue of chunks,
//                  then goes through the queue and enables + disables the necessary and unnecessary
//                  chunks
******************************************************************************/

using UnityEngine;

/// <summary>
/// This script takes in the list of all the chunks and the queue of chunks, 
/// then goes through the queue and enables + disables the necessary and unnecessary chunks
/// </summary>
public class IterativeChunkLoad : MonoBehaviour
{
    [Tooltip("The array with every chunk")]
    [SerializeField]private GameObject[] _everyChunk;

    [Tooltip("The queue of in-scene chunks")]
    private readonly GameObject[] _usedChunks = new GameObject[4];

    #region Pointers
    [Tooltip("Selects the next chunk in the queue")]
    private int _chunkQueuePtrPtr;

    [Tooltip("Queue of pointers for chunk order")]
    private int[] _chunkQueuePtr = new int[0];

    // Pointers for where the chunks are in the every chunk array
    // This is necessary to put chunks Back into the unused chunk landing area
    private int _newFrontChunkPtr;
    private int _frontChunkPtr;
    private int _middleChunkPtr;
    private int _backChunkPtr;

    [Tooltip("Keeps track of which chunk get removed next")]
    private enum ChunkStates
    {
        NewFront, Front, Middle, Back
    }
    
    #endregion

    [Tooltip("The starting position for placing unused chunks")] // Currently just 0,0,0
    private readonly Vector3 _unusedChunkLandingArea = Vector3.zero;

    [Tooltip("Size of chunks. Used to for placing new chunk properly distanced.")]
    [SerializeField] private float _distanceBetweenChunks;

    private BoatMover _boatMover;

    // Returns every chunk in the list (used for the editor window)
    public GameObject[] EveryChunk => _everyChunk;

    // Returns the chunk size(used for the editor window)
    public float DistanceBetweenChunks => _distanceBetweenChunks;

    #region GrabbingChunksFromOtherScripts
    
    /// <summary>
    /// Make sure the enviro manager is in the scene for testing
    /// ~ Gameplay manager and universal manager
    /// </summary>
    private void Awake()
    {
        EnvironmentManager.Instance.GetOnSendingOverChunks().AddListener(ReceiveChunkQueue);
        EnvironmentManager.Instance.GetOnSendAllChunkObjects().AddListener(ReceiveEveryChunk); // STILL NEEDS TO BE IMPLEMENTED
        EnvironmentManager.Instance.GetOnChangeTheChunk().AddListener(ChunkChange);

        _boatMover = GetComponent<BoatMover>();

        // THIS SHOULD BE CHANGED TO REPRESENT THE FIRST CHUNKS IN THE QUEUE INSTEAD OF EVERY CHUNK
        _usedChunks[(int)ChunkStates.Back] = _everyChunk[_chunkQueuePtrPtr++];
        _usedChunks[(int)ChunkStates.Middle] = _everyChunk[_chunkQueuePtrPtr++];
        _boatMover.SetCurrentSpline(_usedChunks[(int)ChunkStates.Middle].GetComponentInChildren<BezierCurve>());
        _usedChunks[(int)ChunkStates.Front] = _everyChunk[_chunkQueuePtrPtr++];
        _boatMover.SetNextSpline(_usedChunks[(int)ChunkStates.Front].GetComponentInChildren<BezierCurve>());
    }

    /// <summary>
    /// Grabs the queue of chunks from an event
    /// </summary>
    /// <param name="chunkQueued">The queue of chunks</param>
    private void ReceiveChunkQueue(int[] chunkQueued)
    {
        _chunkQueuePtr = chunkQueued;
    }

    /// <summary>
    /// Grabs all the usable chunks from an event
    /// </summary>
    /// <param name="theChunks">All usable chunks</param>
    private void ReceiveEveryChunk(GameObject[] theChunks)
    {
        _everyChunk = theChunks; // TO DO
    }

    /// <summary>
    /// Turns off the event listeners. PREVENTS MEMORY LEAKS
    /// </summary>
    private void OnDisable()
    {
        EnvironmentManager.Instance.GetOnSendingOverChunks().RemoveListener(ReceiveChunkQueue);
        EnvironmentManager.Instance.GetOnSendAllChunkObjects().RemoveListener(ReceiveEveryChunk);
        EnvironmentManager.Instance.GetOnChangeTheChunk().RemoveListener(ChunkChange);
    }

    /// <summary>
    /// Sets the distance between the chunks
    /// </summary>
    /// <param name="distance">The distance to set</param>
    public void SetChunkDistance(float distance)
    {
        _distanceBetweenChunks = distance;
    }
    
    #endregion

    /// <summary>
    /// Adds a chunk and removes chunks
    /// </summary>
    private void ChunkChange()
    {
        if (_chunkQueuePtr.Length == 0)
        {
            return;
        }

        AddChunk();
        
        SendChunksAway();
        SwapChunkStates();
    }

    #region FunctionsForChunkChange
    
    /// <summary>
    /// Get the Front chunk pointer 
    /// Tick's up the pointer to the next queue item for the next usage
    /// Moves the chunk to the Front
    /// Set's the front-most chunk active
    /// Adds chunk to the in scene chunk array
    /// </summary>
    private void AddChunk()
    {
        _newFrontChunkPtr = _chunkQueuePtr[_chunkQueuePtrPtr];

        _chunkQueuePtrPtr = (_chunkQueuePtrPtr + 1) % _chunkQueuePtr.Length;

        _everyChunk[_newFrontChunkPtr].transform.position =
            _usedChunks[(int)ChunkStates.Front].transform.position + new Vector3(/*DistanceBetweenChunks*/0, 0, /*0*/_distanceBetweenChunks);

        _everyChunk[_newFrontChunkPtr].SetActive(true);

        _usedChunks[(int)ChunkStates.NewFront] = _everyChunk[_newFrontChunkPtr];
    }

    /// <summary>
    /// Disables the Back chunk
    /// And teleports it away to the unused chunk area
    /// </summary>
    private void SendChunksAway()
    {
        _usedChunks[(int)ChunkStates.Back].SetActive(false); // If causing errors, put at the bottom of the function

        _usedChunks[(int)ChunkStates.Back].transform.position =
            _unusedChunkLandingArea + new Vector3(_distanceBetweenChunks * _backChunkPtr, 0, 0);
    }

    /// <summary>
    /// Swaps all the chunks to be in the new order they are in
    /// This ordering prevents any loss of data
    /// </summary>
    private void SwapChunkStates()
    {
        _usedChunks[(int)ChunkStates.Back] = _usedChunks[(int)ChunkStates.Middle];
        _backChunkPtr = _middleChunkPtr;
        _usedChunks[(int)ChunkStates.Middle] = _usedChunks[(int)ChunkStates.Front];
        _boatMover.SetCurrentSpline(_usedChunks[(int)ChunkStates.Middle].GetComponentInChildren<BezierCurve>());

        _middleChunkPtr = _frontChunkPtr;
        _usedChunks[(int)ChunkStates.Front] = _usedChunks[(int)ChunkStates.NewFront];
        _boatMover.SetNextSpline(_usedChunks[(int)ChunkStates.Front].GetComponentInChildren<BezierCurve>());
        _frontChunkPtr = _newFrontChunkPtr;
        _newFrontChunkPtr = 0; // This ptr is freed until a new chunk is going to be added
    }
    
    #endregion
}


