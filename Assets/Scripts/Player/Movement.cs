using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    [SerializeField] private float _staminaRegenRate = 15f;

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

    [Header("Auto Approach")]
    [SerializeField] private float _approachTurnSpeed = 360f;   // grados por segundo
    [SerializeField] private float _approachTimeout = 1.5f;

    /* VARIABLES | IN CODE */
    private float _speed;
    private float _verticalVelocity;
    private float _stamina = 100f;
    private bool _sprintHeld;
    private bool _canMove = true;
    public bool CanMove => _canMove;
    public bool IsSprinting { get; private set; }
    public bool IsGrounded => _controller.isGrounded;
    public Vector2 MoveInput => IsBusy ? _autoMoveInput : new Vector2(_move.x, _move.z);
    private Vector3 _move;
    private Vector2 _look;
    private Vector2 _autoMoveInput;
    private CharacterController _controller;
    private PlayerInput _input;
    private Animator _animator;
    private GameObject _playerModel;
    private Quaternion targetRotation;
    public bool IsBusy { get; private set; }
    private Vector3 MoveValue => IsBusy ? Vector3.zero : _move;
    public void SetBusy(bool busy) => IsBusy = busy;
    public IEnumerator MoveTo(Vector3 targetPos, Vector3 lookAt)
    {
        const float arriveDist = 0.05f;

        targetPos.y = transform.position.y;
        Vector3 look = lookAt - targetPos;
        look.y = 0f;
        Quaternion targetRot = Quaternion.LookRotation(look);

        float timer = 0f;

        while (timer < _approachTimeout)
        {
            timer += Time.deltaTime;

            Vector3 toTarget = targetPos - transform.position;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;

            bool arrived = dist < arriveDist;
            bool facing = Quaternion.Angle(transform.rotation, targetRot) < 2f;
            if (arrived && facing) break;

            // Move at the walk speed
            Vector3 step = Vector3.ClampMagnitude(toTarget, _walkSpeed * Time.deltaTime);
            _controller.Move(step);

            // Turn towards the target
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, _approachTurnSpeed * Time.deltaTime);

            // Fake the input to make the player walk with the animation
            Vector3 local = arrived ? Vector3.zero : transform.InverseTransformDirection(toTarget / dist);
            _autoMoveInput = new Vector2(local.x, local.z);

            yield return null;
        }

        _autoMoveInput = Vector2.zero;
    }

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
        Vector3 moveDir = transform.forward * MoveValue.z + transform.right * MoveValue.x;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        // Check if the player is moving forward to apply the sprint
        if (MoveValue.z <= 0)
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
        if (Mathf.Abs(MoveValue.x) > 0.01f && Mathf.Abs(MoveValue.z) < 0.01f)
        {
            targetRotation = Quaternion.LookRotation(moveDir, Vector3.up) * Quaternion.Euler(0f, lookDeviation, 0f);
        }
        else
        {
            targetRotation = transform.rotation;
        }

        _playerModel.transform.rotation = Quaternion.Slerp(
            _playerModel.transform.rotation, targetRotation, Time.deltaTime * _modelTurnSpeed);
    }

    /* SPRINT METHOD */
    private void HandleSprint()
    {
        bool sprinting = _sprintHeld && MoveValue.z > 0.1f;

        if (sprinting != IsSprinting)
        {
            IsSprinting = sprinting;
            SprintChanged?.Invoke(sprinting);
        }

        // Handle stamina consumption and regeneration
        if (sprinting && _stamina > 0f)
        {
            _stamina -= _staminaRegenRate * Time.deltaTime;

            if (_stamina < 0f) _stamina = 0f;

            OnStaminaChanged?.Invoke(_stamina, _maxStamina);

        }
        else if (_stamina < _maxStamina)
        {
            _stamina += _staminaRegenRate * Time.deltaTime;

            if (_stamina > _maxStamina) _stamina = _maxStamina;

            OnStaminaChanged?.Invoke(_stamina, _maxStamina);
        }

        _speed = sprinting ? _sprintSpeed : _walkSpeed;
    }

    /* LOOK METHOD */
    private void HandleLook()
    {
        if (IsBusy) return;
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

    /* ACTIONS EVENTS */
    public static Action<float, float> OnStaminaChanged;
    public event Action<bool> SprintChanged;

    /* INPUT EVENTS */
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        _move = context.ReadValue<Vector3>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        _look = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        if (context.performed) _sprintHeld = true;
        if (context.canceled) _sprintHeld = false;
    }

    /* FUNCTIONS */
    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;

        // Stop the player from previous movement and look input when disabling movement
        _move = Vector3.zero;
        _look = Vector2.zero;
        _sprintHeld = false;
    }
}
