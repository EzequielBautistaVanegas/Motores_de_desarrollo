using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UnityEvent onDeath;

    public bool IsDead { get; private set; }

    public void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        Debug.Log("Player died.");
        onDeath?.Invoke();
    }
}
