using System;
using UnityEngine;

public class BoardController : MonoBehaviour, IInteractable
{
    public static Action OnBoardReading;
    [SerializeField] private string _prompt = "Leer";
    [SerializeField] private Transform _promptAnchor;

    public string Prompt => _prompt;
    public string AnimTrigger => null;
    public Transform PromptAnchor => _promptAnchor != null ? _promptAnchor : transform;
    public Transform GetStandPoint(Vector3 playerPos) => null;

    public void Interact(Transform player = null)
    {
        OnBoardReading?.Invoke();
    }
}