using UnityEngine;
using UnityEngine.Events;

public class LeverPuzzle : MonoBehaviour
{
    [SerializeField] private LeverInteractable[] levers;
    [SerializeField] private bool[] correctStates;
    [SerializeField] private UnityEvent onSolved;

    private bool solved;

    public bool IsSolved => solved;

    public void CheckSolution()
    {
        if (solved || levers == null || correctStates == null || levers.Length == 0 || levers.Length != correctStates.Length)
        {
            return;
        }

        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i] == null || levers[i].IsOn != correctStates[i])
            {
                return;
            }
        }

        solved = true;
        Debug.Log("Puzzle completado");
        onSolved?.Invoke();
    }
}
