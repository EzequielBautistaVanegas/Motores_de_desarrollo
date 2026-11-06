using UnityEngine;
using UnityEngine.AI;

public class Anim_Enemy : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private _TestEnemy enemyAI;

    [Header("Suavizado")]
    [SerializeField] private float speedDampTime;

    private void Awake()
    {
        if (animator == null) animator =GetComponentInChildren<Animator>();
        if (agent == null) agent = GetComponentInChildren<NavMeshAgent>();
        if (enemyAI == null) enemyAI = GetComponent<_TestEnemy>();
    }

    private void Update()
    {
        if (animator == null) return;

        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            float currentspeed = agent.velocity.magnitude;
                animator.SetFloat("Speed", currentspeed, speedDampTime, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0f, speedDampTime, Time.deltaTime);
        }
    }

    public void AttackAnim()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

}
