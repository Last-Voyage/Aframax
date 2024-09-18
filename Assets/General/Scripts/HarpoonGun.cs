using System.Collections;
using UnityEngine;

public class HarpoonGun : MonoBehaviour
{
    public GameObject harpoonPrefab; // Prefab of the harpoon
    public float harpoonSpeed = 50f; // Speed of the harpoon
    public float maxDistance = 100f; // Max travel distance
    public float reelDuration = 3f; // Time to reel in at max distance

    internal GameObject harpoonInstance;
    private bool isReeling = false;
    private Vector3 fireDirection;
    private float currentDistance;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private Transform playerLookDir;
    public Transform harpoonTip;
    [SerializeField] private GameObject harpoonOnGun;
    [SerializeField] private LayerMask excludedLayers;
    [SerializeField] private Animator harpoonAnimator;
    public bool isShooting = false;

    void Update()
    {
        // Fire the harpoon when you press the left mouse button
        if (Input.GetMouseButtonDown(0) && harpoonInstance == null)
        {
            FireHarpoon();
        }
    }

    void FireHarpoon()
    {
        // Instantiate the harpoon and set its initial position and direction
        harpoonInstance = Instantiate(harpoonPrefab, playerLookDir.position, Quaternion.identity);
        harpoonInstance.SetActive(false);
        harpoonInstance.transform.GetChild(0).transform.rotation = playerLookDir.rotation;
        isShooting = true;
        fireDirection = playerLookDir.forward; // In the direction the player is looking

        // Start moving the harpoon
        StartCoroutine(MoveHarpoon());
        harpoonOnGun.SetActive(false);
        harpoonAnimator.SetTrigger("shoot");
    }
    private RaycastHit hit;
    private void SetHarpoonActive(){
        harpoonInstance.SetActive(true);
    }
    IEnumerator MoveHarpoon()
    {
        Invoke(nameof(SetHarpoonActive), .15f);
        
        currentDistance = 0f;

        while (currentDistance < maxDistance && !isReeling)
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = fireDirection * harpoonSpeed * Time.deltaTime;

            // Cast a ray from the harpoon's current position forward by the amount it moves this frame

            if (Physics.Raycast(harpoonInstance.transform.position, movement, out hit, 1f, ~excludedLayers))
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
            currentDistance += movement.magnitude;

            yield return null;
        }

        // If max distance reached and no hit, start reeling back
        if (!isReeling)
        {
            StartReeling(harpoonInstance.transform.position);
        }
    }


    void StartReeling(Vector3 hitPosition)
    {
        
        float distanceFromPlayer = Vector3.Distance(transform.position, hitPosition);
        isShooting = false;

        // Adjust the reeling time based on the distance
        reelDuration = distanceFromPlayer / maxDistance;
        StartCoroutine(ReelHarpoon());
    }

    IEnumerator ReelHarpoon()
    {
        yield return new WaitForSeconds(.05f);
        harpoonAnimator.SetTrigger("drawBack");
        float elapsedTime = 0;
        // Lerp the harpoon back to the player over time
        var startPos = harpoonInstance.transform.position;
        isShooting = true;
        while(elapsedTime < reelDuration){
            harpoonInstance.transform.GetChild(0).LookAt(harpoonTip);
            harpoonInstance.transform.position = Vector3.Lerp(startPos, harpoonTip.position, elapsedTime/ reelDuration);
            elapsedTime+= Time.deltaTime;
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
        isReeling = false;
        harpoonOnGun.SetActive(true);
    }
}
