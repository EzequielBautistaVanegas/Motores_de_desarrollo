using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealthHUD : MonoBehaviour
{
    [SerializeField] private GameObject[] _fullLives;
    [SerializeField] private GameObject[] _emptyLives;
    [SerializeField] private float _flashDuration = 1f;
    [SerializeField] private float _flashInterval = 0.1f;

    private int _currentLives;
    private bool _isAnimating = false;

    public static HealthHUD Instance { get; private set; }
    public static System.Action OnTorchesExtinguished;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _currentLives = _fullLives.Length;

        for (int i = 0; i < _fullLives.Length; i++)
        {
            if (_fullLives[i] != null) _fullLives[i].SetActive(true);
            if (_emptyLives[i] != null) _emptyLives[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            TakeDamage();
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Heal();
        }
    }

    public void TakeDamage()
    {
        if (_currentLives <= 0 || _isAnimating) return;

        _currentLives--;
        StartCoroutine(FlashLife(_currentLives, false));
    }

    public void Heal()
    {
        if (_currentLives >= _fullLives.Length || _isAnimating) return;

        StartCoroutine(FlashLife(_currentLives, true));
        _currentLives++;
    }

    private IEnumerator FlashLife(int index, bool gainingLife)
    {
        _isAnimating = true;

        GameObject full = _fullLives[index];
        GameObject empty = _emptyLives[index];

        float timer = 0f;
        bool showFull = !gainingLife;

        while (timer < _flashDuration)
        {
            timer += _flashInterval;
            showFull = !showFull;

            if (full != null) full.SetActive(showFull);
            if (empty != null) empty.SetActive(!showFull);

            yield return new WaitForSeconds(_flashInterval);
        }

        if (full != null) full.SetActive(gainingLife);
        if (empty != null) empty.SetActive(!gainingLife);

        _isAnimating = false;

        if (_currentLives <= 0)
        {
            OnTorchesExtinguished?.Invoke();
        }
    }
}