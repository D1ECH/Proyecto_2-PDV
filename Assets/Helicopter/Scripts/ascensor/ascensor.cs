using System.Collections;
using UnityEngine;

public class ascensor : MonoBehaviour
{
    public GameObject targetObject;  // El objeto que debe estar sobre el plano para activar el ascensor
    public float targetHeight = 10f; // La altura a la que debe llegar el ascensor
    public float speed = 2f;         // La velocidad a la que se eleva el ascensor
    public float delayTime = 2f;     // Tiempo de espera antes de que la plataforma comience a elevarse
    public float waitAtTopTime = 2f; // Tiempo de espera en la posici�n superior antes de bajar

    private bool isActivated = false;  // Indica si el ascensor est� activado
    private bool isDelaying = false;   // Indica si el ascensor est� en el periodo de espera
    private Vector3 originalPosition;  // Posici�n original del ascensor

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // Verificar si el objeto est� sobre el plano
        if (IsTargetObjectOnElevator() && !isActivated && !isDelaying)
        {
            StartCoroutine(WaitAndActivate());
        }

        // Si el ascensor est� activado y no ha alcanzado la altura objetivo, elevarlo
        if (isActivated && transform.position.y < targetHeight)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            if (transform.position.y >= targetHeight)
            {
                StartCoroutine(WaitAtTopAndReturn());
            }
        }

        // Si el ascensor ha sido activado y est� volviendo a la posici�n original
        if (!isActivated && transform.position.y > originalPosition.y)
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }
    }

    private IEnumerator WaitAndActivate()
    {
        isDelaying = true;  // Indica que estamos en el periodo de espera
        yield return new WaitForSeconds(delayTime);  // Esperar el tiempo especificado
        isActivated = true;  // Activar el ascensor despu�s de la espera
        isDelaying = false;  // Salir del periodo de espera
    }

    private IEnumerator WaitAtTopAndReturn()
    {
        yield return new WaitForSeconds(waitAtTopTime);  // Esperar en la posici�n superior
        isActivated = false;  // Desactivar el ascensor para que vuelva a la posici�n original
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
        // Dibujar una l�nea que representa la altura objetivo en la escena
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, targetHeight, transform.position.z));
    }
}
