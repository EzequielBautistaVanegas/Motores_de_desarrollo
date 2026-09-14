using UnityEngine;
using UnityEngine.Events;

public class FinalDoor : InteractableBase
{
    [Header("Llaves")]
    [SerializeField] private ItemData[] requiredKeys;
    [SerializeField] private bool consumeKeys;

    [Header("Puertas")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Rotación de apertura")]
    [SerializeField] private Vector3 leftOpenRotation;
    [SerializeField] private Vector3 rightOpenRotation;

    [Header("Velocidad")]
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Victoria")]
    [SerializeField] private UnityEvent onVictory;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;

    private Quaternion leftTargetRotation;
    private Quaternion rightTargetRotation;

    private bool opened;

    private void Start()
    {
        // Guarda la rotación inicial de cada hoja.
        // Esa posición se considera "cerrada".
        leftClosedRotation = leftDoor.localRotation;
        rightClosedRotation = rightDoor.localRotation;

        // Al comenzar, el objetivo también es la posición cerrada.
        leftTargetRotation = leftClosedRotation;
        rightTargetRotation = rightClosedRotation;
    }

    private void Update()
    {
        // Mueve suavemente la puerta izquierda hacia su rotación objetivo.
        leftDoor.localRotation = Quaternion.RotateTowards(
            leftDoor.localRotation,
            leftTargetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Hace lo mismo con la puerta derecha.
        rightDoor.localRotation = Quaternion.RotateTowards(
            rightDoor.localRotation,
            rightTargetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public override void Interact(PlayerInteractor player)
    {
        if (opened || player == null || player.Inventory == null)
        {
            return;
        }

        if (requiredKeys == null || requiredKeys.Length == 0)
        {
            Debug.LogWarning("FinalDoor no tiene llave asignada");
            return;
        }

        foreach (ItemData key in requiredKeys)
        {
            if (key == null || !player.Inventory.HasItem(key))
            {
                Debug.Log("Necesitas mas llaves");
                return;
            }
        }

        if (consumeKeys)
        {
            foreach (ItemData key in requiredKeys)
            {
                player.Inventory.RemoveItem(key, 1);
            }
        }

        opened = true;

        OpenDoor();

        Debug.Log("Victoria: todas las llaves obtenidas");

        onVictory?.Invoke();
    }

    private void OpenDoor()
    {
        // Calcula la rotación final de cada hoja
        // usando como base su rotación cerrada inicial.
        leftTargetRotation =
            leftClosedRotation *
            Quaternion.Euler(leftOpenRotation);

        rightTargetRotation =
            rightClosedRotation *
            Quaternion.Euler(rightOpenRotation);
    }
}