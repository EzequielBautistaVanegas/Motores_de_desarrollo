using System;
using UnityEngine;

public class BoardController : MonoBehaviour, IInteractable
{
    public static Action OnBoardReading;
    [SerializeField] private string _prompt = "Leer";
    [SerializeField] private Transform _promptAnchor;
    public string Prompt => _prompt;
    public Transform PromptAnchor => _promptAnchor != null ? _promptAnchor : transform;

    public void Interact()
    {
        // Toggle the board's active state
        OnBoardReading?.Invoke();
    }
}
