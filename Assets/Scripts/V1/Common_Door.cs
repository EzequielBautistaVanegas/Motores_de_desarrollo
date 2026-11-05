using System.Collections;
using UnityEngine;

public class Common_Door : MonoBehaviour, IInteractable
{
    public enum FacingAxis { Local_Y, Local_Z, Local_X }

    [Header("Apertura")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private Common_Door linkDoor;

    [Header("Bloqueo")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string lockedPrompt = "Puerta trabada";
    [SerializeField] private string openPrompt = "Abrir";
    [SerializeField] private string closePrompt = "Cerrar";

    [Header("Unidireccion")]
    [SerializeField] private bool isOneWay = false;
    [SerializeField] private bool onlyFromFront = true;
    [SerializeField] private FacingAxis facingAxis = FacingAxis.Local_Y;

    [Header("Prompt Configuration")]
    [SerializeField] private Transform _promptAnchor;

    [Header("Animación del personaje")]
    [Tooltip("Segundos desde que arranca la animación hasta que la mano toca la puerta")]
    [SerializeField] private float interactDelay = 0.4f;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";

    [Header("Puntos de parada (NO pueden ser hijos de esta puerta, porque gira)")]
    [SerializeField] private Transform openPointFront;
    [SerializeField] private Transform openPointBack;
    [SerializeField] private Transform closePointFront;   // opcional: si falta, usa el de abrir
    [SerializeField] private Transform closePointBack;    // opcional: si falta, usa el de abrir

    [Header("Debug - Estado")]
    [SerializeField] private bool isOpen = false;

    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Transform cachedPlayer;

    public string Prompt
    {
        get
        {
            if (isLocked)
            {
                return ItemHUD.HasKey ? openPrompt : lockedPrompt;
            }

            if (!isOpen && IsBlockedByOneWay(GetPlayerTransform()))
            {
                return lockedPrompt;
            }

            return isOpen ? closePrompt : openPrompt;
        }
    }

    public Transform PromptAnchor => _promptAnchor != null ? _promptAnchor : transform;
    public string AnimTrigger => isOpen ? closeTrigger : openTrigger;

    public bool CanInteract(Transform player)
    {
        if (!enabled || isAnimating) return false;
        if (isLocked && !ItemHUD.HasKey) return false;

        if (player == null) player = GetPlayerTransform();
        if (!isOpen && IsBlockedByOneWay(player)) return false;

        return true;
    }

    public Transform GetStandPoint(Vector3 playerPos)
    {
        bool front = IsInFront(playerPos);

        Transform openPoint = front ? openPointFront : openPointBack;
        if (!isOpen) return openPoint;

        Transform closePoint = front ? closePointFront : closePointBack;
        return closePoint != null ? closePoint : openPoint;
    }

    private void Awake()
    {
        // En Awake (no Start) para que IsInFront funcione aunque se consulte antes
        closedRotation = transform.localRotation;
    }

    public void Interact(Transform playerTransform = null)
    {
        if (!enabled || isAnimating) { return; }

        if (isLocked)
        {
            if (ItemHUD.HasKey)
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("Puerta Trabada");
                return;
            }
        }

        if (isOpen)
        {
            CloseDoor(interactDelay);
            if (linkDoor != null && linkDoor.isOpen)
            {
                linkDoor.CloseDoor(interactDelay);
            }
            return;
        }

        if (playerTransform == null)
        {
            playerTransform = GetPlayerTransform();
        }

        if (IsBlockedByOneWay(playerTransform))
        {
            Debug.Log("Puerta Trabada");
            return;
        }

        bool isPlayerInFront = playerTransform == null || IsInFront(playerTransform.position);

        float primaryAngle = isPlayerInFront ? -openAngle : openAngle;
        OpenDoorWithAngle(primaryAngle, interactDelay);

        if (linkDoor != null)
        {
            linkDoor.OpenDoorWithAngle(-primaryAngle, interactDelay);
        }
    }

    public void CloseAndLockDoor()
    {
        if (isOpen)
        {
            CloseDoor();
            if (linkDoor != null && linkDoor.isOpen)
            {
                linkDoor.CloseDoor();
            }
        }

        isLocked = true;
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }

    public void LockDoor()
    {
        isLocked = true;
    }

    private Transform GetPlayerTransform()
    {
        // Primero el player: con cámara sobre el hombro, la cámara puede quedar
        // del otro lado de la puerta aunque el personaje no
        if (cachedPlayer == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) cachedPlayer = playerObj.transform;
        }

        if (cachedPlayer != null) return cachedPlayer;
        return Camera.main != null ? Camera.main.transform : null;
    }

    private bool IsBlockedByOneWay(Transform player)
    {
        if (!isOneWay || player == null) return false;

        bool inFront = IsInFront(player.position);
        return onlyFromFront ? !inFront : inFront;
    }

    // Se calcula respecto de la puerta CERRADA, así da bien aunque esté abierta
    private bool IsInFront(Vector3 worldPos)
    {
        Vector3 pos = transform.parent != null ? transform.parent.InverseTransformPoint(worldPos) : worldPos;
        Vector3 rel = Quaternion.Inverse(closedRotation) * (pos - transform.localPosition);

        switch (facingAxis)
        {
            case FacingAxis.Local_Y: return rel.y > 0;
            case FacingAxis.Local_X: return rel.x > 0;
            default: return rel.z > 0;
        }
    }

    public void OpenDoorWithAngle(float angle, float delay = 0f)
    {
        if (isAnimating) return;
        Quaternion targetRotation = closedRotation * Quaternion.AngleAxis(angle, rotationAxis);
        StartCoroutine(AnimateDoor(targetRotation, delay));
        isOpen = true;
    }

    public void CloseDoor(float delay = 0f)
    {
        if (isAnimating) return;
        StartCoroutine(AnimateDoor(closedRotation, delay));
        isOpen = false;
    }

    private IEnumerator AnimateDoor(Quaternion targetRotation, float delay)
    {
        isAnimating = true;

        // Esperar a que la mano llegue a la puerta
        if (delay > 0f) yield return new WaitForSeconds(delay);

        Quaternion startRotation = transform.localRotation;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * openSpeed;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, time);
            yield return null;
        }

        transform.localRotation = targetRotation;
        isAnimating = false;
    }
}