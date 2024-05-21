using UnityEngine;


public class NPCInteligenteBaloncesto : MonoBehaviour
{

    public GameObject jugador;
    public bool lanzar = false;

    private bool conversacion = false;

    void Start()
    {
        if (Vector3.Distance(jugador.transform.position, this.transform.position) < 15f)
        {
            lanzar = true;
        }
    }

    void Update()
    {
        if (Vector3.Distance(jugador.transform.position, this.transform.position) < 15f)
        {
            lanzar = true;
        }
    }
}
