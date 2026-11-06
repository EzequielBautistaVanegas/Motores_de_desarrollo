using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DesafioController : MonoBehaviour, IInteractable
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
    [SerializeField] private string requiredCode = "351";
    [SerializeField] private bool unlockItemOnOpen = true;

    [Header("Evento Final / Persecución")]
    [SerializeField] private bool triggerEnemyChaseOnOpen = true;
    [SerializeField] private GameObject safeRoomObject;

    [Header("Prompt Configuration")]
    [SerializeField] private Transform _promptAnchor;

    [Header("Animation Configuration")]
    [SerializeField] private float interactDelay = 0.4f;
    [SerializeField] private Transform[] _openPoints;
    [SerializeField] private Transform[] _closePoints;

    private Transform leftDoor, rightDoor;
    private Quaternion rotationLeftTarget, rotationRightTarget;
    private Vector3 leftVector, rightVector;
    private float _moveStartTime;
    private bool isUnlocked = false;

    public string Prompt => isUnlocked ? (WillOpen ? "Abrir" : "Cerrar") : "Ingresar código";
    public string AnimTrigger => WillOpen ? "Open" : "Close";
    public Transform PromptAnchor => _promptAnchor != null ? _promptAnchor : transform;
    private bool WillOpen => isOpen;

    public bool CanInteract(Transform interactor)
    {
        return true;
    }

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
        if (!gameObject.CompareTag("SafeBox"))
        {
            isUnlocked = true;
            OpenDoors();
        }
        else
        {
            isOpen = !isOpen;
        }
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
        if (gameObject.CompareTag("SafeBox") && !isUnlocked)
        {
            if (KeypadUI.Instance != null)
            {
                KeypadUI.Instance.OpenKeypad(this, requiredCode);
            }
            return;
        }

        OpenDoors();
    }

    public void OnCodeCorrect()
    {
        isUnlocked = true;

        if (unlockItemOnOpen && ItemHUD.Instance != null)
        {
            ItemHUD.Instance.UnlockItem(ItemHUD.ItemType.Key);
        }

        if (safeRoomObject != null)
        {
            safeRoomObject.SetActive(false);
        }

        if (triggerEnemyChaseOnOpen)
        {
            _TestEnemy enemy = FindFirstObjectByType<_TestEnemy>();
            if (enemy != null)
            {
                enemy.TriggerChase();
            }
        }

        gameObject.layer = LayerMask.NameToLayer("Default");
        OpenDoors();
    }

    private void OpenDoors()
    {
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