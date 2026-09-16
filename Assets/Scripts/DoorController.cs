using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    /* Configuration Variables */
    [Header("Rotation Configuration")]
    [SerializeField] private float maxRotation = 120f;
    [SerializeField] private float minRotation = 0f;
    [SerializeField] private float rotationVelocity = 60f;
    [Header("Start Status")]
    [SerializeField] private bool isOpen;

    /* Variables In code */
    private Transform leftDoor, rightDoor;
    private Quaternion rotationLeftTarget, rotationRightTarget;

    private void Awake()
    {
        // Get the child transform of the door objects
        leftDoor = transform.Find("PuertaIzquierda");
        rightDoor = transform.Find("PuertaDerecha");
    }

    private void Start()
    {
        // Set the Start Status
        Interact();
    }

    /* Set Where The Doors Will Move */
    public void Interact()
    {
        // Set the rotation target
        if (leftDoor != null)
            rotationLeftTarget = Quaternion.Euler(new Vector3(0f, 0f, !isOpen ? minRotation : -maxRotation));
        if (rightDoor != null)
            rotationRightTarget = Quaternion.Euler(new Vector3(0f, 0f, !isOpen ? minRotation : maxRotation));

        isOpen = !isOpen; // Toggle the door status
    }

    /* Smooth Doors Movement */
    private void Update()
    {
        if (leftDoor != null)
        {
            leftDoor.localRotation = Quaternion.RotateTowards(
                leftDoor.localRotation,
                rotationLeftTarget,
                rotationVelocity * Time.deltaTime
            );
        }

        if (rightDoor != null)
        {
            rightDoor.localRotation = Quaternion.RotateTowards(
                rightDoor.localRotation,
                rotationRightTarget,
                rotationVelocity * Time.deltaTime
            );
        }
    }
}