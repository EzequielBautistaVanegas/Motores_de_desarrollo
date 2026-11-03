using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] private float _headHeight = 1.6f;
    [SerializeField] private float _radius = 0.2f;
    [SerializeField] private float _smooth = 10f;
    [SerializeField] private LayerMask _collisionMask;

    private Transform _player;
    private Vector3 _defaultLocalPos;
    private float _currentT = 1f;

    private void Awake()
    {
        _player = transform.parent;
        _defaultLocalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        Vector3 origin = _player.position + Vector3.up * _headHeight;
        Vector3 desired = _player.TransformPoint(_defaultLocalPos);

        Vector3 toCam = desired - origin;
        float maxDist = toCam.magnitude;
        float targetT = 1f;

        if (Physics.SphereCast(origin, _radius, toCam.normalized, out RaycastHit hit,
                               maxDist, _collisionMask, QueryTriggerInteraction.Ignore))
        {
            targetT = hit.distance / maxDist;
        }

        // Se acerca al instante y se aleja suave
        _currentT = targetT < _currentT
            ? targetT
            : Mathf.Lerp(_currentT, targetT, Time.deltaTime * _smooth);

        transform.position = origin + toCam * _currentT;
    }
}