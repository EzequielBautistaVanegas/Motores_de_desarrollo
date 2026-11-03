using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject _promptObject;
    [SerializeField] private TMP_Text _promptText;
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector2 _screenOffset = new Vector2(0f, 40f);

    private RectTransform _promptRect;
    private IInteractable _target;

    private void Awake() => _promptRect = _promptObject.GetComponent<RectTransform>();

    private void OnEnable() => Interaction.OnTargetChanged += UpdatePrompt;
    private void OnDisable() => Interaction.OnTargetChanged -= UpdatePrompt;

    private void Start() => _promptObject.SetActive(false);

    private void UpdatePrompt(IInteractable target)
    {
        _target = target;

        if (_target == null)
        {
            _promptObject.SetActive(false);
            return;
        }

        string key = _interactAction.action.GetBindingDisplayString();
        _promptText.text = $"[{key}] {_target.Prompt}";
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 screenPos = _camera.WorldToScreenPoint(_target.PromptAnchor.position);

        // Hide if the object is behind the camera
        bool visible = screenPos.z > 0f;
        _promptObject.SetActive(visible);
        if (!visible) return;

        _promptRect.position = (Vector2)screenPos + _screenOffset;
    }
}
