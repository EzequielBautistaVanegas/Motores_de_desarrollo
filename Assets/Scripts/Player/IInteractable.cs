using UnityEngine;

public interface IInteractable
{
    string Prompt { get; } // Prompt to display
    Transform PromptAnchor { get; } // Anchor for the prompt
    string AnimTrigger { get; } // Animation trigger for the interaction
    Transform GetStandPoint(Vector3 playerPos); // Get the stand point for the player to interact
    void Interact();
}