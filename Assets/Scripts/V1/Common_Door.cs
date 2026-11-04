using System.Collections;
using UnityEngine;

public class Common_Door : MonoBehaviour
{
    public enum FacingAxis { Local_Y, Local_Z, Local_X }

    [Header("Apertura")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private Common_Door linkDoor;

    [Header("Unidireccion")]
    [SerializeField] private bool isOneWay = false;
    [SerializeField] private bool onlyFromFront = true;
    [SerializeField] private FacingAxis facingAxis = FacingAxis.Local_Y;

    [Header("Debug - Estado")]
    [SerializeField] private bool isOpen = false;

    private bool isAnimating = false;
    private Quaternion closedRotation;

    private void Start()
    {
        closedRotation = transform.localRotation;
    }

    public void Interact(Transform playerTransform)
    {
        if (!enabled || isAnimating) { return; }

        if (isAnimating) return;

        if (isOpen)
        {
            CloseDoor();
            if (linkDoor != null && linkDoor.isOpen)
            {
                linkDoor.CloseDoor();
            }
            return;
        }

        Vector3 localPlayerPos = transform.InverseTransformPoint(playerTransform.position);
        bool isPlayerInFront = false;

        if (facingAxis == FacingAxis.Local_Y) isPlayerInFront = localPlayerPos.y > 0;
        else if (facingAxis == FacingAxis.Local_X) isPlayerInFront = localPlayerPos.x > 0;
        else isPlayerInFront = localPlayerPos.z > 0;

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