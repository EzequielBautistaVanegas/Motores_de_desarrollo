using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class _TestEnemy : MonoBehaviour
{
    [Header("Parametros")]
    [SerializeField] private Transform target;
    [SerializeField] private float attackDistance = 3.5f;

    [Header("Points Vigilancia")]
    [SerializeField] private Transform[] ePoints;
    [SerializeField] private float platformChangeInterval;
    [SerializeField] private float platformMoveSpeed;

    [Header("Tiempo en descender")]
    [SerializeField] private float descendSpeed;

    private NavMeshAgent agent;
    private bool isChasing = false;
    private bool isDescending = false;
    private bool isReturning = false;
    private bool isAscending = false;
    private bool isMovingToPlataform = false;
    private int currentPlatformIndex = 0;
    private float platformTimer = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    private void OnEnable()
    {
        HealthHUD.OnTorchesExtinguished += TriggerChase;
    }

    private void OnDisable()
    {
        HealthHUD.OnTorchesExtinguished -= TriggerChase;
    }

    private void Start()
    {
        if (ePoints != null && ePoints.Length > 0 && ePoints[0] != null)
        {
            transform.position = ePoints[0].position;
        }
    }

    private void Update()
    {
        if (!isChasing && !isDescending && !isAscending && !isReturning)
        {
            PlatformRoutine();
        }

        if (isChasing && !isReturning && agent.enabled && agent.isOnNavMesh && target != null)
        {
            agent.SetDestination(target.position);

            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= attackDistance)
            {
                KillPlayer();
            }
        }
    }

    private void PlatformRoutine()
    {
        if (ePoints == null || ePoints.Length <= 1) return;

        platformTimer += Time.deltaTime;

        if (platformTimer >= platformChangeInterval && !isMovingToPlataform)
        {
            platformTimer = 0;
            currentPlatformIndex = (currentPlatformIndex + 1) % ePoints.Length;
            StartCoroutine(MoveNextPlataform(ePoints[currentPlatformIndex].position));
        }
    }

    private IEnumerator MoveNextPlataform(Vector3 targetPos)
    {
        isMovingToPlataform = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            if (isDescending || isChasing) yield break;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, platformMoveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMovingToPlataform = false;
    }

    public void TriggerChase()
    {
        if (isDescending) return;

        StopAllCoroutines();
        isMovingToPlataform = false;
        isReturning = false;
        isAscending = false;

        if (agent.enabled && agent.isOnNavMesh)
        {
            isChasing = true;
            return;
        }

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 50.0f, NavMesh.AllAreas))
        {
            StartCoroutine(DescendGround(hit.position));
        }
    }

    public void TriggerReturn()
    {
        if (isDescending || isAscending) return;
        StopAllCoroutines();
        isMovingToPlataform = false;
        isChasing = false;

        StartCoroutine(ReturnToPlataform());
    }

    private IEnumerator ReturnToPlataform()
    {
        isReturning = true;

        Vector3 targetPlatformPos = ePoints[currentPlatformIndex].position;
        Vector3 groundUnderPlatform = targetPlatformPos;

        if (NavMesh.SamplePosition(targetPlatformPos, out NavMeshHit hit, 50.0f, NavMesh.AllAreas))
        {
            groundUnderPlatform = hit.position;
        }

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(groundUnderPlatform);
            yield return new WaitForEndOfFrame();

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.3f)
            {
                if (!isReturning) yield break;
                yield return null;
            }
        }

        agent.enabled = false;
        isAscending = true;

        while (Vector3.Distance(transform.position, targetPlatformPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlatformPos, descendSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPlatformPos;

        isAscending = false;
        isReturning = false;
        isChasing = false;
        platformTimer = 0f;
    }

    private IEnumerator DescendGround(Vector3 groundPosition)
    {
        isDescending = true;

        while (Vector3.Distance(transform.position, groundPosition) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, groundPosition, descendSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = groundPosition;

        agent.enabled = true;
        if (agent.isOnNavMesh)
        {
            agent.Warp(groundPosition);
        }

        isDescending = false;
        isChasing = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isChasing) return;

        if (other.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isChasing) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        if (DeathScreenUI.Instance != null)
        {
            DeathScreenUI.Instance.ShowDeathScreen();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}