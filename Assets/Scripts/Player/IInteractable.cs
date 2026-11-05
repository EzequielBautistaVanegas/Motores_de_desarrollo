using UnityEngine;

public interface IInteractable
{
    string Prompt { get; }
    Transform PromptAnchor { get; }
    string AnimTrigger { get; }                 // null o "" = sin animación
    Transform GetStandPoint(Vector3 playerPos); // null = interactuar sin acercarse
    void Interact(Transform player = null);

    // ¿Se puede usar ahora? (trabada, sin llave, lado equivocado...)
    // Tiene implementación por defecto: los scripts que no la definan devuelven true.
    bool CanInteract(Transform player) => true;
}