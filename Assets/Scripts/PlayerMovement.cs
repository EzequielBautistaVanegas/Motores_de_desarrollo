using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Transform playerCamera;

    [Header("Movimienhto")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Vision")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 80f;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = input.MoveInput;

        Vector3 horizontalDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (horizontalDirection.sqrMagnitude > 1f)
        {
            horizontalDirection.Normalize();
        }

        controller.Move(horizontalDirection * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        Vector2 lookInput = input.LookInput;

        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
