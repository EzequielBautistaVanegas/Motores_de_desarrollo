using UnityEngine;

public interface IInteractable
{
    string Prompt { get; } // Prompt to display
    Transform PromptAnchor { get; } // Anchor for the prompt
    void Interact();
}