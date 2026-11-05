using UnityEngine;

public interface IInteractable
{
    string Prompt { get; }
    Transform PromptAnchor { get; }
    string AnimTrigger { get; }
    Transform GetStandPoint(Vector3 playerPos);
    void Interact(Transform player = null); 
}