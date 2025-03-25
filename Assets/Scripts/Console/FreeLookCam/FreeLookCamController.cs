/*****************************************************************************
// Name: FreeLookCamController.CS
// Author: Nabil Tagba
// Date: 11/5/2024
// Overview: Handles movement for the free look cam
*****************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Free look cam movement
/// </summary>
public class FreeLookCamController : MonoBehaviour
{
    private PlayerInputMap _playerInput;

    [Header("Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _lookSpeed;

    [Header("References")]
    [SerializeField] private Rigidbody _body;
    [HideInInspector] public Transform Cam;

    /// <summary>
    /// happens on awake
    /// </summary>
    private void Awake()
    {
        _playerInput = new PlayerInputMap();
        _playerInput.Enable();
    }
    
    /// <summary>
    /// happens on start
    /// </summary>
    private void Start()
    {
        //assign cam
        Cam = Camera.main.gameObject.transform;
    }
    
    /// <summary>
    /// happens every frame
    /// </summary>
    private void Update()
    {
        //moves the player with a Horizontal, vertical, forward axis
        Move(_playerInput.DebugConsole.FreeCamMoveRight.ReadValue<Vector3>(), _body);
    }

    /// <summary>
    /// moves the free look cam
    /// value holds 3 floats that represent 
    /// 3 differenct axies. Horizontal, Forword, Vertical
    /// </summary>
    /// <param name="value"></param>
    /// <param name="body"></param>
    private void Move(Vector3 value, Rigidbody body)
    {
        Vector3 dir = (Cam.right * value.x) + (Cam.up * value.y) +  (Cam.forward * value.z);
        body.velocity = dir.normalized * _moveSpeed * Time.deltaTime;// dir * speed * delta time
    }

    /// <summary>
    /// happens when the object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        _playerInput.DebugConsole.FreeCamMoveRight.performed -= ctx => Move(ctx.ReadValue<Vector3>(), _body);
        _playerInput.Disable();
    }
}
