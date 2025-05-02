/*****************************************************************************
// File Name :         FreeCamController.cs
// Author :            Tommy Roberts
// Creation Date :     1/31/2025
//
// Brief Description : This script controls a editor free cam for testing stuff in scene/
//                     (not intended for use in actual game)
*****************************************************************************/
using UnityEngine;

/// <summary>
/// Free cam controller functionality
/// </summary>
public class FreeCamController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float verticalMoveSpeed = 5f; // Speed for moving up/down

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    /// <summary>
    /// Just locks the mouse for the camera
    /// </summary>
    private void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Moves and rotates the camera based on player input
    /// </summary>
    private void Update()
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f); // Prevent over-rotation

        _yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);

        // Movement (WASD)
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // A and D keys
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;   // W and S keys

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move;

        // Vertical movement (Space to go up, Ctrl to go down)
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * verticalMoveSpeed * Time.deltaTime; // Move up in world space
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += Vector3.down * verticalMoveSpeed * Time.deltaTime; // Move down in world space
        }
    }
}