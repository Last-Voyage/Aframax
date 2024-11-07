/*****************************************************************************
// Name: FreeLookCamController.CS
// Author: Nabil Tagba
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
    [SerializeField] private float _speed;
    [SerializeField] private float _lookSpeed;

    [Header("References")]
    [SerializeField] private Rigidbody _body;
    [HideInInspector]
    public Transform _cam;

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
        _playerInput.DebugConsole.FreeCamMoveRight.performed += ctx => Move(ctx.ReadValue<Vector3>(), _body);

        //assign cam
        _cam = Camera.main.gameObject.transform;
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
        Vector3 dir = (_cam.right * value.x) + (_cam.up * value.y) +  (_cam.forward * value.z);
        body.velocity = dir.normalized * _speed * Time.deltaTime;// dir * speed * delta time
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
