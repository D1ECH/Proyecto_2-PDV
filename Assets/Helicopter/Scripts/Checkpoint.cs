using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Action CheckpointActivated;

    // Tag del personaje
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto que entra en el checkpoint es el personaje
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Activate");
            // Llamar al evento del checkpoint activado solo si es el personaje
            CheckpointActivated?.Invoke();
        }
    }
}

