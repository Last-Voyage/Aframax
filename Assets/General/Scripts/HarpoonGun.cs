/*****************************************************************************
// File Name :         HarpoonGun.cs
// Author :            Tommy Roberts
// Creation Date :     9/22/2024
//
// Brief Description : Controls the basic shoot harpoon and retract functionality.
*****************************************************************************/
using System.Collections;
using UnityEngine;

public class HarpoonGun : MonoBehaviour
{
    #region Inspector Variables
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
    [SerializeField] private Transform _playerLookDir;
    public Transform harpoonTip;
    [SerializeField] private GameObject _harpoonOnGun;
    [Tooltip("Layers the launched harpoon can not hit")]
    [SerializeField] private LayerMask _excludeLayers;
    [SerializeField] private Animator _harpoonAnimator;

    #endregion

    internal GameObject harpoonInstance;
    private bool _isReeling = false;
    private Vector3 _fireDir;
    private float _currentDist;
    internal bool isShooting = false;
    private RaycastHit _hit;
    private float _currentReelDur;
    /// <summary>
    /// Looks for the shoot button to be pressed and fires the harpoon
    /// </summary>
    void Update()
    {
        // Fire the harpoon when you press the left mouse button
        if (Input.GetMouseButtonDown(0) && harpoonInstance == null && !_isReeling)
        {
            FireHarpoon();
        }
    }
    /// <summary>
    /// creates a harpoon, sets up the fire direction and everything else to begin the launch
    /// </summary>
    void FireHarpoon()
    {
        // Instantiate the harpoon and set its initial position and direction
        harpoonInstance = Instantiate(_harpoonPrefab, _playerLookDir.position, Quaternion.identity);
        harpoonInstance.SetActive(false);
        harpoonInstance.transform.GetChild(0).rotation = _playerLookDir.rotation;
        isShooting = true;
        _fireDir = _playerLookDir.forward; // In the direction the player is looking

        // Start moving the harpoon
        StartCoroutine(MoveHarpoon());
        _harpoonOnGun.SetActive(false);
        _harpoonAnimator.SetTrigger("shoot");
        CinemachineShake.Instance.ShakeCamera(5f, .05f);
    }
    private void SetHarpoonActive(){
        //delays visual of harpoon appearing for better appearance
        harpoonInstance.SetActive(true);
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

            if (Physics.Raycast(harpoonInstance.transform.position, movement, out _hit, 1f, ~_excludeLayers))
            {
                if(_hit.transform.gameObject.layer == LayerMask.NameToLayer("Grabbable")){
                    _hit.transform.SetParent(harpoonInstance.transform);
                    if(_hit.transform.GetComponent<Rigidbody>() != null){
                        _hit.transform.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                // Harpoon _hit something, stop its movement and start reeling it in
                harpoonInstance.transform.position = _hit.point; // Snap the harpoon to the _hit point
                StartReeling(_hit.point);
                yield break; // Exit the coroutine, no need to keep moving the harpoon
            }

            // If no collision, move the harpoon
            harpoonInstance.transform.Translate(movement);
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
        if(_hit.transform != null){
            distanceFromPlayer = Vector3.Distance(transform.position, _hitPosition);
        }else{
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
        _harpoonAnimator.SetTrigger("drawBack");
        float elapsedTime = 0;
        // Lerp the harpoon back to the player over time
        var startPos = harpoonInstance.transform.position;
        while(elapsedTime < _currentReelDur){
            if(_holdToRetractMode){
                if(Input.GetKey(KeyCode.Mouse0)){
                    isShooting = true;
                    harpoonInstance.transform.GetChild(0).LookAt(harpoonTip);
                    harpoonInstance.transform.position = Vector3.Lerp(startPos, harpoonTip.position, elapsedTime/ _currentReelDur);
                    elapsedTime+= Time.deltaTime;
                }  
            }else{
                harpoonInstance.transform.GetChild(0).LookAt(harpoonTip);
                harpoonInstance.transform.position = Vector3.Lerp(startPos, harpoonTip.position, elapsedTime/ _currentReelDur);
                elapsedTime+= Time.deltaTime; 
            }
            
            
            yield return null;
        }
        // get rid of harpoon
        if(_hit.transform != null){
            _hit.transform.SetParent(null);
            if(_hit.transform.GetComponent<Rigidbody>() != null){
                _hit.transform.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        CinemachineShake.Instance.ShakeCamera(3f, .05f);
        _hit = new RaycastHit();
        
        Destroy(harpoonInstance);
        isShooting = false;
        yield return new WaitForSeconds(_gunCD);
        _isReeling = false;
        _harpoonOnGun.SetActive(true);
    }
}
