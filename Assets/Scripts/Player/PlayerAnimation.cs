using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float dampTime = 0.15f;

    private Movement _movement;
    private Animator _animator;

    private static readonly int DirectionHash = Animator.StringToHash("direction");
    private static readonly int OrientationHash = Animator.StringToHash("orientation");
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int MirrorHash = Animator.StringToHash("Mirror");

    private void Awake()
    {
        _movement = GetComponent<Movement>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (_movement != null)
        {
            _movement.SprintChanged += OnSprintChanged;
        }

        Player_Interaction.OnInteractAnimation += PlayInteract;
    }

    private void OnDisable()
    {
        if (_movement != null)
        {
            _movement.SprintChanged -= OnSprintChanged;
        }

        Player_Interaction.OnInteractAnimation -= PlayInteract;
    }

    private void Update()
    {
        if (_animator == null) return;

        Vector2 input = _movement != null ? _movement.MoveInput : Vector2.zero;
        _animator.SetFloat(DirectionHash, input.y, dampTime, Time.deltaTime);
        _animator.SetFloat(OrientationHash, input.x, dampTime, Time.deltaTime);
        _animator.SetBool(IsWalkingHash, input.sqrMagnitude > 0.01f);
    }

    private void OnSprintChanged(bool sprinting)
    {
        if (_animator != null)
        {
            _animator.SetBool(IsRunningHash, sprinting);
        }
    }

    private void PlayInteract(string trigger, bool mirror)
    {
        if (_animator == null) return;

        _animator.SetBool(MirrorHash, mirror);
        _animator.SetTrigger(trigger);
    }
}