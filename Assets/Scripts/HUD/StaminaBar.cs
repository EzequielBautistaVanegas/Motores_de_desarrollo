using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image fill;

    private void OnEnable()
    {
        Movement.OnStaminaChanged += UpdateStamina;
    }

    private void OnDisable()
    {
        Movement.OnStaminaChanged -= UpdateStamina;
    }

    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        float staminaPercent = currentStamina / maxStamina;
        fill.fillAmount = staminaPercent;
        Debug.Log($"Stamina updated: {currentStamina}/{maxStamina} ({staminaPercent * 100f}%)");
    }
}
