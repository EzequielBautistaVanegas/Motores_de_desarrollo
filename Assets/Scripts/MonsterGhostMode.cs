using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterGhostMode : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;

    [Header("Fantasma")]
    [SerializeField, Min(0f)] private float ghostSpeed = 7f;
    [SerializeField, Min(0f)] private float rotationSpeed = 10f;

    [Header("Piso")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Min(0.1f)] private float groundRayStartHeight = 2f;
    [SerializeField, Min(0.1f)] private float groundCheckDistance = 6f;
    [SerializeField] private float groundOffset = 0f;

    [Header("NavMesh")]
    [SerializeField, Min(0.1f)] private float navMeshSearchRadius = 8f;

    private bool active;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    public void EnterGhostMode()
    {
        if (active)
        {
            return;
        }

        active = true;

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.enabled = false;
        }
    }

    public bool ExitGhostMode()
    {
        if (!active)
        {
            return true;
        }

        if (agent == null)
        {
            return false;
        }

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            Debug.LogWarning("Monster could not find a nearby NavMesh position.");
            return false;
        }

        transform.position = hit.position;

        agent.enabled = true;
        agent.Warp(hit.position);

        active = false;
        return true;
    }

    public void Tick()
    {
        if (!active || player == null)
        {
            return;
        }

        Vector3 horizontalDirection = player.position - transform.position;
        horizontalDirection.y = 0f;

        if (horizontalDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        horizontalDirection.Normalize();

        Vector3 targetPosition = transform.position + horizontalDirection * ghostSpeed * Time.deltaTime;

        Vector3 rayOrigin =
            targetPosition + Vector3.up * groundRayStartHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit groundHit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            targetPosition.y = groundHit.point.y + groundOffset;
        }
        else
        {
            // Si no detecta piso no se mueve
            return;
        }

        transform.position = targetPosition;

        Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
