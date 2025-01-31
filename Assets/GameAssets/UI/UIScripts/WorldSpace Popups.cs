using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpacePopups : MonoBehaviour
{
    private Camera _playerCamera;

    private GameObject _playerReference;

    [SerializeField]
    private Sprite _farDistanceSprite;

    [SerializeField]
    private Sprite _closeDistanceSprite;

    [SerializeField]
    private float _playerDetectionProximity;

    void Awake()
    {
        StartCoroutine(FindPlayer());
    }

    void LateUpdate()
    {
        if (_playerCamera != null)
        {
            //rotate to face the player camera
            transform.LookAt(_playerCamera.transform);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.Rotate(0, 180, 0);
        }

        if (_playerReference != null)
        {
            //check proximity to player
            //Debug.Log(Vector3.Distance(_playerReference.transform.position, transform.position));
            //Debug.Log(_playerReference.transform.position);
            if (Vector3.Distance(_playerReference.transform.position, transform.position) >= 5)
            {
                GetComponent<SpriteRenderer>().sprite = _farDistanceSprite;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = _closeDistanceSprite;
            }
        }
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        //I hope this code isn't gonna get me shot
        _playerCamera = PlayerFunctionalityCore.Instance.gameObject.transform.Find("PlayerCamera").transform.Find("Main Camera").GetComponent<Camera>();
        _playerReference = PlayerFunctionalityCore.Instance.gameObject.transform.Find("Player").gameObject;

        yield return null;
    }
}
