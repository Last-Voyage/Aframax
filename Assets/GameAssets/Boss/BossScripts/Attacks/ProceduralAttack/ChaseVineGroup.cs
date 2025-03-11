/*****************************************************************************
// File Name :         ChaseVineGroup.cs
// Author :            Tommy Roberts
// Contributor:        Ryan Swanson
// Creation Date :     2/19/2025
//
// Brief Description : This script controls the chase vine group
*****************************************************************************/
using UnityEngine;
using FMOD.Studio;

/// <summary>
/// This class controls the group of chasing vines
/// </summary>
public class ChaseVineGroup : MonoBehaviour
{
    [Header("Chase settings")]
    [SerializeField] private float _chaseSpeed = 3f;
    [SerializeField] private Transform _chaseCollider;
    [SerializeField] private bool _isTriggeredByPlayerWalkingBy = true;
    [SerializeField] private bool _doeskillPlayerInstantly = true;
    [SerializeField] private float _damageToPlayer = 50f;
    private ChaseSequenceVine[] _chaseSequenceVines;
    private Transform _colliderFollow;
    private bool _isFirstChase = true;

    [Header("Camera Shake and what not")]
    [SerializeField] private bool _shakeCameraOnStart = true;
    [SerializeField] private float _startCameraShakeIntensity = 1f;
    [SerializeField] private float _startCameraShakeTime = .5f;

    [Header("Audio")]
    [SerializeField] private bool _doesPlayStartAudioEveryChase;
    [Tooltip("The instance of the audio that is started for the looping audio")]
    private EventInstance _chaseEventInstance;

    /// <summary>
    /// Sets up the chase vine group
    /// </summary>
    private void Start()
    {
        _chaseSequenceVines = GetComponentsInChildren<ChaseSequenceVine>();
        foreach(ChaseSequenceVine chaseSequenceVine in _chaseSequenceVines)
        {
            chaseSequenceVine.gameObject.SetActive(false);
        }
        _chaseSequenceVines[0].OnChaseEnd.AddListener(StopMovementAudio);

        CreateMovementAudio();
    }

    /// <summary>
    /// Makes collider follow the vines when appropriate
    /// </summary>
    private void Update()
    {
        if(_colliderFollow != null)
        {
            _chaseCollider.position = _colliderFollow.position;
        }   
    }

    /// <summary>
    /// Activates all the vines and starts them moving toward end of path
    /// </summary>
    public void ActivateThisGroupOfVines()
    {
        //this transform should be the joint which is leading the vine toward its destination
        _chaseCollider.gameObject.SetActive(true);
        foreach(ChaseSequenceVine chaseSequenceVine in _chaseSequenceVines)
        {
            chaseSequenceVine.gameObject.SetActive(true);
            chaseSequenceVine.ActivateChase(_chaseSpeed);
        }
        _colliderFollow = _chaseSequenceVines[0].transform.GetChild(0);


        if(_shakeCameraOnStart)
        {
            CinemachineShake.Instance.ShakeCamera(_startCameraShakeIntensity, _startCameraShakeTime, true);
        }

        StartMovementAudio();

        if (_isFirstChase)
        {
            _isFirstChase = false;
        }
    }

    #region Audio
    /// <summary>
    /// Creates the initial instance of the movement audio
    /// </summary>
    private void CreateMovementAudio()
    {
        //return statement added so as not to throw a thousand nulls in logs
        if (RuntimeSfxManager.Instance == null || FmodSfxEvents.Instance == null)
        {
            return;
        }

        _chaseEventInstance = RuntimeSfxManager.Instance.
            CreateInstanceFromReference(FmodSfxEvents.Instance.ChaseSequenceLoop, _chaseSequenceVines[0].Head.gameObject);
    }

    /// <summary>
    /// Starts playing the movement audio
    /// </summary>
    private void StartMovementAudio()
    {
        if(!_doesPlayStartAudioEveryChase && !_isFirstChase)
        {
            //Play the start audio
            RuntimeSfxManager.APlayOneShotSfx?
                .Invoke(FmodSfxEvents.Instance.ChaseSequenceStart, _chaseSequenceVines[0].Head.transform.position);
        }

        if (!_chaseEventInstance.isValid())
        {
            return;
        }

        //Play the looping audio
        RuntimeSfxManager.Instance.FadeInLoopingOneShot(_chaseEventInstance, FmodSfxEvents.Instance.ChaseLoopFadeInTime);
    }

    /// <summary>
    /// Stops playing the movement audio
    /// </summary>
    private void StopMovementAudio()
    {
        if (!_chaseEventInstance.isValid())
        {
            return;
        }
        RuntimeSfxManager.Instance.FadeOutLoopingOneShot(_chaseEventInstance, FmodSfxEvents.Instance.ChaseLoopFadeOutTime);
    }
    #endregion

    public bool IsTriggeredByPlayerWalkThrough() => _isTriggeredByPlayerWalkingBy;
    public bool IsSupposedToKillInstant() => _doeskillPlayerInstantly;
    public float GetPlayerDamageAmount() => _damageToPlayer;
}
