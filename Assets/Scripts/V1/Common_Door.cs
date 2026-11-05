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

    [Header("Debug - Estado")]
    [SerializeField] private bool isOpen = false;

    private bool isAnimating = false;
    private Quaternion closedRotation;

    public string Prompt
    {
        get
        {
            if (isLocked)
            {
                return ItemHUD.HasKey ? openPrompt : lockedPrompt;
            }

            if (!isOpen && isOneWay)
            {
                Transform playerTransform = GetPlayerTransform();
                if (playerTransform != null)
                {
                    bool isPlayerInFront = CheckIfPlayerInFront(playerTransform);
                    if ((onlyFromFront && !isPlayerInFront) || (!onlyFromFront && isPlayerInFront))
                    {
                        return lockedPrompt;
                    }
                }
            }

            return isOpen ? closePrompt : openPrompt;
        }
    }

    public Transform PromptAnchor => _promptAnchor != null ? _promptAnchor : transform;
    public string AnimTrigger => "Interact";

    public Transform GetStandPoint(Vector3 playerPos)
    {
        return transform;
    }

    private void Start()
    {
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
            CloseDoor();
            if (linkDoor != null && linkDoor.isOpen)
            {
                linkDoor.CloseDoor();
            }
            return;
        }

        if (playerTransform == null)
        {
            playerTransform = GetPlayerTransform();
        }

        bool isPlayerInFront = CheckIfPlayerInFront(playerTransform);

        if (isOneWay)
        {
            if ((onlyFromFront && !isPlayerInFront) || (!onlyFromFront && isPlayerInFront))
            {
                Debug.Log("Puerta Trabada");
                return;
            }
        }

        float primaryAngle = isPlayerInFront ? -openAngle : openAngle;
        OpenDoorWithAngle(primaryAngle);

        if (linkDoor != null)
        {
            linkDoor.OpenDoorWithAngle(-primaryAngle);
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
        if (Camera.main != null)
        {
            return Camera.main.transform;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        return playerObj != null ? playerObj.transform : null;
    }

    private bool CheckIfPlayerInFront(Transform playerTransform)
    {
        if (playerTransform == null) return true;

        Vector3 localPlayerPos = transform.InverseTransformPoint(playerTransform.position);
        if (facingAxis == FacingAxis.Local_Y) return localPlayerPos.y > 0;
        if (facingAxis == FacingAxis.Local_X) return localPlayerPos.x > 0;
        return localPlayerPos.z > 0;
    }

    public void OpenDoorWithAngle(float angle)
    {
        if (isAnimating) return;
        Quaternion targetRotation = closedRotation * Quaternion.AngleAxis(angle, rotationAxis);
        StartCoroutine(AnimateDoor(targetRotation));
        isOpen = true;
    }

    public void CloseDoor()
    {
        if (isAnimating) return;
        StartCoroutine(AnimateDoor(closedRotation));
        isOpen = false;
    }

    private IEnumerator AnimateDoor(Quaternion targetRotation)
    {
        isAnimating = true;
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