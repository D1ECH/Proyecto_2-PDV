using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public GameObject targetObject;  // El objeto que debe estar sobre el plano para activar el ascensor
    public float targetHeight = 10f; // La altura a la que debe llegar el ascensor
    public float speed = 2f;         // La velocidad a la que se eleva el ascensor
    public float delayTime = 2f;     // Tiempo de espera antes de que la plataforma comience a elevarse

    private bool isActivated = false;  // Indica si el ascensor está activado
    private bool isDelaying = false;   // Indica si el ascensor está en el periodo de espera

    void Update()
    {
        // Verificar si el objeto está sobre el plano
        if (IsTargetObjectOnElevator() && !isActivated && !isDelaying)
        {
            StartCoroutine(WaitAndActivate());
        }

        // Si el ascensor está activado y no ha alcanzado la altura objetivo, elevarlo
        if (isActivated && transform.position.y < targetHeight)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }

    private IEnumerator WaitAndActivate()
    {
        isDelaying = true;  // Indica que estamos en el periodo de espera
        yield return new WaitForSeconds(delayTime);  // Esperar el tiempo especificado
        isActivated = true;  // Activar el ascensor después de la espera
        isDelaying = false;  // Salir del periodo de espera
    }

    private bool IsTargetObjectOnElevator()
    {
        Collider targetCollider = targetObject.GetComponent<Collider>();
        Collider elevatorCollider = GetComponent<Collider>();

        if (targetCollider == null || elevatorCollider == null)
        {
            return false;
        }

        return elevatorCollider.bounds.Intersects(targetCollider.bounds);
    }

    void OnDrawGizmos()
    {
        // Dibujar una línea que representa la altura objetivo en la escena
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, targetHeight, transform.position.z));
    }
}
