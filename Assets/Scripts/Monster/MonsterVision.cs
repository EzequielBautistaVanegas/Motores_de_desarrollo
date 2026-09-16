using UnityEngine;

public class MonsterVision : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Vision")]
    [SerializeField, Min(0f)] private float visionDistance = 12f;
    [SerializeField, Range(0f, 360f)] private float visionAngle = 90f;
    [SerializeField] private float eyeHeight = 1.6f;
    [SerializeField] private float playerTargetHeight = 1f;

    [Header("Elementos que bloquean la vision")]
    [SerializeField] private LayerMask visionBlockingLayers;

    public bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPosition = player.position + Vector3.up * playerTargetHeight;
        Vector3 toPlayer = targetPosition - eyePosition;

        float distance = toPlayer.magnitude;

        if (distance > visionDistance || distance <= Mathf.Epsilon)
        {
            return false;
        }

        Vector3 direction = toPlayer / distance;
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle > visionAngle * 0.5f)
        {
            return false;
        }

        bool blocked = Physics.Raycast(eyePosition, direction, distance, visionBlockingLayers, QueryTriggerInteraction.Ignore);

        return !blocked;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;

        Gizmos.DrawWireSphere(eyePosition, visionDistance);

        Vector3 leftDirection = Quaternion.Euler(0f, -visionAngle * 0.5f, 0f) * transform.forward;

        Vector3 rightDirection = Quaternion.Euler(0f, visionAngle * 0.5f, 0f) * transform.forward;

        Gizmos.DrawRay(eyePosition, leftDirection * visionDistance);
        Gizmos.DrawRay(eyePosition, rightDirection * visionDistance);
    }
}
