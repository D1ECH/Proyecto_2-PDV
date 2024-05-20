using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moverPlataforma : MonoBehaviour
{
    public float velocFrontal = 0;
    public float velocGiro = 0;
    Rigidbody rb;                                              //Puede congelar (Freeze) rotaciones en X Z; también la posición Y.

    private void Start()
    {
        velocGiro =  5;
        velocFrontal = 20;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up *  velocGiro * Time.deltaTime);
        rb.velocity = transform.forward * velocFrontal;

        if (Time.time > 40) velocGiro = -15;
        if (Time.time > 60) velocGiro = 0;
        if (Time.time > 80) velocGiro = -15;
    }
    

}
