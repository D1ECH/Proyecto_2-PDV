using UnityEngine;

public class puente_roto : MonoBehaviour
{
    public GameObject cylinder1HingeJoint; // Referencia al primer HingeJoint del cilindro
    public GameObject cylinder2HingeJoint; // Referencia al segundo HingeJoint del cilindro
    public float newBreakForce = 50f; // Nueva fuerza de rotura cuando el jugador está en la plataforma
    public float defaultBreakForce = Mathf.Infinity; // Fuerza de rotura predeterminada

    //private bool playerOnPlatform = false;

    void Start()
    {
        // Asegúrate de que los HingeJoint tengan la fuerza de rotura predeterminada
        cylinder1HingeJoint.GetComponent<HingeJoint>().breakForce = defaultBreakForce;
        cylinder2HingeJoint.GetComponent<HingeJoint>().breakForce = defaultBreakForce;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //playerOnPlatform = true;
            ModifyBreakForce(newBreakForce);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //playerOnPlatform = false;
            ModifyBreakForce(defaultBreakForce);
        }
    }

    void ModifyBreakForce(float breakForce)
    {
        cylinder1HingeJoint.GetComponent<HingeJoint>().breakForce = breakForce;
        cylinder2HingeJoint.GetComponent<HingeJoint>().breakForce = breakForce;
    }
}
