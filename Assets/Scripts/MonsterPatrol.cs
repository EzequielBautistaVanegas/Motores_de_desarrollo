using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterPatrol : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Patrulla")]
    [SerializeField, Min(0f)] private float patrolSpeed = 2f;
    [SerializeField, Min(0f)] private float waitTime = 1f;

    private int currentPoint;
    private float waitTimer;
    private bool active;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    public void Activate()
    {
        active = true;
        waitTimer = 0f;

        if (!CanUseAgent())
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        GoToCurrentPoint();
    }

    public void Stop()
    {
        active = false;

        if (CanUseAgent())
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void Tick()
    {
        if (!active || patrolPoints == null || patrolPoints.Length == 0 || !CanUseAgent())
        {
            return;
        }

        if (agent.pathPending)
        {
            return;
        }

        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                GoToCurrentPoint();
            }
        }
    }

    private void GoToCurrentPoint()
    {
        if (!CanUseAgent() || patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        Transform point = patrolPoints[currentPoint];

        if (point != null)
        {
            agent.SetDestination(point.position);
        }
    }

    private bool CanUseAgent()
    {
        return agent != null && agent.enabled && agent.isOnNavMesh;
    }
}
