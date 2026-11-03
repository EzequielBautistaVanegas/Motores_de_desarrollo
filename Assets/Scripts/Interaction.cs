using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _interactionDistance = 2f;
    [SerializeField] private float _aimRadius = 0.2f;
    [SerializeField] private LayerMask _interactiveLayer;
    [SerializeField] private Movement _movement;

    private IInteractable _current;
    private IInteractable _active;
    public static event Action<IInteractable> OnTargetChanged;

    private void Update()
    {
        IInteractable found = FindInteractable();

        if (found != _current)
        {
            _current = found;
            OnTargetChanged?.Invoke(_current);
        }
    }

    private IInteractable FindInteractable()
    {
        if (!_movement.CanMove) return null;

        Ray ray = _camera.ViewportPointToRay(new Vector3(0.3f, 0.5f, 0f));

        // Add the camera to player distance.
        float camToPlayer = Vector3.Distance(_camera.transform.position, transform.position);
        float maxDist = camToPlayer + _interactionDistance;

        Debug.DrawRay(ray.origin, ray.direction * maxDist, Color.red);

        if (!Physics.SphereCast(ray, _aimRadius, out RaycastHit hit, maxDist,
                                _interactiveLayer, QueryTriggerInteraction.Collide))
            return null;

        // Verify if the interactable is really close to the player, not just to the camera
        if (Vector3.Distance(transform.position, hit.point) > _interactionDistance)
            return null;

        return hit.collider.GetComponentInParent<IInteractable>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        IInteractable target = _active ?? _current;
        if (target == null) return;

        target?.Interact();
        _active = _movement.CanMove ? null : target;
    }
}