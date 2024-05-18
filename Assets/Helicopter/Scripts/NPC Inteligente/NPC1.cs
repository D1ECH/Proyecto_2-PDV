using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC1 : MonoBehaviour
{
    public GameObject player; // Referencia al jugador
    public GameObject helicopter; // Referencia al helicóptero
    public float pushForce = 10f; // Fuerza con la que el NPC empuja al jugador
    public float detectionRadius = 3f; // Radio de detección alrededor del helicóptero

    private NPC1Conversacion npc1Conversacion;
    // private bool playerConvinced = false;
    private Rigidbody playerRb;

    void Start()
    {
        npc1Conversacion = GetComponent<NPC1Conversacion>();
        playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, helicopter.transform.position) < detectionRadius)
        {
            if (!npc1Conversacion.permiso)
            {
                StartCoroutine(PushPlayer());
            }
        }

        if (!npc1Conversacion.permiso)
        {
            // StartCoroutine(HandleConversation());
        }
    }

    private IEnumerator PushPlayer()
    {
        while (Vector3.Distance(player.transform.position, helicopter.transform.position) < detectionRadius)
        {
            Vector3 pushDirection = (player.transform.position - helicopter.transform.position).normalized;
            playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            yield return new WaitForSeconds(1f);
        }
    }

}
