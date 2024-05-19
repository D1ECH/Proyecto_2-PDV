using UnityEngine;

public class PlatformHingeController : MonoBehaviour
{
    public HingeJoint[] hingeJoints;  // Array de HingeJoints a controlar
    public GameObject player;         // Referencia al jugador
    public Transform platform;        // Plataforma que detectará al jugador
    public float detectionRadius = 1f; // Radio de detección del jugador
    public float newBreakForce = 500f; // Nueva fuerza de ruptura cuando el jugador esté encima
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
        // Verificar si el jugador está sobre la plataforma
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
        // Verificar si el jugador está dentro del radio de detección
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
        // Dibujar un gizmo para visualizar el radio de detección en la escena
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(platform.position, detectionRadius);
    }
}
