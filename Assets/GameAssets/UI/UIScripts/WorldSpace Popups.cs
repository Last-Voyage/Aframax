using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpacePopups : MonoBehaviour
{
    private Camera playerCamera;

    private GameObject playerReference;

    [SerializeField]
    private float playerDetectionProximity;

    void Awake()
    {
        StartCoroutine(FindPlayer());
    }

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            //rotate to face the player camera
            transform.LookAt(playerCamera.transform);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.Rotate(0, 180, 0);
        }

        if (playerReference != null)
        {
            //check proximity to player
            Debug.Log(Vector3.Distance(playerReference.transform.position, transform.position));
        }
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        //I hope this line of code isn't gonna get me shot
        playerCamera = PlayerFunctionalityCore.Instance.gameObject.transform.Find("PlayerCamera").transform.Find("Main Camera").GetComponent<Camera>();

        playerReference = PlayerFunctionalityCore.Instance.gameObject;

        yield return null;
    }
}
