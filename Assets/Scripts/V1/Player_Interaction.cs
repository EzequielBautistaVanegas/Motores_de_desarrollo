using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private float playerEye;
    [SerializeField] private LayerMask interactiveLayer;
    [SerializeField] private Transform cameraTransform;

    public static System.Action<string, bool> OnInteractAnimation;
    public static System.Action<IInteractable> OnTargetChanged;

    private IInteractable currentTarget;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        Vector3 rayOrigin = GetRayOrigin();
        Vector3 rayDirection = GetRayDirection();

        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red);

        Ray ray = new Ray(rayOrigin, rayDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactiveLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != currentTarget)
            {
                currentTarget = interactable;
                OnTargetChanged?.Invoke(currentTarget);
            }
        }
        else
        {
            if (currentTarget != null)
            {
                currentTarget = null;
                OnTargetChanged?.Invoke(null);
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || currentTarget == null) return;

        currentTarget.Interact(transform);
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