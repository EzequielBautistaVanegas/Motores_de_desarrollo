using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private float playerEye;
    [SerializeField] private LayerMask interactiveLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Movement movement;

    [Header("Seguridad")]
    [Tooltip("Si la animación no libera al player (ej: trigger mal escrito), se libera solo pasado este tiempo")]
    [SerializeField] private float maxBusyTime = 3f;

    public static System.Action<string, bool> OnInteractAnimation;   // trigger, mirror
    public static System.Action<IInteractable> OnTargetChanged;

    private IInteractable currentTarget;
    private int interactionId;

    private void Awake()
    {
        if (movement == null) movement = GetComponent<Movement>();
    }

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
        if (movement != null && movement.IsBusy) return;

        IInteractable target = currentTarget;

        // Trabada / sin llave / lado equivocado: se llama igual (para su mensaje),
        // pero sin acercarse ni animar
        if (!target.CanInteract(transform))
        {
            target.Interact(transform);
            RefreshPrompt();
            return;
        }

        Transform stand = target.GetStandPoint(transform.position);
        bool hasAnim = !string.IsNullOrEmpty(target.AnimTrigger);

        if (stand != null && hasAnim && movement != null)
            StartCoroutine(InteractRoutine(target, stand));
        else
            DoInteract(target, stand);
    }

    private IEnumerator InteractRoutine(IInteractable target, Transform stand)
    {
        int id = ++interactionId;

        movement.SetBusy(true);
        yield return movement.MoveTo(stand.position, target.PromptAnchor.position);

        DoInteract(target, stand);

        // Failsafe: normalmente lo libera InteractionAnimState al terminar la animación
        yield return new WaitForSeconds(maxBusyTime);
        if (id == interactionId && movement.IsBusy)
        {
            Debug.LogWarning("Player liberado por timeout: revisá que los estados Open/Close tengan InteractionAnimState y que los triggers coincidan.");
            movement.SetBusy(false);
        }
    }

    private void DoInteract(IInteractable target, Transform stand)
    {
        // Leer el trigger ANTES de Interact(), porque Interact() cambia isOpen
        string trigger = target.AnimTrigger;

        // La animación original usa la mano izquierda; la espejada, la derecha
        StandPoint point = stand != null ? stand.GetComponent<StandPoint>() : null;
        bool mirror = point != null && point.UseHand == StandPoint.Hand.Right;

        target.Interact(transform);

        if (!string.IsNullOrEmpty(trigger)) OnInteractAnimation?.Invoke(trigger, mirror);
        RefreshPrompt();
    }

    // El objeto es el mismo, pero su Prompt pudo cambiar (Abrir <-> Cerrar)
    private void RefreshPrompt()
    {
        if (currentTarget != null) OnTargetChanged?.Invoke(currentTarget);
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