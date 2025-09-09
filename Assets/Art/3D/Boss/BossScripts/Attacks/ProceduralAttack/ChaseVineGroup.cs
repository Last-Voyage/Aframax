/*****************************************************************************
// File Name :         ChaseVineGroup.cs
// Author :            Tommy Roberts
// Contributors:        Ryan Swanson, Jeremiah Peters
// Creation Date :     2/19/2025
//
// Brief Description : This script controls the chase vine group
*****************************************************************************/
using UnityEngine;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using Cinemachine;

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

    [Header("Start Animation")]
    [SerializeField] private GameObject _startScreamObject;
    [SerializeField] private CinemachineVirtualCamera _startVirtualCamera;
    [SerializeField] private float _lengthOfStartAnimation = 4.5f;
    private CinemachineVirtualCamera _playerCam;
    [SerializeField] private float _delayCameraSwitch = 2f;

    //timing for lining up the cry with when the monster opens its mouth
    [SerializeField] private float _delayCreatureCry = 3.2f;

    private static bool _hasBeenActivated = false;

    /// <summary>
    /// Sets up the chase vine group
    /// </summary>
    private void Start()
    {
        _hasBeenActivated = false;

        _chaseSequenceVines = GetComponentsInChildren<ChaseSequenceVine>();
        foreach(ChaseSequenceVine chaseSequenceVine in _chaseSequenceVines)
        {
            chaseSequenceVine.gameObject.SetActive(false);
        }
        _chaseSequenceVines[0].OnChaseEnd.AddListener(StopMovementAudio);
        PlayerManager.Instance.GetOnPlayerDeath().AddListener(StopMovementAudio);

        CreateMovementAudio();
    }

    /// <summary>
    /// Makes collider follow the vines when appropriate
    /// </summary>
    private void Update()
    {
        if(!_colliderFollow.IsUnityNull())
        {
            _chaseCollider.position = _colliderFollow.position;
        }   
    }

    /// <summary>
    /// Activates all the vines and starts them moving toward end of path
    /// </summary>
    public IEnumerator ActivateThisGroupOfVines()
    {
        if (!_hasBeenActivated)
        {
            //get the player virtual camera
            _playerCam = PlayerCameraController.Instance.PlayerVirtualCamera;

            //disable player camera and enable this camera
            _startVirtualCamera.enabled = true;
            _playerCam.enabled = false;

            // Disable things from other scripts
            CameraManager.Instance.InvokeOnCinematicStart();

            //play start screaming animation
            _startScreamObject.SetActive(true);

            //play scream sfx
            StartCoroutine(PlayScreamSfx());

            //wait until animation is over
            yield return new WaitForSeconds(_lengthOfStartAnimation);
        }

        _hasBeenActivated = true;
        EnemyManager.Instance.InvokeOnChaseSequenceBegin();

        //this transform should be the joint which is leading the vine toward its destination
        _chaseCollider.gameObject.SetActive(true);
        foreach(ChaseSequenceVine chaseSequenceVine in _chaseSequenceVines)
        {
            chaseSequenceVine.gameObject.SetActive(true);
            //release the kraken
            chaseSequenceVine.ActivateChase(_chaseSpeed);
        }
        _colliderFollow = _chaseSequenceVines[0].transform.GetChild(0);


        if(_shakeCameraOnStart)
        {
            CinemachineShake.Instance.ShakeCamera(_startCameraShakeIntensity, _startCameraShakeTime, true);
        }

        yield return new WaitForSeconds(_delayCameraSwitch);

        //after all the vines appear move camera back to player
        _playerCam.enabled = true;
        _startVirtualCamera.enabled = false;

        // Enable things from other scripts
        CameraManager.Instance.InvokeOnCinematicEnd();

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
        if (RuntimeSfxManager.Instance.IsUnityNull() || FmodSfxEvents.Instance.IsUnityNull())
        {
            return;
        }

        _chaseEventInstance = RuntimeSfxManager.Instance.
            CreateInstanceFromReference(FmodSfxEvents.Instance.ChaseSequenceLoop, _chaseSequenceVines[0]._chaseAudioSource.gameObject);
    }

    /// <summary>
    /// plays the creature roar sound effect in time with the animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayScreamSfx()
    {
        yield return new WaitForSeconds(_delayCreatureCry);
        EventInstance ins = AudioManager.Instance.CreateInstanceFromReference(FmodSfxEvents.Instance.CreatureCry);
        ins.set3DAttributes(RuntimeUtils.To3DAttributes(_playerCam.transform));
        ins.setVolume(2f);
        ins.start();
        ins.release();

    }

    /// <summary>
    /// Starts playing the movement audio
    /// </summary>
    private void StartMovementAudio()
    {
        if (!_chaseEventInstance.isValid())
        {
            return;
        }

        //Play the looping audio
        RuntimeSfxManager.Instance.FadeInLoopingOneShot(_chaseEventInstance, FmodSfxEvents.Instance.ChaseLoopFadeInTime);
        RuntimeManager.AttachInstanceToGameObject(_chaseEventInstance, _chaseSequenceVines[0]._chaseAudioSource.gameObject.transform);
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

    /// <summary>
    /// When the script instance is destroyed
    /// </summary>
    private void OnDestroy()
    {
        PlayerManager.Instance.GetOnPlayerDeath().RemoveListener(StopMovementAudio);
    }

    /// <summary>
    /// When the script instance is disabled
    /// </summary>
    private void OnDisable()
    {
        PlayerManager.Instance.GetOnPlayerDeath().RemoveListener(StopMovementAudio);
    }
}
