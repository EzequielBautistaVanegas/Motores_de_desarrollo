using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour, IInteractable
{
    public enum RotationAxis { x, y, z }

    /* Configuration Variables */
    [Header("Rotation Configuration")]
    [SerializeField] private float rotationVelocity = 60f;
    [SerializeField] private float maxRotation = 120f;
    [SerializeField] private RotationAxis rotationAxis;
    [SerializeField] private Vector3 startRotation = new Vector3(0f, 0f, 0f);
    [Header("Start Status")]
    [SerializeField] private bool isOpen;
    [Header("Has a Code?")]
    [SerializeField] private List<CodeController> codeNums;

    /* Variables In code */
    private List<int> nums;
    private Transform leftDoor, rightDoor;
    private Quaternion rotationLeftTarget, rotationRightTarget;
    private Vector3 leftVector, rightVector;
    private ConfirmCode confirmCode = new ConfirmCode();
    private string code;

    private void Awake()
    {
        // Get the child transform of the door objects
        leftDoor = transform.Find("LeftDoor");
        rightDoor = transform.Find("RightDoor");
    }

    private void Start()
    {
        // Set the Start Status
        if (!gameObject.CompareTag("SafeBox")) Interact();
        else isOpen = !isOpen; // Toggle the door status
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

    /* Set Where The Doors Will Move */
    public void Interact()
    {
        if (gameObject.CompareTag("SafeBox"))
        {
            code = "345";
            nums = new List<int>();

            foreach (var item in codeNums)
            {
                nums.Add(item.GetNumCode());
            }

            // Check if the code is right
            if (!confirmCode.CodeHandler(nums, code)) return;
        }

        // Set the rotation target
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

        isOpen = !isOpen; // Toggle the door status
    }

    /* Smooth Doors Movement */
    private void Update()
    {
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