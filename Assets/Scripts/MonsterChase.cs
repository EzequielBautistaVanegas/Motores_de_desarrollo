using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterChase : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;

    [Header("Velocidades")]
    [SerializeField, Min(0f)] private float normalSpeed = 4f;
    [SerializeField, Min(0f)] private float darkSpeed = 6f;

    private bool active;
    private float currentSpeed;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    public void SetNormalChase()
    {
        BeginChase(normalSpeed);
    }

    public void SetDarkChase()
    {
        BeginChase(darkSpeed);
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
        if (!active || player == null || !CanUseAgent())
        {
            return;
        }

        agent.speed = currentSpeed;
        agent.SetDestination(player.position);
    }

    private void BeginChase(float speed)
    {
        active = true;
        currentSpeed = speed;

        if (!CanUseAgent())
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = currentSpeed;
    }

    private bool CanUseAgent()
    {
        return agent != null && agent.enabled && agent.isOnNavMesh;
    }
}
