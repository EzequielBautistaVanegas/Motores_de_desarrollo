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
    [SerializeField] private float maxBusyTime = 3f;

    [Header("Equipamiento")]
    [SerializeField] private GameObject lampInHand;

    public static System.Action<string, bool> OnInteractAnimation;
    public static System.Action<IInteractable> OnTargetChanged;

    private IInteractable currentTarget;
    private int interactionId;
    private bool isLampEquipped;

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
        CheckInput();

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

    private void CheckInput()
    {
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            ToggleLamp();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || currentTarget == null) return;
        if (movement != null && movement.IsBusy) return;

        IInteractable target = currentTarget;

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

        yield return new WaitForSeconds(maxBusyTime);
        if (id == interactionId && movement.IsBusy)
        {
            Debug.LogWarning("Player liberado por timeout");
            movement.SetBusy(false);
        }
    }

    private void DoInteract(IInteractable target, Transform stand)
    {
        string trigger = target.AnimTrigger;

        StandPoint point = stand != null ? stand.GetComponent<StandPoint>() : null;
        bool mirror = point != null && point.UseHand == StandPoint.Hand.Right;

        target.Interact(transform);

        if (!string.IsNullOrEmpty(trigger)) OnInteractAnimation?.Invoke(trigger, mirror);
        RefreshPrompt();
    }

    public void EquipLamp()
    {
        if (ItemHUD.Instance != null)
        {
            ItemHUD.Instance.UnlockItem(ItemHUD.ItemType.Lamp);
        }

        isLampEquipped = true;

        if (lampInHand != null)
        {
            lampInHand.SetActive(true);
        }

        if (LampOilHUD.Instance != null)
        {
            LampOilHUD.Instance.SetLampState(true);
        }
    }

    public void ToggleLamp()
    {
        if (!ItemHUD.HasLamp) return;

        isLampEquipped = !isLampEquipped;

        if (lampInHand != null)
        {
            lampInHand.SetActive(isLampEquipped);
        }

        if (LampOilHUD.Instance != null)
        {
            LampOilHUD.Instance.SetLampState(isLampEquipped);
        }
    }

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