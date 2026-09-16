using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleDoorController : MonoBehaviour
{
    [Header("Puertas")]
    [SerializeField] private Transform simpleDoor;  /*SerializeFiedl permite modificar cosas privadas en el inspector*/
   

    [Header("Ángulos de apertura")]
    [SerializeField] private Vector3 openRotation = new Vector3(0f, 0f, 0f);
   

    [Header("Configuración")]
    [SerializeField] private InputActionReference interactionAction;
    [SerializeField] private float rotationSpeed = 60f;

    private Quaternion closedRotation; /*Quaternion  lo utilizamos para mejorar el sistema de rotaciones/giros*/
     /*En este caso guardamos las rotaciones iniciales, si importar el angulo de inicio */

    private Quaternion targetRotation; /* Variable creada para indicar hacia donde giran las puertas */
    

    private bool isOpen; /*variable para indicar si la puerta se encuntra abierta o cerrada, al no asignarle valor, tomar como valor inicial FALSE*/

    private bool playerNearby = false;

    private void Start()
    {
        closedRotation = simpleDoor.localRotation; /* Carga la rotacion inicial como "cerrado" utilizando el motodo .localRotation perteneciente a leftDoor */
       

        targetRotation = closedRotation; /* se asigna la posicion inicial de cerrada para despues poder indicar hacia donde va rotar */
        
    }

    private void Update()
    {
        if (playerNearby && interactionAction != null && interactionAction.action.WasPressedThisFrame()) 
        {
            ToggleDoor();
        }

        simpleDoor.localRotation = Quaternion.RotateTowards( /* este metodo hace que las rotaciones se acerquen progresivamente hacia otra */
            simpleDoor.localRotation, /* indica la rotacion acutal*/
            targetRotation, /*indica hacia donde va a rotar*/
            rotationSpeed * Time.deltaTime /*indica la velocidad de rotacion, y se estabiliza con el metodo .deltaTime*/
        );

     
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen; /* La puerta inicia en posicion cerrada, cuando ingresamos a la funcion, pasa primero a posicion abierta y luego se ejecuta la abertura de la misma*/

        if (isOpen) /*Como ahora la puerta cambia de estado de cerrada a abierta, se le indica la nueva posicion hacia donde tiene que ir que es targetRotation*/
        {
            targetRotation = /* para indicar la nueva posicion, multiplica la posicion inicial estando cerrada " closeRotation" por la variable de apertura "openRotation"*/
                closedRotation * Quaternion.Euler(openRotation);

            
        }
        else /* Si la puerta esta abierta,y se se activa la opcion de cerrar, "targetRotation" vuelve a la posicion de inicio */
        {
            targetRotation = closedRotation;
            
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