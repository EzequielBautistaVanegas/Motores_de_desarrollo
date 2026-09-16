using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask interactiveLayer;

    private void Update()
    {
        // Show raycast for debuggin
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.red);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interaction input received.");
        if (context.performed)
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactiveLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
