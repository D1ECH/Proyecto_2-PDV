using System;
using UnityEngine;

public class Soga : MonoBehaviour
{
    public float swingForce = 10f; // Fuerza de balanceo
    public KeyCode grabKey = KeyCode.F; // Tecla para agarrar la soga
    public KeyCode releaseKey = KeyCode.G; // Tecla para soltarse
    public KeyCode jumpKey = KeyCode.Space; // Tecla para saltar desde la soga

    private Rigidbody rb;
    private bool isSwinging;
    private new HingeJoint hingeJoint;
    private Rigidbody connectedRopeSegment;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isSwinging)
        {
            if (Input.GetKeyDown(releaseKey) || Input.GetKeyDown(jumpKey))
            {
                Release();
            }

            ApplySwingForce();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(grabKey) && other.CompareTag("rope"))
        {
            Grab(other.GetComponent<Rigidbody>());
        }
    }

    private void Grab(Rigidbody ropeSegment)
    {
        Debug.Log("Trying to grab the rope");

        hingeJoint = gameObject.AddComponent<HingeJoint>();
        hingeJoint.connectedBody = ropeSegment;
        hingeJoint.anchor = Vector3.zero; // Ajusta según sea necesario
        hingeJoint.autoConfigureConnectedAnchor = false;
        hingeJoint.connectedAnchor = ropeSegment.transform.InverseTransformPoint(transform.position);

        connectedRopeSegment = ropeSegment;
        isSwinging = true;
        rb.useGravity = false;

        Debug.Log("Grabbed the rope successfully");
    }

    private void Release()
    {
        Debug.Log("Releasing the rope");

        if (hingeJoint != null)
        {
            Destroy(hingeJoint);
        }

        isSwinging = false;
        rb.useGravity = true;

        // Añadir un pequeño impulso al saltar
        if (Input.GetKeyDown(jumpKey))
        {
            rb.AddForce(Vector3.up * swingForce, ForceMode.Impulse);
        }

        Debug.Log("Released the rope successfully");
    }

    private void ApplySwingForce()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector3.left * swingForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector3.right * swingForce);
        }
    }
}
