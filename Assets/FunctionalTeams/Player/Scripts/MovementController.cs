using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class MovementController : MonoBehaviour
{
    // IMPORTANT: THIS CLASS ALSO TEMPORARILY CONTROLS THE CAMERA
    // REMOVE THIS WHEN WE IMPLEMENT A MORE DYNAMIC CAMERA
    [SerializeField] private Transform playerCamera;
    private float xRotation;

    // Horizontal Movement
    [SerializeField] private float speed;

    // Vertical Movement
    [SerializeField] private float gravity;
    [SerializeField] private LayerMask groundMask;
    private float currentVerVelo;
    bool isGrounded;

    // Mouse Controls
    [SerializeField] private float mouseSensitivityX;
    [SerializeField] private float mouseSensitivityY;
    private float cameraClamp;

    // Input
    private PlayerInput playerInput;
    private InputAction movement;
    private InputAction mouseX;
    private InputAction mouseY;

    // Character Controller
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        xRotation = 0;
        currentVerVelo = 0;
        isGrounded = true;
        cameraClamp = 90;

        playerInput = GetComponent<PlayerInput>();
        playerInput.currentActionMap.Enable();
        movement = playerInput.currentActionMap.FindAction("Movement");
        mouseX = playerInput.currentActionMap.FindAction("MouseX");
        mouseY = playerInput.currentActionMap.FindAction("MouseY");

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDir = movement.ReadValue<Vector2>();
        Vector3 newMovement = (transform.right * moveDir.x + transform.forward * moveDir.y) * speed;
        controller.Move(newMovement * Time.deltaTime);

        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);
        if (isGrounded)
        {
            currentVerVelo = 0;
        }
        currentVerVelo -= gravity * Time.deltaTime;
        controller.Move(new Vector3(0, currentVerVelo, 0) * Time.deltaTime);

        transform.Rotate(Vector3.up, mouseX.ReadValue<float>() * Time.deltaTime * mouseSensitivityX);

        xRotation -= mouseY.ReadValue<float>() * mouseSensitivityY;
        xRotation = Mathf.Clamp(xRotation, -cameraClamp, cameraClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        playerCamera.eulerAngles = targetRotation;
    }
}
