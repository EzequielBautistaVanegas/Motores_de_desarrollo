using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleDoorController : MonoBehaviour
{
    [Header("Puertas")]
    [SerializeField] private Transform leftDoor;  /*SerializeFiedl permite modificar cosas privadas en el inspector*/
    [SerializeField] private Transform rightDoor;

    [Header("Ángulos de apertura")]
    [SerializeField] private Vector3 leftOpenRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 rightOpenRotation = new Vector3(0f, 0f, 0f);

    [Header("Configuración")]
    [SerializeField] private InputActionReference interactionAction;
    [SerializeField] private float rotationSpeed = 60f;

    private Quaternion leftClosedRotation; /*Quaternion no lo utilizamos para mejorar el sistema de rotaciones/giros*/
    private Quaternion rightClosedRotation; /*En este caso guardamos las rotaciones iniciales, si importar el angulo de inicio */

    private Quaternion leftTargetRotation; /* Variable creada para indicar hacia donde giran las puertas */
    private Quaternion rightTargetRotation;

    private bool isOpen; /*variable para indicar si la puerta se encuntra abierta o cerrada, al no asignarle valor, tomar como valor inicial FALSE*/

    private bool playerNearby = false; /* El boleano cambiara a true cuando el player colisione con un area cercana a la puerta*/

    private void Start()
    {
        leftClosedRotation = leftDoor.localRotation; /* Carga la rotacion inicial como "cerrado" utilizando el motodo .localRotation perteneciente a leftDoor */
        rightClosedRotation = rightDoor.localRotation;

        leftTargetRotation = leftClosedRotation; /* se asigna la posicion inicial de cerrada para despues poder indicar hacia donde va rotar */
        rightTargetRotation = rightClosedRotation;
    }

    private void Update()
    {
        if (playerNearby && interactionAction != null && interactionAction.action.WasPressedThisFrame())
         {
            ToggleDoor();
        }

        leftDoor.localRotation = Quaternion.RotateTowards( /* este metodo hace que las rotaciones se acerquen progresivamente hacia otra */
            leftDoor.localRotation, /* indica la rotacion acutal*/
            leftTargetRotation, /*indica hacia donde va a rotar*/
            rotationSpeed * Time.deltaTime /*indica la velocidad de rotacion, y se estabiliza con el metodo .deltaTime*/
        );

        rightDoor.localRotation = Quaternion.RotateTowards( /* idem para la otra puerta, que cada puerta es un objeto hijo o independiente dentro de un objeto padre*/
            rightDoor.localRotation,
            rightTargetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen; /* La puerta inicia en posicion cerrada, cuando ingresamos a la funcion, pasa primero a posicion abierta y luego se ejecuta la abertura de la misma*/

        if (isOpen) /*Como ahora la puerta cambia de estado de cerrada a abierta, se le indica la nueva posicion hacia donde tiene que ir que es targetRotation*/
        {
            leftTargetRotation = /* para indicar la nueva posicion, multiplica la posicion inicial estando cerrada " closeRotation" por la variable de apertura "openRotation"*/
                leftClosedRotation * Quaternion.Euler(leftOpenRotation);

            rightTargetRotation =
                rightClosedRotation * Quaternion.Euler(rightOpenRotation);
        }
        else /* Si la puerta esta abierta,y se se activa la opcion de cerrar, "targetRotation" vuelve a la posicion de inicio */
        {
            leftTargetRotation = leftClosedRotation;
            rightTargetRotation = rightClosedRotation;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}