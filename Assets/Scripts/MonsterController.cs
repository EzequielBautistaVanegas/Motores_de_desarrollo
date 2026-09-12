using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public enum MonsterState
    {
        Patrol,
        Chase,
        DarkChase,
        GhostChase
    }

    [Header("Referencias")]
    [SerializeField] private PlayerLight playerLight;
    [SerializeField] private MonsterVision vision;
    [SerializeField] private MonsterPatrol patrol;
    [SerializeField] private MonsterChase chase;
    [SerializeField] private MonsterGhostMode ghostMode;

    [Header("Temporizadores")]
    [SerializeField, Min(0f)] private float chaseMemoryTime = 3f;
    [SerializeField, Min(0f)] private float ghostModeDelay = 20f;

    public MonsterState CurrentState { get; private set; }

    private float lostPlayerTimer;
    private float darknessTimer;
    private bool previousLightState;
    private bool stateInitialized;

    private void Start()
    {
        if (playerLight == null ||
            vision == null ||
            patrol == null ||
            chase == null ||
            ghostMode == null)
        {
            Debug.LogError("MonsterController: Una o mas referencias estan vacias", this);
            enabled = false;
            return;
        }

        previousLightState = playerLight.IsLightOn();

        if (previousLightState)
        {
            ChangeState(MonsterState.Patrol);
        }
        else
        {
            ChangeState(MonsterState.DarkChase);
        }
    }

    private void Update()
    {
        bool lightIsOn = playerLight.IsLightOn();

        // Importante: si la lampara vuelve cuando el monstruo ya empezo a perseguir debe esperar los segundos del timer antes de volver a patrullar
        if (!previousLightState && lightIsOn)
        {
            lostPlayerTimer = chaseMemoryTime;
        }

        if (lightIsOn)
        {
            HandleLightOn();
        }
        else
        {
            HandleDarkness();
        }

        previousLightState = lightIsOn;
        ExecuteCurrentState();
    }

    private void HandleDarkness()
    {
        darknessTimer += Time.deltaTime;

        if (darknessTimer >= ghostModeDelay)
        {
            ChangeState(MonsterState.GhostChase);
        }
        else
        {
            ChangeState(MonsterState.DarkChase);
        }
    }

    private void HandleLightOn()
    {
        darknessTimer = 0f;

        if (vision.CanSeePlayer())
        {
            lostPlayerTimer = chaseMemoryTime;
            ChangeState(MonsterState.Chase);
            return;
        }

        bool wasChasing = CurrentState == MonsterState.Chase || CurrentState == MonsterState.DarkChase || CurrentState == MonsterState.GhostChase;

        if (!wasChasing)
        {
            ChangeState(MonsterState.Patrol);
            return;
        }

        if (lostPlayerTimer > 0f)
        {
            lostPlayerTimer -= Time.deltaTime;
            ChangeState(MonsterState.Chase);
            return;
        }

        ChangeState(MonsterState.Patrol);
    }

    private void ChangeState(MonsterState newState)
    {
        if (stateInitialized && CurrentState == newState)
        {
            return;
        }

        if (CurrentState == MonsterState.GhostChase && newState != MonsterState.GhostChase)
        {
            if (!ghostMode.ExitGhostMode())
            {
                return;
            }
        }

        CurrentState = newState;
        stateInitialized = true;

        switch (CurrentState)
        {
            case MonsterState.Patrol:
                chase.Stop();
                patrol.Activate();
                break;

            case MonsterState.Chase:
                patrol.Stop();
                chase.SetNormalChase();
                break;

            case MonsterState.DarkChase:
                patrol.Stop();
                chase.SetDarkChase();
                break;

            case MonsterState.GhostChase:
                patrol.Stop();
                chase.Stop();
                ghostMode.EnterGhostMode();
                break;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (CurrentState)
        {
            case MonsterState.Patrol:
                patrol.Tick();
                break;

            case MonsterState.Chase:
            case MonsterState.DarkChase:
                chase.Tick();
                break;

            case MonsterState.GhostChase:
                ghostMode.Tick();
                break;
        }
    }
}
