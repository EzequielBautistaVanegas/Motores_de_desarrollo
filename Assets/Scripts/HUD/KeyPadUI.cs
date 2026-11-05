using System.Collections;
using UnityEngine;
using TMPro;

public class KeypadUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject panel;

    public static KeypadUI Instance { get; private set; }

    private string currentInput = "";
    private string targetCode = "";
    private DesafioController currentSafeBox;
    private Movement playerMovement;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (panel != null) panel.SetActive(false);
    }

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<Movement>();
        }
    }

    public void OpenKeypad(DesafioController safeBox, string code)
    {
        currentSafeBox = safeBox;
        targetCode = code;
        currentInput = "";
        UpdateDisplay();

        if (panel != null) panel.SetActive(true);

        if (playerMovement != null)
        {
            playerMovement.SetBusy(true);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PressNumber(string number)
    {
        if (currentInput.Length < 6)
        {
            currentInput += number;
            UpdateDisplay();
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    public void ConfirmCode()
    {
        if (currentInput == targetCode)
        {
            if (currentSafeBox != null)
            {
                currentSafeBox.OnCodeCorrect();
            }
            CloseKeypad();
        }
        else
        {
            currentInput = "ERROR";
            UpdateDisplay();

            if (HealthHUD.Instance != null)
            {
                HealthHUD.Instance.TakeDamage();
            }

            StartCoroutine(ClearInputRoutine());
        }
    }

    private IEnumerator ClearInputRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        ClearInput();
    }

    public void CloseKeypad()
    {
        Time.timeScale = 1f;

        if (panel != null) panel.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.SetBusy(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateDisplay()
    {
        if (displayText != null)
        {
            displayText.text = currentInput;
        }
    }
}