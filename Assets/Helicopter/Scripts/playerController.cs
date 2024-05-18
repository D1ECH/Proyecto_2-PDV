using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public CharacterController cc;  // Referencia al CharacterController que controla al jugador
    public float Velocidad = 12;    // Velocidad de movimiento del jugador (ajustabe desde el inspector)

    public float Gravedad;      // Valor de la gravedad
    public Vector3 velocity;    // Vector de velocidad del jugador

    public Transform groundCheck;       // Punto de comprobaci�n de suelo para verificar si el jugador est� en el suelo
    public float groundDistance = 0.4f; // Distancia de comprobaci�n de suelo
    public LayerMask floorMask;         // M�scara de capa para la detecci�n de suelo
    bool isGrounded;                    // Variable booleana que indica si el jugador est� en el suelo

    Transform tramoAgarrado;
    bool agarrado;
    Vector3 offset;
    void Update()
    {
        // Verifica si el jugador est� en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, floorMask);

        
        // Si el jugador est� en el suelo y la velocidad en y es menor que 0
        if (isGrounded && velocity.y < 0)
        {
            // Resetea la velocidad en y a un valor peque�o para evitar que se hunda en el suelo
            velocity.y = -2f;
        }

        // Si se presiona el bot�n de salto y el jugador est� en el suelo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Calcula la velocidad en y necesaria para realizar un salto
            velocity.y = Mathf.Sqrt(3 * -1 * Gravedad);
        }

        
        // Obtiene los valores de entrada horizontal y vertical para el movimiento
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calcula el vector de movimiento en funci�n de la entrada del jugador y la direcci�n del objeto
        Vector3 move = transform.right * x + transform.forward * z;

        // Mueve al jugador seg�n el vector de movimiento y la velocidad del jugador
        cc.Move(move * Velocidad * Time.deltaTime);

        // Aplica la gravedad al jugador
        velocity.y += Gravedad * Time.deltaTime;

        // Mueve al jugador seg�n la velocidad en y
        cc.Move(velocity * Time.deltaTime);


        if (agarrado)
        {
            //Posicionar y rotar igual al tramo
            cc.transform.position = tramoAgarrado.position + offset;
            cc.transform.rotation = tramoAgarrado.rotation;
            
            if(Input.GetKeyDown(KeyCode.G)) {
                soltarse();
            }
        }
    }

    private void soltarse()
    {
        agarrado = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //1. Que se mueva y rote con la cuerda
            tramoAgarrado = other.transform;
            agarrado = true;
            //2. Suspender la gravedad
            
            //3. Impulso

        }
    }
}
