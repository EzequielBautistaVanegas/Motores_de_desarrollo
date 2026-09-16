using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private Light oilLamp;
    [SerializeField] private PlayerInputHandler input;

    [Header("Aceite")]
    [SerializeField, Min(0.01f)] private float maxOil = 100f;
    [SerializeField, Min(0f)] private float startingOil = 100f;
    [SerializeField, Min(0f)] private float consumptionPerSecond = 1f;

    private float currentOil;
    private bool lightOn;

    public float CurrentOil => currentOil;
    public float MaxOil => maxOil;
    public float OilPercentage => maxOil > 0f ? currentOil / maxOil : 0f;

    private void Start()
    {
        currentOil = Mathf.Clamp(startingOil, 0f, maxOil);
        SetLight(currentOil > 0f);
    }

    private void OnEnable()
    {
        if (input != null)
        {
            input.ToggleLampPressed += ToggleLight;
        }
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.ToggleLampPressed -= ToggleLight;
        }
    }

    private void Update()
    {
        if (!lightOn)
        {
            return;
        }

        currentOil -= consumptionPerSecond * Time.deltaTime;

        if (currentOil <= 0f)
        {
            currentOil = 0f;
            SetLight(false);
        }
    }

    private void ToggleLight()
    {
        if (lightOn)
        {
            SetLight(false);
            return;
        }

        if (currentOil > 0f)
        {
            SetLight(true);
        }
    }

    private void SetLight(bool state)
    {
        lightOn = state && currentOil > 0f;

        if (oilLamp != null)
        {
            oilLamp.enabled = lightOn;
        }
    }

    public bool IsLightOn()
    {
        return lightOn;
    }

    public void ForceLightOff()
    {
        SetLight(false);
    }

    public bool AddOil(float amount)
    {
        if (amount <= 0f || currentOil >= maxOil)
        {
            return false;
        }

        currentOil = Mathf.Clamp(currentOil + amount, 0f, maxOil);
        return true;
    }
}
