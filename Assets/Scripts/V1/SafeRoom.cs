using UnityEngine;

public class SafeRoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _TestEnemy enemy = FindObjectOfType<_TestEnemy>();
            if (enemy != null)
            {
                enemy.TriggerReturn();
            }
        }
    }
}