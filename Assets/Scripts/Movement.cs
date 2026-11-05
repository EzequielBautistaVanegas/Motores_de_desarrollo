using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;

    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _staminaDrainRate = 20f;
    [SerializeField] private float _staminaRegenRate = 15f;

    [Header("Rotation")]
    [SerializeField] private float _rotateSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float maxLookAngle = 30f;
    [SerializeField] private float minLookAngle = 0f;

    [Header("Physics")]
    [SerializeField] private float _gravity = -9.81f;

    private CharacterController _controller;
    private float _speed;
    private float _verticalVelocity;
    private Vector3 _move;
    private Vector2 _look;
    private PlayerInput _input;
    private bool _canMove = true;
    private bool _isSprinting = false;
    private float _currentStamina;

    public Vector2 MoveInput => new Vector2(_move.x, _move.z);
    public System.Action<bool> SprintChanged;

    public static System.Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _speed = _walkSpeed;
        _currentStamina = _maxStamina;
        OnStaminaChanged?.Invoke(_currentStamina, _maxStamina);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleStamina();
        HandleMovement();
        HandleLook();
    }

    private void HandleStamina()
    {
        bool isMoving = _move.sqrMagnitude > 0.01f;

        if (_isSprinting && isMoving && _currentStamina > 0f)
        {
            _currentStamina -= _staminaDrainRate * Time.deltaTime;
            _currentStamina = Mathf.Clamp(_currentStamina, 0f, _maxStamina);
            _speed = _sprintSpeed;

            if (_currentStamina <= 0f)
            {
                _speed = _walkSpeed;
                SprintChanged?.Invoke(false);
            }

            OnStaminaChanged?.Invoke(_currentStamina, _maxStamina);
        }
        else
        {
            if (_isSprinting && _currentStamina <= 0f)
            {
                _speed = _walkSpeed;
            }

            if (_currentStamina < _maxStamina)
            {
                _currentStamina += _staminaRegenRate * Time.deltaTime;
                _currentStamina = Mathf.Clamp(_currentStamina, 0f, _maxStamina);
                OnStaminaChanged?.Invoke(_currentStamina, _maxStamina);
            }
        }
    }

    private void HandleMovement()
    {
        if (!_canMove) return;

        Vector3 move = transform.forward * _move.z + transform.right * _move.x;
        move = move.normalized * _speed;

        if (!_controller.isGrounded)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
            move.y = _verticalVelocity;
        }
        else
        {
            _verticalVelocity = 0f;
        }

        _controller.Move(move * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (!_canMove) return;

        float mouseX = _look.x * _rotateSpeed * Time.deltaTime;
        float mouseY = _look.y * _rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

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
            _isSprinting = true;
            if (_currentStamina > 0f)
            {
                _speed = _sprintSpeed;
                SprintChanged?.Invoke(true);
            }
        }

        if (context.canceled)
        {
            _isSprinting = false;
            _speed = _walkSpeed;
            SprintChanged?.Invoke(false);
        }
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }
}