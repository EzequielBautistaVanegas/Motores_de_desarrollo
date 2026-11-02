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
    [SerializeField] private float _maxStamina = 100;

    [Header("Rotation")]
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _modelTurnSpeed = 10f;
    [SerializeField] private float lookDeviation = -5f;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float maxLookAngle = 30f;
    [SerializeField] private float minLookAngle = 0f;

    [Header("Physics")]
    [SerializeField] private float _gravity = -9.81f;

    [Header("Animation")]
    [SerializeField] private float dampTime = 0.15f;

    /* VARIABLES | IN CODE */
    private float _speed;
    private float _verticalVelocity;
    private bool _sprintHeld;
    private float _stamina;
    private Vector3 _move;
    private Vector2 _look;
    private CharacterController _controller;
    private PlayerInput _input;
    private Animator _animator;
    private GameObject _playerModel;
    private Quaternion targetRotation;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _animator = GetComponentInChildren<Animator>();
        _playerModel = _animator.gameObject;
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
        HandleSprint();
        HandleMovement();
        HandleLook();
    }

    /* MOVEMENT METHOD */
    private void HandleMovement()
    {
        Vector3 moveDir = transform.forward * _move.z + transform.right * _move.x;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        // Check if the player is moving forward to apply the sprint
        if (_move.z <= 0)
        {
            _speed = _walkSpeed;
        }

        Vector3 velocity = moveDir.normalized * _speed;

        // Apply gravity
        if (!_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f; // Small negative value to keep the player grounded
        }
        _verticalVelocity += _gravity * Time.deltaTime;
        velocity.y = _verticalVelocity;
        

        _controller.Move(velocity * Time.deltaTime);

        // Rotate the player model to face the movement direction just on the X
        if (Mathf.Abs(_move.x) > 0.01f && Mathf.Abs(_move.z) < 0.01f)
        {
            targetRotation = Quaternion.LookRotation(moveDir, Vector3.up) * Quaternion.Euler(0f, lookDeviation, 0f);
        }
        else
        {
            targetRotation = transform.rotation;
        }

        _playerModel.transform.rotation = Quaternion.Slerp(
            _playerModel.transform.rotation, targetRotation, Time.deltaTime * _modelTurnSpeed);

        // Animate the player based on movement input
        _animator.SetFloat("direction", _move.z, dampTime, Time.deltaTime);
        _animator.SetFloat("orientation", _move.x, dampTime, Time.deltaTime);
        _animator.SetBool("isWalking", moveDir.sqrMagnitude > 0.01f);
    }

    /* SPRINT METHOD */
    private void HandleSprint()
    {
        bool isSprinting = _sprintHeld && _move.z > 0.1f && _stamina > 0;

        // Stamina management
        if (isSprinting)
        {
            _stamina = Mathf.Max(_stamina - 1 * Time.deltaTime * 10, 0);
            OnStaminaChanged?.Invoke(_stamina, _maxStamina);
        }
        else if (!_sprintHeld && _stamina < _maxStamina)
        {
            _stamina = Mathf.Min(_stamina + 1 * Time.deltaTime * 10, _maxStamina);
            OnStaminaChanged?.Invoke(_stamina, _maxStamina);
        }

        _speed = isSprinting ? _sprintSpeed : _walkSpeed;
        _animator.SetBool("isRunning", isSprinting);
    }

    /* LOOK METHOD */
    private void HandleLook()
    {
        float mouseX = _look.x * _rotateSpeed;
        float mouseY = _look.y * _rotateSpeed;
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

    /* HUD ACTIONS */
    public static Action<float, float> OnStaminaChanged;

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
        if (context.performed) _sprintHeld = true;
        if (context.canceled) _sprintHeld = false;
    }
}
