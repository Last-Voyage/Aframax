/*****************************************************************************
// File Name :         HarpoonGun.cs
// Author :            Tommy Roberts
// Creation Date :     9/22/2024
//
// Brief Description : Controls the basic shoot harpoon and retract functionality.
*****************************************************************************/
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
    [SerializeField] private float _gunCD = 2f; // cd of harpoon gun after fully retracted
    [Tooltip("if true then you have to hold mouse down to retract fully. if false retracts automatically")]
    [SerializeField] private bool _holdToRetractMode = true; // turns on or off having to hold mouse down to retract
    [Header("Harpoon Functionality Dependencies")]
    [SerializeField] private Transform _playerLookDir;
    [SerializeField] private string _harpoonShootTrigger = "shoot";
    [SerializeField] private string _harpoonRetractTrigger = "drawBack";
    public Transform HarpoonTip;
    [SerializeField] private GameObject _harpoonOnGun;
    [Tooltip("Layers the launched harpoon can not hit")]
    [SerializeField] private LayerMask _excludeLayers;
    [SerializeField] private Animator _harpoonAnimator;
    [SerializeField] private InputActionReference _harpoonShoot;
    [SerializeField] private HarpoonRope _harpoonRope;
    [Header("Camera Shake Values")]
    [SerializeField] private float _recoilCameraShakeIntensity = 5f;
    [SerializeField] private float _recoilCameraShakeTime = 0.05f;
    [SerializeField] private float _retractCameraShakeIntensity = 3f;
    [SerializeField] private float _retractCameraShakeTime = .05f;
    

    #endregion

    private GameObject _harpoonSpear;
    public GameObject HarpoonSpear {get { return _harpoonSpear;}}
    private bool _isReeling = false;
    private Vector3 _fireDir;
    private float _currentDist;
    private bool _isShooting = false;
    public bool IsShooting {get { return _isShooting;}}
    private RaycastHit _hit;
    private float _currentReelDur;

    private void OnEnable() {
        _harpoonShoot.action.performed += FireHarpoon;
    }
    private void OnDisable() {
        _harpoonShoot.action.performed -= FireHarpoon;
    }
    /// <summary>
    /// creates a harpoon, sets up the fire direction and everything else to begin the launch
    /// </summary>
    void FireHarpoon(InputAction.CallbackContext context)
    {
        //dont shoot if already shot
        if(_harpoonSpear != null || _isReeling)
        {
            return;
        }
        // Instantiate the harpoon and set its initial position and direction
        _harpoonSpear = Instantiate(_harpoonPrefab, _playerLookDir.position, Quaternion.identity);
        _harpoonSpear.SetActive(false);
        _harpoonSpear.transform.GetChild(0).rotation = _playerLookDir.rotation;
        _isShooting = true;
        _fireDir = _playerLookDir.forward; // In the direction the player is looking

        // Start moving the harpoon
        StartCoroutine(MoveHarpoon());
        _harpoonOnGun.SetActive(false);
        _harpoonAnimator.SetTrigger(_harpoonShootTrigger);
        CinemachineShake.Instance.ShakeCamera(_recoilCameraShakeIntensity, _recoilCameraShakeTime);
        _harpoonRope.StartDrawingRope();
    }
    private void SetHarpoonActive(){
        //delays visual of harpoon appearing for better appearance
        _harpoonSpear.SetActive(true);
    }
    /// <summary>
    /// coroutine to move the created harpoon to the target direction. starts the reel coroutine at the end
    /// </summary>
    IEnumerator MoveHarpoon()
    {
        Invoke(nameof(SetHarpoonActive), .15f);
        
        _currentDist = 0f;

        while (_currentDist < _maxDistance && !_isReeling)
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = _fireDir * _harpoonSpeed * Time.deltaTime;

            // Cast a ray from the harpoon's current position forward by the amount it moves this frame

            if (Physics.Raycast(_harpoonSpear.transform.position, movement, out _hit, 1f, ~_excludeLayers))
            {
                //if hit grabbable object then child object to harpoon to bring object back with harpoon
                if(_hit.transform.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
                {
                    _hit.transform.SetParent(_harpoonSpear.transform);
                    Rigidbody hitRB = _hit.transform.GetComponent<Rigidbody>();
                    if(hitRB != null)
                    {
                        hitRB.isKinematic = true;
                    }
                }
                // Harpoon _hit something, stop its movement and start reeling it in
                _harpoonSpear.transform.position = _hit.point; // Snap the harpoon to the _hit point
                StartReeling(_hit.point);
                yield break; // Exit the coroutine, no need to keep moving the harpoon
            }

            // If no collision, move the harpoon
            _harpoonSpear.transform.Translate(movement);
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
    void StartReeling(Vector3 _hitPosition)
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
    IEnumerator ReelHarpoon()
    {
        yield return new WaitForSeconds(.05f);
        float elapsedTime = 0;
        if(!_holdToRetractMode)
        {
            _harpoonAnimator.SetTrigger(_harpoonRetractTrigger);
            //cause the wave action again when reeling
            _harpoonRope.StopDrawingRope();
            _harpoonRope.StartDrawingRope();
        }
        // Lerp the harpoon back to the player over time
        var startPos = _harpoonSpear.transform.position;
        bool startedRetracting = false;
        while(elapsedTime < _currentReelDur){
            //if hold to retract is on, only pull in harpoon if holding down button
            if(_holdToRetractMode)
            {
                if(_harpoonShoot.action.inProgress)
                {
                    if(!startedRetracting){
                        startedRetracting = true;
                        _harpoonAnimator.SetTrigger(_harpoonRetractTrigger);
                        //cause the wave action again when reeling
                        _harpoonRope.StopDrawingRope();
                        _harpoonRope.StartDrawingRope();
                    }
                    _isShooting = true;
                    _harpoonSpear.transform.GetChild(0).LookAt(HarpoonTip);
                    _harpoonSpear.transform.position = Vector3.Lerp(startPos, HarpoonTip.position, elapsedTime/ _currentReelDur);
                    elapsedTime+= Time.deltaTime;
                } 
                //otherwise automatically pull in 
            }else
            {
                _harpoonSpear.transform.GetChild(0).LookAt(HarpoonTip);
                _harpoonSpear.transform.position = Vector3.Lerp(startPos, HarpoonTip.position, elapsedTime/ _currentReelDur);
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
        _harpoonRope.StopDrawingRope();
    }
    /// <summary>
    /// Resets the harpoon and "reloads" it
    /// </summary>
    IEnumerator ResetHarpoon(){
        _hit = new RaycastHit();
        
        Destroy(_harpoonSpear);
        _isShooting = false;
        yield return new WaitForSeconds(_gunCD);
        _isReeling = false;
        _harpoonOnGun.SetActive(true);
    }
}
