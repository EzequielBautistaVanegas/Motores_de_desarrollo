using UnityEngine;
using UnityEngine.Events;

public class SequencePuzzle : MonoBehaviour
{
    [SerializeField] private int[] correctSequence;
    [SerializeField] private UnityEvent onSolved;
    [SerializeField] private UnityEvent onMistake;

    private int currentIndex;
    private bool solved;

    public bool IsSolved => solved;

    public void Press(int id)
    {
        if (solved || correctSequence == null || correctSequence.Length == 0)
        {
            return;
        }

        if (id == correctSequence[currentIndex])
        {
            currentIndex++;

            if (currentIndex >= correctSequence.Length)
            {
                Solve();
            }

            return;
        }

        currentIndex = 0;
        onMistake?.Invoke();

        if (id == correctSequence[0])
        {
            currentIndex = 1;
        }
    }

    private void Solve()
    {
        solved = true;
        currentIndex = 0;

        Debug.Log("Sequence puzzle solved");
        onSolved?.Invoke();
    }
}
