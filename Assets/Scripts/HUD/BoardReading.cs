using UnityEngine;
using UnityEngine.Rendering.UI;

public class BoardReading : MonoBehaviour
{
    [SerializeField] private GameObject board;
    [SerializeField] private Movement playerMovement;

    private void OnEnable() => BoardController.OnBoardReading += ShowBoard;
    private void OnDisable() => BoardController.OnBoardReading -= ShowBoard;

    private void ShowBoard()
    {
        bool open = !board.activeSelf;
        board.SetActive(open);
        playerMovement.SetCanMove(!open);
    }
}
