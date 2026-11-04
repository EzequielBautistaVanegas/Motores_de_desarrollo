using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private float playerEye;
    [SerializeField] private LayerMask interactiveLayer;
    [SerializeField] private Transform cameraTransform;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * playerEye;
        Vector3 rayDirection = GetRayDirection();

        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        
        if (!context.performed) return;

        Vector3 rayOrigin = GetRayOrigin();
        Vector3 rayDirection = GetRayDirection();

        Ray ray = new Ray(rayOrigin, rayDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactiveLayer))
        {
            if (hit.collider == null) return;

            Common_Door door = hit.collider.GetComponentInParent<Common_Door>();
            if (door != null)
            {
                door.Interact(transform);
                return;
            }

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private Vector3 GetRayOrigin()
    {
        return transform.position + Vector3.up * playerEye;
    }

    private Vector3 GetRayDirection()
    {
        if (cameraTransform != null)
            return cameraTransform.forward;

        return transform.forward;
    }
}