using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Importar el namespace de SceneManagement para cambiar de escena

public class montarse_helicoptero : MonoBehaviour
{
    public float rangoVision;
    public LayerMask capaJugador;
    public Transform jugador;

    bool estarAlerta;

    // Referencia al canvas de mensaje
    public GameObject mensajeCanvas;
    // Variable para controlar si el mensaje está mostrándose
    private bool mensajeMostrado = false;

    // Tecla para cambiar de escena
    public KeyCode teclaCambioEscena = KeyCode.F;
    // Nombre de la escena a la que cambiar
    public string nombreEscenaACambiar = "NombreDeTuEscena";

    // Start is called before the first frame update
    void Start()
    {
        // Desactivar el canvas de mensaje al inicio
        mensajeCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Verificar si el jugador está dentro del rango de visión
        estarAlerta = Physics.CheckSphere(transform.position, rangoVision, capaJugador);

        // Si el jugador está dentro del rango y el mensaje no se ha mostrado
        if (estarAlerta && !mensajeMostrado)
        {
            // Mostrar el mensaje en pantalla
            mensajeCanvas.SetActive(true);
            mensajeMostrado = true;
        }
        // Si el jugador sale del rango y el mensaje se ha mostrado
        else if (!estarAlerta && mensajeMostrado)
        {
            // Ocultar el mensaje en pantalla
            mensajeCanvas.SetActive(false);
            mensajeMostrado = false;
        }

        // Si el mensaje está mostrándose y el jugador pulsa la tecla de cambio de escena
        if (mensajeMostrado && Input.GetKeyDown(teclaCambioEscena))
        {
            // Cambiar a la escena especificada
            SceneManager.LoadScene(nombreEscenaACambiar);

            // Restablecer la captura del cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar un gizmo esférico para representar el rango de visión
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
    }
}

