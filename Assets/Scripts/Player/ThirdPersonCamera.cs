using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform player;

    [Header("Posición")]
    [SerializeField] private float distance = 4f;
    [SerializeField] private float height = 1.5f;

    [Header("Ángulo inicial y recentrado")]
    [SerializeField] private float recenterVerticalAngle = 5f;

    [Header("Sensibilidad")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float gamepadSensitivity = 120f;

    [Header("Movimiento")]
    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private float minimumVerticalAngle = -20f;
    [SerializeField] private float maximumVerticalAngle = 65f;

    private InputAction lookAction;
    private InputAction recenterAction;

    private float horizontalAngle;
    private float verticalAngle;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("No se asignó el Player a la cámara.");
            enabled = false;
            return;
        }

        PlayerInput playerInput =
            player.GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("El Player no tiene Player Input.");
            enabled = false;
            return;
        }

        lookAction =
            playerInput.actions.FindAction("Look");

        recenterAction =
            playerInput.actions.FindAction("RecenterCamera");

        // La cámara comienza detrás del jugador.
        horizontalAngle = player.eulerAngles.y;
        verticalAngle = recenterVerticalAngle;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (recenterAction != null &&
            recenterAction.WasPressedThisFrame())
        {
            RecenterCamera();
        }
        else
        {
            RotateCamera();
        }

        UpdateCameraPosition();
    }

    private void RotateCamera()
    {
        if (lookAction == null)
        {
            return;
        }

        Vector2 lookInput =
            lookAction.ReadValue<Vector2>();

        bool usingMouse =
            lookAction.activeControl?.device is Mouse;

        if (usingMouse)
        {
            horizontalAngle +=
                lookInput.x * mouseSensitivity;

            verticalAngle -=
                lookInput.y * mouseSensitivity;
        }
        else
        {
            horizontalAngle +=
                lookInput.x *
                gamepadSensitivity *
                Time.deltaTime;

            verticalAngle -=
                lookInput.y *
                gamepadSensitivity *
                Time.deltaTime;
        }

        verticalAngle = Mathf.Clamp(
            verticalAngle,
            minimumVerticalAngle,
            maximumVerticalAngle
        );
    }

    private void RecenterCamera()
    {
        // Copia la dirección actual del jugador.
        horizontalAngle = player.eulerAngles.y;

        // Devuelve la inclinación al ángulo configurado.
        verticalAngle = recenterVerticalAngle;
    }

    private void UpdateCameraPosition()
    {
        Quaternion cameraRotation =
            Quaternion.Euler(
                verticalAngle,
                horizontalAngle,
                0f
            );

        Vector3 targetPosition =
            player.position + Vector3.up * height;

        Vector3 desiredPosition =
            targetPosition -
            cameraRotation * Vector3.forward * distance;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(targetPosition);
    }
}