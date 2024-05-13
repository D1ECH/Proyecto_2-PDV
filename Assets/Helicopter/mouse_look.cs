using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse_look : MonoBehaviour
{
    public float Sensibilidad = 150; // Sensibilidad del movimiento del rat�n
    public Transform playerBody; // Referencia al cuerpo del jugador (o cualquier objeto que queramos rotar)
    public float xRotacion; // Variable para almacenar la rotaci�n en el eje X

    private void Start()
    {
        // Bloquea el cursor del rat�n en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtiene el movimiento del rat�n en los ejes X e Y
        float mouseX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        // Calcula la rotaci�n de la camara arriba o abajo
        // Limita la rotaci�n para evitar que el jugador gire demasiado hacia arriba o abajo
        xRotacion -= mouseY;
        xRotacion = Mathf.Clamp(xRotacion, -150, 150);

        // Aplica la rotaci�n al objeto actual (la c�mara en este caso)
        transform.localRotation = Quaternion.Euler(xRotacion, 0, 0);

        // Rota el cuerpo del jugador (o cualquier objeto que queramos rotar) en el eje Y
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

