using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour, IInteractable
{
    public enum RotationAxis { x, y, z }

    [Header("Rotation Configuration")]
    [SerializeField] private float rotationVelocity = 60f;
    [SerializeField] private float maxRotation = 120f;
    [SerializeField] private RotationAxis rotationAxis;
    [SerializeField] private Vector3 startRotation = new Vector3(0f, 0f, 0f);
    [Header("Start Status")]
    [SerializeField] private bool isOpen;
    [Header("Has a Code?")]
    [SerializeField] private List<CodeController> codeNums;
    [Header("Prompt Configuration")]
    [SerializeField] private Transform _promptAnchor;
    [Header("Animation Configuration")]
    [SerializeField] private float interactDelay = 0.4f;
    [SerializeField] private Transform[] _openPoints;
    [SerializeField] private Transform[] _closePoints;

    private List<int> nums;
    private Transform leftDoor, rightDoor;
    private Quaternion rotationLeftTarget, rotationRightTarget;
    private Vector3 leftVector, rightVector;
    private ConfirmCode confirmCode = new ConfirmCode();
    private string code;
    public string Prompt => WillOpen ? "Abrir" : "Cerrar";
    public string AnimTrigger => WillOpen ? "Open" : "Close";
    public Transform PromptAnchor => _promptAnchor != null ? _promptAnchor : transform;
    private float _moveStartTime;
    private bool WillOpen => isOpen;

    public Transform GetStandPoint(Vector3 playerPos)
    {
        Transform[] points = WillOpen ? _openPoints : _closePoints;
        return Closest(points, playerPos);
    }

    private static Transform Closest(Transform[] points, Vector3 from)
    {
        Transform closest = null;
        float best = float.MaxValue;

        foreach (Transform p in points)
        {
            if (p == null) continue;
            float d = (p.position - from).sqrMagnitude;
            if (d < best) { best = d; closest = p; }
        }
        return closest;
    }

    private void Awake()
    {
        leftDoor = transform.Find("LeftDoor");
        rightDoor = transform.Find("RightDoor");
    }

    private void Start()
    {
        if (!gameObject.CompareTag("SafeBox")) Interact();
        else isOpen = !isOpen;
        _moveStartTime = 0f;
    }

    private Vector3 SetAxis(Vector3 vector, RotationAxis axis, float value)
    {
        switch (axis)
        {
            case RotationAxis.x: vector.x = value; break;
            case RotationAxis.y: vector.y = value; break;
            case RotationAxis.z: vector.z = value; break;
        }

        return vector;
    }

    public void Interact(Transform player = null)
    {
        if (gameObject.CompareTag("SafeBox"))
        {
            code = "345";
            nums = new List<int>();

            foreach (var item in codeNums)
            {
                nums.Add(item.GetNumCode());
            }

            if (!confirmCode.CodeHandler(nums, code) || !isOpen) return;

            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        if (leftDoor != null)
        {
            leftVector = !isOpen ? startRotation : SetAxis(startRotation, rotationAxis, -maxRotation);
            rotationLeftTarget = Quaternion.Euler(leftVector);
        }
        if (rightDoor != null)
        {
            rightVector = !isOpen ? startRotation : SetAxis(startRotation, rotationAxis, maxRotation);
            rotationRightTarget = Quaternion.Euler(rightVector);
        }

        _moveStartTime = Time.time + interactDelay;
        isOpen = !isOpen;
    }

    private void Update()
    {
        if (Time.time < _moveStartTime) return;

        if (leftDoor != null)
        {
            leftDoor.localRotation = Quaternion.RotateTowards(
                leftDoor.localRotation,
                rotationLeftTarget,
                rotationVelocity * Time.deltaTime
            );
        }

        if (rightDoor != null)
        {
            rightDoor.localRotation = Quaternion.RotateTowards(
                rightDoor.localRotation,
                rotationRightTarget,
                rotationVelocity * Time.deltaTime
            );
        }
    }
}