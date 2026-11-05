using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _speed = 12f;

    private Vector3 _baseScale;
    private Vector3 _targetScale;

    private void Awake()
    {
        _baseScale = transform.localScale;
        _targetScale = _baseScale;
    }

    private void OnDisable()
    {
        transform.localScale = _baseScale;
        _targetScale = _baseScale;
    }

    public void OnPointerEnter(PointerEventData eventData) => _targetScale = _baseScale * _hoverScale;
    public void OnPointerExit(PointerEventData eventData) => _targetScale = _baseScale;

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale, _targetScale, Time.unscaledDeltaTime * _speed);
    }
}