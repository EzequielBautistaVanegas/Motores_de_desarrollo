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
        if (fill == null || maxStamina <= 0f) return;

        float staminaPercent = currentStamina / maxStamina;
        fill.fillAmount = staminaPercent;
    }
}