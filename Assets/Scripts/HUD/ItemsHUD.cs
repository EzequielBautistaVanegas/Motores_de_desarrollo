using UnityEngine;
using UnityEngine.InputSystem;

public class ItemHUD : MonoBehaviour
{
    public enum ItemType { Lamp, Key }

    [Header("Parametros")]
    [SerializeField] private GameObject _lampIcon;
    [SerializeField] private GameObject _keyIcon;

    public static ItemHUD Instance { get; private set; }
    public static bool HasKey { get; private set; }
    public static bool HasLamp { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        HasKey = false;
        HasLamp = false;
        if (_lampIcon != null) _lampIcon.SetActive(false);
        if (_keyIcon != null) _keyIcon.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            UnlockItem(ItemType.Key);
        }
    }

    public void UnlockItem(ItemType type)
    {
        if (type == ItemType.Lamp)
        {
            HasLamp = true;
            if (_lampIcon != null) _lampIcon.SetActive(true);
        }
        else if (type == ItemType.Key)
        {
            HasKey = true;
            if (_keyIcon != null) _keyIcon.SetActive(true);
        }
    }
}