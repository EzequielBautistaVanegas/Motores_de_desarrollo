using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement3D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;

    [SerializeField][Range(0f, 0.5f)] private float movementDeadzone = 0.15f;


    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];

        // Evita que el personaje se caiga hacia los costados,
        // pero le permite girar sobre Y.
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        // Elimina rotaciones producidas por golpes físicos.
        playerRigidbody.angularVelocity = Vector3.zero;

        Vector2 input = moveAction.ReadValue<Vector2>();

        // Ignora movimientos pequeños producidos por el drift.
        if (input.sqrMagnitude <
            movementDeadzone * movementDeadzone)
        {
            input = Vector2.zero;
        }
        else
        {
            input = Vector2.ClampMagnitude(input, 1f);
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementDirection =
            cameraForward * input.y +
            cameraRight * input.x;

        // Mantiene la intensidad analógica del joystick.
        movementDirection =
            Vector3.ClampMagnitude(movementDirection, 1f);

        Vector3 desiredVelocity =
            movementDirection * movementSpeed;

        Vector3 currentVelocity =
            playerRigidbody.linearVelocity;

        playerRigidbody.linearVelocity = new Vector3(
            desiredVelocity.x,
            currentVelocity.y,
            desiredVelocity.z
        );

        // Solo gira si realmente hay movimiento.
        if (movementDirection.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation =
                Quaternion.LookRotation(movementDirection);

            Quaternion newRotation =
                Quaternion.RotateTowards(
                    playerRigidbody.rotation,
                    desiredRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );

            playerRigidbody.MoveRotation(newRotation);
        }
    }
}

