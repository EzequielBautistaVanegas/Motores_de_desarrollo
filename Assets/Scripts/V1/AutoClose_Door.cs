using UnityEngine;

public class AutoClose_Door : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Common_Door mainDoor;
    [SerializeField] private Common_Door linkedDoor;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player") || other.GetComponentInParent<Player_Interaction>() != null)
        {
            if (mainDoor != null)
            {
                mainDoor.CloseAndLockDoor();
            }

            if (linkedDoor != null)
            {
                linkedDoor.CloseAndLockDoor();
            }

            hasTriggered = true;
            Debug.Log("Puerta principal cerrada y trabada");
        }
    }
}