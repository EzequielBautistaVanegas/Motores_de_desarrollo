using UnityEngine;

public class LampPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string animTrigger = "";
    [SerializeField] private string prompt = "Agarrar Lámpara";

    public string AnimTrigger => animTrigger;
    public string Prompt => prompt;
    public Transform PromptAnchor => transform;

    public bool CanInteract(Transform interactor)
    {
        return true;
    }

    public void Interact(Transform interactor)
    {
        Player_Interaction player = interactor.GetComponent<Player_Interaction>();
        if (player != null)
        {
            player.EquipLamp();
        }

        Destroy(gameObject);
    }

    public Transform GetStandPoint(Vector3 playerPosition)
    {
        return null;
    }
}