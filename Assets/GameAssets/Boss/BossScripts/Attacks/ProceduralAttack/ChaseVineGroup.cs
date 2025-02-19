/*****************************************************************************
// File Name :         ChaseVineGroup.cs
// Author :            Tommy Roberts
// Creation Date :     2/19/2025
//
// Brief Description : This script controls the chase vine group
*****************************************************************************/
using UnityEngine;

/// <summary>
/// This class controls the group of chasing vines
/// </summary>
public class ChaseVineGroup : MonoBehaviour
{
    [Header("Chase settings")]
    [SerializeField] private float _chaseSpeed = 3f;
    [SerializeField] private Transform _chaseCollider;
    [SerializeField] private bool _isTriggeredByPlayerWalkingBy = true;
    [SerializeField] private bool _killPlayerInstantly = true;
    [SerializeField] private float _damageToPlayer = 50f;
    private ChaseSequenceVine[] _chaseSequenceVines;
    private Transform _colliderFollow;

    [Header("Camera Shake and what not")]
    [SerializeField] private bool _shakeCameraOnStart = true;
    [SerializeField] private float _startCameraShakeIntensity = 1f;
    [SerializeField] private float _startCameraShakeTime = .5f;
    
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
    }

    /// <summary>
    /// Makes collider follow the vines when appropriate
    /// </summary>
    private void Update()
    {
        if(_colliderFollow != null)
            _chaseCollider.position = _colliderFollow.position;
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
    }

    public bool IsTriggeredByPlayerWalkThrough() => _isTriggeredByPlayerWalkingBy;
    public bool IsSupposedToKillInstant() => _killPlayerInstantly;
    public float GetPlayerDamageAmount() => _damageToPlayer;
}
