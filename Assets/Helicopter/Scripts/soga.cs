using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    public float swingForce = 10f;   // Fuerza de balanceo
    public KeyCode grabKey = KeyCode.F;  // Tecla para agarrar la soga
    public KeyCode releaseKey = KeyCode.G; // Tecla para soltarse
    public Transform handTransform;  // Transform del punto donde el personaje se agarra

    private Rigidbody rb;
    private HingeJoint hingeJoint;
    private bool isSwinging;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isSwinging && Input.GetKeyDown(releaseKey))
        {
            Release();
        }

        if (isSwinging)
        {
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
        hingeJoint = gameObject.AddComponent<HingeJoint>();
        hingeJoint.connectedBody = ropeSegment;
        hingeJoint.anchor = handTransform.localPosition;
        hingeJoint.autoConfigureConnectedAnchor = false;
        hingeJoint.connectedAnchor = ropeSegment.transform.InverseTransformPoint(handTransform.position);

        isSwinging = true;
    }

    private void Release()
    {
        Destroy(hingeJoint);
        isSwinging = false;
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

