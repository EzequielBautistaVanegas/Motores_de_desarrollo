using UnityEngine;

public class StandPoint : MonoBehaviour
{
    public enum Hand { Left, Right }

    [SerializeField] private Hand _hand = Hand.Left;
    public Hand UseHand => _hand;

    // Blue for left hand, Red for right hand
    private void OnDrawGizmos()
    {
        Gizmos.color = _hand == Hand.Left ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.15f);
        Gizmos.DrawRay(transform.position, transform.forward * 0.4f);
    }
}