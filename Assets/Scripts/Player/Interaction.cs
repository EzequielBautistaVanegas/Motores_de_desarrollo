using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    public static event Action<string, bool> OnInteractAnimation;

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
        if (!context.performed || _movement.IsBusy) return;

        IInteractable target = _active ?? _current;
        if (target == null) return;

        Transform stand = target.GetStandPoint(transform.position);

        if (stand != null && !string.IsNullOrEmpty(target.AnimTrigger))
            StartCoroutine(InteractRoutine(target, stand));
        else
            DoInteract(target);
    }

    private IEnumerator InteractRoutine(IInteractable target, Transform stand)
    {
        _movement.SetBusy(true);
        yield return _movement.MoveTo(stand.position, target.PromptAnchor.position);
        DoInteract(target, stand);
    }

    private void DoInteract(IInteractable target, Transform stand = null)
    {
        string trigger = target.AnimTrigger;

        StandPoint point = stand != null ? stand.GetComponent<StandPoint>() : null;
        bool mirror = point != null && point.UseHand == StandPoint.Hand.Right;

        target.Interact();
        _active = _movement.CanMove ? null : target;

        if (!string.IsNullOrEmpty(trigger)) OnInteractAnimation?.Invoke(trigger, mirror);
        if (_current != null) OnTargetChanged?.Invoke(_current);
    }
}