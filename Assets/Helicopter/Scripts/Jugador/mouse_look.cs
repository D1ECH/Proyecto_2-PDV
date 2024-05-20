using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse_look : MonoBehaviour
{
    public float Sensibilidad = 150; // Sensibilidad del movimiento del ratón
    public Transform playerBody; // Referencia al cuerpo del jugador (o cualquier objeto que queramos rotar)
    public float xRotacion; // Variable para almacenar la rotación en el eje X

    private void Start()
    {
        // Bloquea el cursor del ratón en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtiene el movimiento del ratón en los ejes X e Y
        float mouseX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        // Calcula la rotación de la camara arriba o abajo
        // Limita la rotación para evitar que el jugador gire demasiado hacia arriba o abajo
        xRotacion -= mouseY;
        xRotacion = Mathf.Clamp(xRotacion, -150, 150);

        // Aplica la rotación al objeto actual (la cámara en este caso)
        transform.localRotation = Quaternion.Euler(xRotacion, 0, 0);

        // Rota el cuerpo del jugador (o cualquier objeto que queramos rotar) en el eje Y
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

