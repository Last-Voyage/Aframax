/*****************************************************************************
// File Name :         HarpoonGun.cs
// Author :            Tommy Roberts
// Creation Date :     9/22/2024
//
// Brief Description : Controls the basic shoot harpoon and retract functionality.
*****************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HarpoonGun : MonoBehaviour
{
    #region Inspector Variables
    [Header("Harpoon Variables")]
    [SerializeField] private GameObject _harpoonPrefab; // Prefab of the harpoon
    [Tooltip("The speed the harpoon moves in the launch direction")]
    [SerializeField] private float _harpoonSpeed = 50f; // Speed of the harpoon
    [Tooltip("max distance the harpoon can launch")]
    [SerializeField] private float _maxDistance = 100f; // Max travel distance
    [Tooltip("the max duration the harpoon can take to reel in(at max distance)")]
    [SerializeField] private float _reelDuration = 3f; // Time to reel in at max distance
    [Tooltip("cooldown of the gun after fully reeled in")]
    [SerializeField] private float _gunCooldown = 2f; // cd of harpoon gun after fully retracted
    [Tooltip("if true then you have to hold mouse down to retract fully. if false retracts automatically")]
    [SerializeField] private bool _holdToRetractMode = true; // turns on or off having to hold mouse down to retract

    [Header("Harpoon Functionality Dependencies")]
    [Tooltip("Transform of whatever the cameras rotation is. Probably the cinemachine camera object")]
    [SerializeField] private Transform _playerLookDirection;
    [Tooltip("name of shoot animation")]
    [SerializeField] private string _harpoonShootTrigger = "shoot";
    [Tooltip("name of retract animation")]
    [SerializeField] private string _harpoonRetractTrigger = "drawBack";
    [Tooltip("Transform on the end of the harpoon gun (whereever the harpoon comes out of)")]
    public Transform HarpoonTip;

    [Tooltip("The harpoon object on the gun. Disappears and reappears to indicate whether player has shot ready")]
    [SerializeField] private GameObject _harpoonOnGun;
    [Tooltip("Layers the launched harpoon can not hit")]
    [SerializeField] private LayerMask _excludeLayers;
    [SerializeField] private Animator _harpoonAnimator;
    [SerializeField] private InputActionReference _harpoonShoot;
    private HarpoonRope _harpoonRope;

    [Header("Camera Shake Values")]
    [Tooltip("Recoil Intensity Shake")]
    [SerializeField] private float _recoilCameraShakeIntensity = 5f;
    [Tooltip("Recoil time Shake")]
    [SerializeField] private float _recoilCameraShakeTime = 0.05f;
    [Tooltip("Retract Intensity Shake")]
    [SerializeField] private float _retractCameraShakeIntensity = 3f;
    [Tooltip("Retract time Shake")]
    [SerializeField] private float _retractCameraShakeTime = .05f;
    #endregion

    public GameObject HarpoonSpear {get; private set;}
    public static event Action<HarpoonGun> OnShotEvent;
    public static event Action<HarpoonGun> OnRetractEvent;
    private bool _isReeling = false;
    private Vector3 _fireDir;
    private float _currentDist;
    public bool IsShooting {get; private set; }
    private RaycastHit _hit;
    private float _currentReelDur;

    private void Awake(){
        _harpoonRope = GetComponent<HarpoonRope>();
    }

    /// <summary>
    /// sets up the button for shooting
    /// </summary>
    private void OnEnable() 
    {
        _harpoonShoot.action.performed += FireHarpoon;
    }

    /// <summary>
    /// disables shoot button
    /// </summary>
    private void OnDisable() 
    {
        _harpoonShoot.action.performed -= FireHarpoon;
    }

    /// <summary>
    /// creates a harpoon, sets up the fire direction and everything else to begin the launch
    /// </summary>
    private void FireHarpoon(InputAction.CallbackContext context)
    {
        //dont shoot if already shot
        if(HarpoonSpear != null || _isReeling)
        {
            return;
        }
        // Instantiate the harpoon and set its initial position and direction
        HarpoonSpear = Instantiate(_harpoonPrefab, _playerLookDirection.position, Quaternion.identity);
        HarpoonSpear.SetActive(false);
        HarpoonSpear.transform.GetChild(0).rotation = _playerLookDirection.rotation;
        IsShooting = true;
        _fireDir = _playerLookDirection.forward; // In the direction the player is looking
        // Start moving the harpoon
        StartCoroutine(MoveHarpoon());
        _harpoonOnGun.SetActive(false);
        _harpoonAnimator.SetTrigger(_harpoonShootTrigger);
        CinemachineShake.Instance.ShakeCamera(_recoilCameraShakeIntensity, _recoilCameraShakeTime);
        OnShotEvent(this);
    }

    private void SetHarpoonActive()
    {
        //delays visual of harpoon appearing for better appearance
        HarpoonSpear.SetActive(true);
    }

    /// <summary>
    /// coroutine to move the created harpoon to the target direction. starts the reel coroutine at the end
    /// </summary>
    private IEnumerator MoveHarpoon()
    {
        Invoke(nameof(SetHarpoonActive), .15f);
        _currentDist = 0f;
        while (_currentDist < _maxDistance && !_isReeling)
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = _fireDir * _harpoonSpeed * Time.deltaTime;
            // Cast a ray from the harpoon's current position forward by the amount it moves this frame
            if (Physics.Raycast(HarpoonSpear.transform.position, movement, out _hit, 1f, ~_excludeLayers))
            {
                //if hit grabbable object then child object to harpoon to bring object back with harpoon
                if(_hit.transform.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
                {
                    _hit.transform.SetParent(HarpoonSpear.transform);
                    Rigidbody hitRB = _hit.transform.GetComponent<Rigidbody>();
                    if(hitRB != null)
                    {
                        hitRB.isKinematic = true;
                    }
                }
                // Harpoon _hit something, stop its movement and start reeling it in
                HarpoonSpear.transform.position = _hit.point; // Snap the harpoon to the _hit point
                StartReeling(_hit.point);
                yield break; // Exit the coroutine, no need to keep moving the harpoon
            }

            // If no collision, move the harpoon
            HarpoonSpear.transform.Translate(movement);
            _currentDist += movement.magnitude;

            yield return null;
        }
        // If max distance reached and no _hit, start reeling back
        if (!_isReeling)
        {
            StartReeling(_hit.point);
        }
    }

    /// <summary>
    /// sets up the reel positions and time. if the target hit is grabbable child it to the harpoon so it comes back
    /// </summary>
    /// <param name="_hitPosition"></param>
    private void StartReeling(Vector3 _hitPosition)
    {
        _isReeling = true;
        
        float distanceFromPlayer;
        if(_hit.transform != null)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, _hitPosition);
        }else
        {
            distanceFromPlayer = _maxDistance;
        }
        // Adjust the reeling time based on the distance
        _currentReelDur = distanceFromPlayer / _maxDistance * _reelDuration;
        StartCoroutine(ReelHarpoon());
    }

    /// <summary>
    /// actually reels in the harpoon and destorys the instaniated harpoon. Begins reload timer at the end.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReelHarpoon()
    {
        yield return new WaitForSeconds(.05f);
        float elapsedTime = 0;
        if(!_holdToRetractMode)
        {
            _harpoonAnimator.SetTrigger(_harpoonRetractTrigger);
            //cause the wave action again when reeling
            OnRetractEvent(this);
            OnShotEvent(this);
        }
        // Lerp the harpoon back to the player over time
        var startPos = HarpoonSpear.transform.position;
        bool startedRetracting = false;
        while(elapsedTime < _currentReelDur)
        {
            //if hold to retract is on, only pull in harpoon if holding down button
            if(_holdToRetractMode)
            {
                if(_harpoonShoot.action.inProgress)
                {
                    if(!startedRetracting)
                    {
                        startedRetracting = true;
                        _harpoonAnimator.SetTrigger(_harpoonRetractTrigger);
                        //cause the wave action again when reeling
                        OnRetractEvent(this);
                        OnShotEvent(this);
                    }
                    IsShooting = true;
                    HarpoonSpear.transform.GetChild(0).LookAt(HarpoonTip);
                    HarpoonSpear.transform.position = Vector3.Lerp(startPos, HarpoonTip.position, elapsedTime/ _currentReelDur);
                    elapsedTime+= Time.deltaTime;
                } 
                //otherwise automatically pull in 
            }
            else
            {
                HarpoonSpear.transform.GetChild(0).LookAt(HarpoonTip);
                HarpoonSpear.transform.position = Vector3.Lerp(startPos, HarpoonTip.position, elapsedTime/ _currentReelDur);
                elapsedTime+= Time.deltaTime; 
            }
            yield return null;
        }
        // get rid of harpoon
        if(_hit.transform != null)
        {
            _hit.transform.SetParent(null);
            if(_hit.transform.GetComponent<Rigidbody>() != null)
            {
                _hit.transform.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        CinemachineShake.Instance.ShakeCamera(_retractCameraShakeIntensity, _retractCameraShakeTime);  
        StartCoroutine(ResetHarpoon());
        OnRetractEvent(this);
    }

    /// <summary>
    /// Resets the harpoon and "reloads" it
    /// </summary>
    private IEnumerator ResetHarpoon()
    {
        _hit = new RaycastHit();
        Destroy(HarpoonSpear);
        IsShooting = false;
        yield return new WaitForSeconds(_gunCooldown);
        _isReeling = false;
        _harpoonOnGun.SetActive(true);
    }
}
