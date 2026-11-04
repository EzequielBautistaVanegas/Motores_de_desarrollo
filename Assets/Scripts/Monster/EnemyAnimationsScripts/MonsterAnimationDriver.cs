using UnityEngine;

public class MonsterAnimationDriver : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Vector3 lastPosition;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
      
        float speed = (transform.position - lastPosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPosition = transform.position;

        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}