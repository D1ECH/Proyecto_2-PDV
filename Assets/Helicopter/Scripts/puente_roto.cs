using UnityEngine;

public class PlatformHingeController : MonoBehaviour
{
    public HingeJoint[] hingeJoints;  // Array de HingeJoints a controlar
    public GameObject player;         // Referencia al jugador
    public Transform platform;        // Plataforma que detectar� al jugador
    public float detectionRadius = 1f; // Radio de detecci�n del jugador
    public float newBreakForce = 500f; // Nueva fuerza de ruptura cuando el jugador est� encima
    private float originalBreakForce;  // Para almacenar la fuerza de ruptura original

    void Start()
    {
        // Inicializar la fuerza de ruptura original como infinita
        foreach (var hingeJoint in hingeJoints)
        {
            originalBreakForce = hingeJoint.breakForce;
            hingeJoint.breakForce = Mathf.Infinity;
        }
    }

    void Update()
    {
        // Verificar si el jugador est� sobre la plataforma
        if (IsPlayerOnPlatform())
        {
            SetHingeJointBreakForce(newBreakForce);  // Reducir la fuerza de ruptura
        }
        else
        {
            SetHingeJointBreakForce(Mathf.Infinity);  // Restaurar la fuerza de ruptura
        }
    }

    private bool IsPlayerOnPlatform()
    {
        // Verificar si el jugador est� dentro del radio de detecci�n
        return Vector3.Distance(player.transform.position, platform.position) <= detectionRadius;
    }

    private void SetHingeJointBreakForce(float breakForce)
    {
        foreach (var hingeJoint in hingeJoints)
        {
            hingeJoint.breakForce = breakForce;
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar un gizmo para visualizar el radio de detecci�n en la escena
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(platform.position, detectionRadius);
    }
}
