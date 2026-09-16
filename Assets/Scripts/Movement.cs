using System;
using UnityEngine;
using UnityEngine.InputSystem;

/* REQUIRES */
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Movement : MonoBehaviour
{
    /* VARIABLES | CONFIGURATION */
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float _rotateSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float maxLookAngle = 30f;
    [SerializeField] private float minLookAngle = 0f;

    [Header("Physics")]
    [SerializeField] private float _gravity = -9.81f;

    /* VARIABLES | IN CODE */
    private CharacterController _controller;
    private float _speed;
    private float _verticalVelocity;
    private Vector3 _move;
    private Vector2 _look;
    private PlayerInput _input;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _speed = _walkSpeed;

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    /* MOVEMENT METHOD */
    private void HandleMovement()
    {
        Vector3 move = transform.forward * _move.z + transform.right * _move.x;
        move = move.normalized * _speed;

        // Apply gravity
        if (!_controller.isGrounded)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
            move.y = _verticalVelocity;
        }
        else
        {
            _verticalVelocity = 0f; // Reset vertical velocity when grounded
        }

        _controller.Move(move * Time.deltaTime);
    }

    /* LOOK METHOD */
    private void HandleLook()
    {
        float mouseX = _look.x * _rotateSpeed * Time.deltaTime;
        float mouseY = _look.y * _rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // Clamp the camera's vertical rotation
        _cameraTransform.Rotate(Vector3.left * mouseY);
        if (_cameraTransform.localEulerAngles.x > 180f)
        {
            _cameraTransform.localEulerAngles = new Vector3(Mathf.Clamp(_cameraTransform.localEulerAngles.x - 360f, minLookAngle, maxLookAngle), 0f, 0f);
        }
        else
        {
            _cameraTransform.localEulerAngles = new Vector3(Mathf.Clamp(_cameraTransform.localEulerAngles.x, minLookAngle, maxLookAngle), 0f, 0f);
        }
    }

    /* INPUT EVENTS */
    public void OnMovement(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector3>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _speed = _sprintSpeed;
        }

        if (context.canceled)
        {
            _speed = _walkSpeed;
        }
    }
}
