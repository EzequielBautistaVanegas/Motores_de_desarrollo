using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player") || other.GetComponentInParent<Player_Interaction>() != null)
        {
            if (ItemHUD.HasKey)
            {
                _hasTriggered = true;

                if (WinScreenUI.Instance != null)
                {
                    WinScreenUI.Instance.ShowWinScreen();
                }
            }
        }
    }
}