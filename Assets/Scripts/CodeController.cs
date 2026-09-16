using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CodeController : MonoBehaviour, IInteractable
{
    /* Configuration Variables */
    [SerializeField] private float rotationVelocity = 60f;
    /* Variables In Code */
    private int numCode;
    private float gradesTarget;
    private Transform pivot;
    private Quaternion rotationTarget;

    private void Awake()
    {
        pivot = transform.parent;
    }

    public void Start()
    {
        numCode = 0;
        Interact();
    }

    public void Interact()
    {
        numCode += numCode < 6 ? 1 : -5;
        
        switch (numCode)
        {
            case 1: gradesTarget = 90f; break;
            case 2: gradesTarget = 146f; break;
            case 3: gradesTarget = 209.5f; break;
            case 4: gradesTarget = 277.5f; break;
            case 5: gradesTarget = 345f; break;
            case 6: gradesTarget = 35f; break;
        }

        rotationTarget = Quaternion.Euler(new Vector3(0f, gradesTarget, 0f));
    }

    public void Update()
    {
        pivot.localRotation = Quaternion.RotateTowards(
            pivot.localRotation,
            rotationTarget,
            rotationVelocity * Time.deltaTime
        );
    }

    public int GetNumCode() => numCode;
}
