using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Acciones")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string lookActionName = "Look";
    [SerializeField] private string interactActionName = "Interact";
    [SerializeField] private string useItemActionName = "UseItem";
    [SerializeField] private string toggleLampActionName = "ToggleLamp";
    [SerializeField] private string nextItemActionName = "NextItem";

    private PlayerInput playerInput;
    private InputActionMap playerMap;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interactAction;
    private InputAction useItemAction;
    private InputAction toggleLampAction;
    private InputAction nextItemAction;

    public Vector2 MoveInput => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    public Vector2 LookInput => lookAction?.ReadValue<Vector2>() ?? Vector2.zero;

    public event Action InteractPressed;
    public event Action UseItemPressed;
    public event Action ToggleLampPressed;
    public event Action NextItemPressed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        playerMap = playerInput.actions.FindActionMap(actionMapName, true);

        moveAction = playerMap.FindAction(moveActionName, true);
        lookAction = playerMap.FindAction(lookActionName, true);
        interactAction = playerMap.FindAction(interactActionName, true);
        useItemAction = playerMap.FindAction(useItemActionName, true);
        toggleLampAction = playerMap.FindAction(toggleLampActionName, true);
        nextItemAction = playerMap.FindAction(nextItemActionName, true);
    }

    private void OnEnable()
    {
        playerMap.Enable();

        interactAction.performed += OnInteractPerformed;
        useItemAction.performed += OnUseItemPerformed;
        toggleLampAction.performed += OnToggleLampPerformed;
        nextItemAction.performed += OnNextItemPerformed;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteractPerformed;
        useItemAction.performed -= OnUseItemPerformed;
        toggleLampAction.performed -= OnToggleLampPerformed;
        nextItemAction.performed -= OnNextItemPerformed;

        playerMap.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        InteractPressed?.Invoke();
    }

    private void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        UseItemPressed?.Invoke();
    }

    private void OnToggleLampPerformed(InputAction.CallbackContext context)
    {
        ToggleLampPressed?.Invoke();
    }

    private void OnNextItemPerformed(InputAction.CallbackContext context)
    {
        NextItemPressed?.Invoke();
    }
}
