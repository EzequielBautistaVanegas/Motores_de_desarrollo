using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class _TestEnemy : MonoBehaviour
{
    [Header("Parametros")]
    [SerializeField] private Transform target;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target  != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }

    }
}
