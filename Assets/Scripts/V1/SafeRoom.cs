using UnityEngine;

public class SafeRoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _TestEnemy enemy = FindFirstObjectByType<_TestEnemy>();
            if (enemy != null)
            {
                enemy.TriggerReturn();
            }
        }
    }
}