using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Inventory inventory;

    [Header("Interacciones")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionMask = ~0;

    public Inventory Inventory => inventory;

    private void OnEnable()
    {
        if (input != null)
        {
            input.InteractPressed += TryInteract;
        }
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.InteractPressed -= TryInteract;
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionMask, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        InteractableBase interactable = hit.collider.GetComponentInParent<InteractableBase>();

        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }
}
