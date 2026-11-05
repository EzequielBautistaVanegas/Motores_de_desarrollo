using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LampOilHUD : MonoBehaviour
{
    [Header("Parametros")]
    [SerializeField] private Image _oilFillImage;
    [SerializeField] private float _maxOil = 100f;
    [SerializeField] private float _drainRate = 0.5f;

    private float _currentOil;
    private bool _isLampEquipped;
    private bool _hasTriggeredChase;

    public static LampOilHUD Instance { get; private set; }

    public float CurrentOil => _currentOil;
    public float MaxOil => _maxOil;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _currentOil = _maxOil;
        _hasTriggeredChase = false;
        UpdateUI();
    }

    private void Update()
    {
        if (_isLampEquipped)
        {
            ConsumeOil(_drainRate * Time.deltaTime);
        }

        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            RefillCompletely();
        }
    }

    public void ConsumeOil(float amount)
    {
        if (_currentOil <= 0f) return;

        _currentOil -= amount;
        _currentOil = Mathf.Clamp(_currentOil, 0f, _maxOil);
        UpdateUI();

        if (_currentOil <= 0f && !_hasTriggeredChase)
        {
            _hasTriggeredChase = true;
            HealthHUD.OnTorchesExtinguished?.Invoke();
        }
    }

    public void AddOil(float amount)
    {
        _currentOil += amount;
        _currentOil = Mathf.Clamp(_currentOil, 0f, _maxOil);
        if (_currentOil > 0f)
        {
            _hasTriggeredChase = false;
        }
        UpdateUI();
    }

    public void RefillCompletely()
    {
        _currentOil = _maxOil;
        _hasTriggeredChase = false;
        UpdateUI();
    }

    public void SetLampState(bool isEquipped)
    {
        _isLampEquipped = isEquipped;
    }

    private void UpdateUI()
    {
        if (_oilFillImage != null && _maxOil > 0f)
        {
            _oilFillImage.fillAmount = _currentOil / _maxOil;
        }
    }
}