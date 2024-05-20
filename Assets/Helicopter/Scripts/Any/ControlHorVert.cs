using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlHorVert : MonoBehaviour{
    private float fuerzaLevitacion;
    private Rigidbody rb;
    public GameObject ObjetoPerseguido;
    public float RapidezVertical = 0.4f;
    public float RapidezHorizontal = 30f;
    public float AlturaDeseada = 20;
    public float SeparacionConObjetivo = 40;
    public bool Caer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1000;
        fuerzaLevitacion = -(rb.mass * Physics.gravity.y);
    }

    void FixedUpdate()
    {
        if (!Caer)
        {
            AlcanzarAltura(AlturaDeseada, RapidezVertical);
            AlcanzarPosicion(ObjetoPerseguido.transform.position - SeparacionConObjetivo * ObjetoPerseguido.transform.forward, RapidezHorizontal, 1);
        }
    }

    private void AlcanzarAltura(float alturaObjetivo, float rapidezVertical)
    {
        float distancia = (alturaObjetivo - transform.position.y);
        if (rb.velocity.y >= 0f)
        {
            float factor = distancia * rapidezVertical;
            rb.AddForce(Vector3.up * fuerzaLevitacion * factor);
        }
        else
        {
            float factor = Mathf.Max(0, distancia * rapidezVertical * 5);
            rb.AddForce(Vector3.up * fuerzaLevitacion * factor);
        }
    }
    private void AlcanzarPosicion(Vector3 posObjetivo, float rapidezHorizontal, float propulsionFrontal)
    {
        Vector3 vectorHaciaObjetivo = posObjetivo - transform.position;
        float velocidadRelativa = ObjetoPerseguido.GetComponent<Rigidbody>().velocity.magnitude - rb.velocity.magnitude;
        float angulo = Vector3.Angle(vectorHaciaObjetivo, GetComponent<Rigidbody>().velocity);

        if ((velocidadRelativa > 0) || (angulo < 70))
        {
            float factor = vectorHaciaObjetivo.magnitude * rapidezHorizontal;
            rb.AddForce(vectorHaciaObjetivo * propulsionFrontal * factor);
            rb.transform.LookAt(new Vector3(posObjetivo.x, rb.transform.position.y, posObjetivo.z));
        }
        else
        {
            //Ir frenando...    Tarea: cambiar la siguiente instrucción por rb.addForce...  con el mismo efecto.
            rb.velocity = rb.velocity * 0.95f;
            rb.transform.LookAt(new Vector3(posObjetivo.x, rb.transform.position.y, posObjetivo.z));
        }
    }
}
