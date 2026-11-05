using UnityEngine;

public class StandPoint : MonoBehaviour
{
    public enum Hand { Left, Right }

    [SerializeField] private Hand _hand = Hand.Left;
    public Hand UseHand => _hand;

    // Para verlo en la escena: azul = izquierda, rojo = derecha
    private void OnDrawGizmos()
    {
        Gizmos.color = _hand == Hand.Left ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.15f);
        Gizmos.DrawRay(transform.position, transform.forward * 0.4f);
    }
}