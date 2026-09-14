using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField] private PlayerLight playerLight;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField, Min(0f)] private float attackCooldown = 4f;

    private float nextAttackTime;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || Time.time < nextAttackTime)
        {
            return;
        }

        Attack();
        nextAttackTime = Time.time + attackCooldown;
    }

    private void Attack()
    {
        if (playerLight == null)
        {
            Debug.LogError("MonsterAttack: PlayerLight falta", this);
            return;
        }

        if (!playerLight.IsLightOn())
        {
            if (playerHealth != null)
            {
                playerHealth.Die();
            }

            return;
        }

        playerLight.ForceLightOff();
    }
}
