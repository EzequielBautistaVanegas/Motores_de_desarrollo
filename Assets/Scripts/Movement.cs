using System;
using System.Collections;
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

    [Header("Auto Approach (acercarse a puertas)")]
    [SerializeField] private float _approachTurnSpeed = 360f;   // grados por segundo
    [SerializeField] private float _approachTimeout = 1.5f;     // por si algo bloquea el camino

    private CharacterController _controller;
    private float _speed;
    private float _verticalVelocity;
    private Vector3 _move;
    private Vector2 _look;
    private PlayerInput _input;
    private bool _canMove = true;
    private bool _isSprinting = false;
    private float _currentStamina;
    private Vector2 _autoMoveInput;

    public bool CanMove => _canMove;
    public bool IsBusy { get; private set; }

    // El input se sigue leyendo siempre; solo se ignora mientras está bloqueado.
    // (Si se cortara en OnMovement, al desbloquear no camina hasta soltar y volver a apretar)
    private bool InputBlocked => !_canMove || IsBusy;
    private Vector3 MoveValue => InputBlocked ? Vector3.zero : _move;

    // Mientras se acerca solo a la puerta, el blend tree recibe el input "automático"
    public Vector2 MoveInput => IsBusy ? _autoMoveInput : new Vector2(MoveValue.x, MoveValue.z);

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
        bool isMoving = MoveValue.sqrMagnitude > 0.01f;

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
        // Sin "return" cuando está bloqueado: la gravedad tiene que seguir funcionando
        Vector3 input = MoveValue;
        Vector3 move = transform.forward * input.z + transform.right * input.x;
        move = move.normalized * _speed;

        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f; // pequeño empuje para que isGrounded no parpadee
        }
        _verticalVelocity += _gravity * Time.deltaTime;
        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (InputBlocked) return;

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
            if (_currentStamina > 0f && !IsBusy)
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

    public void SetBusy(bool busy)
    {
        if (IsBusy == busy) return;
        IsBusy = busy;

        // No mostrar la animación de correr mientras se acerca o abre la puerta
        if (busy) SprintChanged?.Invoke(false);
        else if (_isSprinting && _currentStamina > 0f) SprintChanged?.Invoke(true);
    }

    // Camina solo hasta targetPos (a velocidad de caminata) y gira para mirar lookAt
    public IEnumerator MoveTo(Vector3 targetPos, Vector3 lookAt)
    {
        const float arriveDist = 0.05f;

        targetPos.y = transform.position.y;
        Vector3 look = lookAt - targetPos;
        look.y = 0f;
        Quaternion targetRot = look.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(look) : transform.rotation;

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

            // Mover a velocidad de caminata
            Vector3 step = Vector3.ClampMagnitude(toTarget, _walkSpeed * Time.deltaTime);
            _controller.Move(step);

            // Girar hacia la puerta
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, _approachTurnSpeed * Time.deltaTime);

            // Input "falso" para el blend tree, relativo a hacia dónde mira el player
            Vector3 local = arrived ? Vector3.zero : transform.InverseTransformDirection(toTarget / dist);
            _autoMoveInput = new Vector2(local.x, local.z);

            yield return null;
        }

        _autoMoveInput = Vector2.zero;
    }
}