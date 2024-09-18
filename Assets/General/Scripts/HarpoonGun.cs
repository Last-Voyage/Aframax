using System.Collections;
using UnityEngine;

public class HarpoonGun : MonoBehaviour
{
    [SerializeField] private GameObject _harpoonPrefab; // Prefab of the harpoon
    [SerializeField] private float _harpoonSpeed = 50f; // Speed of the harpoon
    [SerializeField] private float _maxDistance = 100f; // Max travel distance
    [SerializeField] private float _reelDuration = 3f; // Time to reel in at max distance
    [SerializeField] private float _gunCD = 2f;
    [SerializeField] private bool _holdToRetractMode = true;

    internal GameObject harpoonInstance;
    private bool _isReeling = false;
    private Vector3 _fireDir;
    private float _currentDist;
    [SerializeField] private Transform _playerLookDir;
    public Transform harpoonTip;
    [SerializeField] private GameObject _harpoonOnGun;
    [SerializeField] private LayerMask _excludeLayers;
    [SerializeField] private Animator _harpoonAnimator;
    internal bool isShooting = false;

    void Update()
    {
        // Fire the harpoon when you press the left mouse button
        if (Input.GetMouseButtonDown(0) && harpoonInstance == null && !_isReeling)
        {
            FireHarpoon();
        }
    }

    void FireHarpoon()
    {
        // Instantiate the harpoon and set its initial position and direction
        harpoonInstance = Instantiate(_harpoonPrefab, _playerLookDir.position, Quaternion.identity);
        harpoonInstance.SetActive(false);
        harpoonInstance.transform.GetChild(0).transform.rotation = _playerLookDir.rotation;
        isShooting = true;
        _fireDir = _playerLookDir.forward; // In the direction the player is looking

        // Start moving the harpoon
        StartCoroutine(MoveHarpoon());
        _harpoonOnGun.SetActive(false);
        _harpoonAnimator.SetTrigger("shoot");
    }
    private RaycastHit hit;
    private void SetHarpoonActive(){
        //delays visual of harpoon appearing for better appearance
        harpoonInstance.SetActive(true);
    }
    IEnumerator MoveHarpoon()
    {
        Invoke(nameof(SetHarpoonActive), .15f);
        
        _currentDist = 0f;

        while (_currentDist < _maxDistance && !_isReeling)
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = _fireDir * _harpoonSpeed * Time.deltaTime;

            // Cast a ray from the harpoon's current position forward by the amount it moves this frame

            if (Physics.Raycast(harpoonInstance.transform.position, movement, out hit, 1f, ~_excludeLayers))
            {
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Grabbable")){
                    hit.transform.SetParent(harpoonInstance.transform);
                    if(hit.transform.GetComponent<Rigidbody>() != null){
                        hit.transform.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                // Harpoon hit something, stop its movement and start reeling it in
                harpoonInstance.transform.position = hit.point; // Snap the harpoon to the hit point
                StartReeling(hit.point);
                yield break; // Exit the coroutine, no need to keep moving the harpoon
            }

            // If no collision, move the harpoon
            harpoonInstance.transform.Translate(movement);
            _currentDist += movement.magnitude;

            yield return null;
        }

        // If max distance reached and no hit, start reeling back
        if (!_isReeling)
        {
            StartReeling(harpoonInstance.transform.position);
        }
    }


    void StartReeling(Vector3 hitPosition)
    {
        _isReeling = true;
        
        float distanceFromPlayer = Vector3.Distance(transform.position, hitPosition);
        

        // Adjust the reeling time based on the distance
        _reelDuration = distanceFromPlayer / _maxDistance;
        StartCoroutine(ReelHarpoon());
    }

    IEnumerator ReelHarpoon()
    {
        yield return new WaitForSeconds(.05f);
        _harpoonAnimator.SetTrigger("drawBack");
        float elapsedTime = 0;
        // Lerp the harpoon back to the player over time
        var startPos = harpoonInstance.transform.position;
        while(elapsedTime < _reelDuration){
            if(_holdToRetractMode){
                if(Input.GetKey(KeyCode.Mouse0)){
                    isShooting = true;
                    harpoonInstance.transform.GetChild(0).LookAt(harpoonTip);
                    harpoonInstance.transform.position = Vector3.Lerp(startPos, harpoonTip.position, elapsedTime/ _reelDuration);
                    elapsedTime+= Time.deltaTime;
                }  
            }else{
                harpoonInstance.transform.GetChild(0).LookAt(harpoonTip);
                harpoonInstance.transform.position = Vector3.Lerp(startPos, harpoonTip.position, elapsedTime/ _reelDuration);
                elapsedTime+= Time.deltaTime; 
            }
            
            
            yield return null;
        }
        // get rid of harpoon
        if(hit.transform != null){
            hit.transform.SetParent(null);
            if(hit.transform.GetComponent<Rigidbody>() != null){
                hit.transform.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        hit = new RaycastHit();
        
        Destroy(harpoonInstance);
        isShooting = false;
        yield return new WaitForSeconds(_gunCD);
        _isReeling = false;
        _harpoonOnGun.SetActive(true);
    }
}
