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

    public Transform groundCheck;       // Punto de comprobación de suelo para verificar si el jugador está en el suelo
    public float groundDistance = 0.4f; // Distancia de comprobación de suelo
    public LayerMask floorMask;         // Máscara de capa para la detección de suelo
    bool isGrounded;                    // Variable booleana que indica si el jugador está en el suelo

    Transform tramoAgarrado;
    bool agarrado;
    Vector3 offset;
    float choque = 10;

    private bool stop = false;

    public GameObject esferaPrefab; // Prefab de la esfera
    public float fuerzaMinima = 10f; // Fuerza mínima para lanzar la esfera
    public float fuerzaMaxima = 100f; // Fuerza máxima para lanzar la esfera
    public float fuerzaPorSegundo = 10f; // Incremento de fuerza por segundo mientras se mantiene presionada la tecla
    public KeyCode teclaLanzar = KeyCode.Return; // Tecla para lanzar la esfera


    void Update()
    {
        // Si las teclas de movimiento deben detenerse, no procesar más las entradas del jugador
        if (!stop)
        {
            // Verifica si el jugador está en el suelo
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, floorMask);


            // Si el jugador está en el suelo y la velocidad en y es menor que 0
            if (isGrounded && velocity.y < 0)
            {
                // Resetea la velocidad en y a un valor pequeño para evitar que se hunda en el suelo
                velocity.y = -2f;
            }

            // Si se presiona el botón de salto y el jugador está en el suelo
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                // Calcula la velocidad en y necesaria para realizar un salto
                velocity.y = Mathf.Sqrt(3 * -1 * Gravedad);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(LanzarEsfera());
            }


            // Obtiene los valores de entrada horizontal y vertical para el movimiento
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Calcula el vector de movimiento en función de la entrada del jugador y la dirección del objeto
            Vector3 move = transform.right * x + transform.forward * z;

            // Mueve al jugador según el vector de movimiento y la velocidad del jugador
            cc.Move(move * Velocidad * Time.deltaTime);

            // Aplica la gravedad al jugador
            velocity.y += Gravedad * Time.deltaTime;

            // Mueve al jugador según la velocidad en y
            cc.Move(velocity * Time.deltaTime);


            if (agarrado)
            {
                //Posicionar y rotar igual al tramo
                cc.transform.position = tramoAgarrado.position + offset;
                cc.transform.rotation = tramoAgarrado.rotation;

                if (Input.GetKey(KeyCode.G))
                {
                    soltarse();
                }
            }
        }
    }

    private void soltarse()
    {
        agarrado = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.F))
        {
            //1. Que se mueva y rote con la cuerda
            tramoAgarrado = other.transform;
            agarrado = true;
            //2. Suspender la gravedad

            //3. Impulso
        }
    }

    // Método para activar o desactivar la detención de las teclas de movimiento
    public void SetStop(bool value)
    {
        stop = value;
    }

    
    IEnumerator LanzarEsfera()
    {
        // Calcular la fuerza inicial
        float fuerza = fuerzaMinima;

        // Mientras se mantiene presionada la tecla de lanzar
        while (Input.GetKey(teclaLanzar))
        {
            // Aumentar la fuerza gradualmente
            if (fuerza < fuerzaMaxima)
            {
                fuerza += fuerzaPorSegundo * Time.deltaTime;
            }

            yield return null;
        }

        // Instanciar la esfera y aplicarle una fuerza
        GameObject esfera = Instantiate(esferaPrefab, transform.position + transform.forward, Quaternion.identity);
        Rigidbody rb = esfera.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * fuerza, ForceMode.Impulse);

        // Esperar 5 segundos antes de destruir la esfera
        yield return new WaitForSeconds(5f);

        // Destruir la esfera después de 5 segundos
        Destroy(esfera);
    }
}
