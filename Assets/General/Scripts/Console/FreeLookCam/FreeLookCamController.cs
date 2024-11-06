using UnityEngine;
using UnityEngine.InputSystem;


public class FreeLookCamController : MonoBehaviour
{
    private PlayerInputMap _playerInput;

    private InputAction _movementInput;
    private Rigidbody _rb;

    [Header("Settings")]
    [SerializeField] private float _speed;
    [SerializeField] private float _lookSpeed;

    [Header("References")]
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Transform _cam;

    private void Awake()
    {
        _playerInput = new PlayerInputMap();
        _playerInput.Enable();
        
    }

    private void Start()
    {
        _movementInput = _playerInput.DebugConsole.FreeCamMoveRight;
        _playerInput.DebugConsole.FreeCamMoveRight.performed += ctx => Move(ctx.ReadValue<Vector3>(), _body);

    }


    private void Move(Vector3 value, Rigidbody body)
    {
        Vector3 dir = (_cam.right * value.x) + (_cam.up * value.y) +  (_cam.forward * value.z);
        body.velocity = dir.normalized * _speed * Time.deltaTime;// dir * speed * delta time
    }

    

    private void OnDisable()
    {
        _playerInput.DebugConsole.FreeCamMoveRight.performed -= ctx => Move(ctx.ReadValue<Vector3>(), _body);
        _playerInput.Disable();

    }
}
